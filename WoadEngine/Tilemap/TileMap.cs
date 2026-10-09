using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using WoadEngine.Diagnostics;
using WoadEngine.Rendering;

namespace WoadEngine.Tiles;

public sealed class TileMap
{
    private const float LayerDepthStart = 0.1f;
    private const float LayerDepthStep = 0.01f;

    public Point TileSize { get; }
    public int Width { get; }
    public int Height { get; }

    public TextureAtlas Atlas { get; }

    public bool BoundsAreSolid {get; set; } = false;

    private readonly Dictionary<int, TileDef> _defs = new();
    public IReadOnlyDictionary<int, TileDef> Defs => _defs;
    public List<TileLayer> Layers { get; } = new();

    public TileMap(TextureAtlas? atlas, Point tileSize, int width, int height)
    {
        if (tileSize.X <= 0 || tileSize.Y <= 0) Logger.Exception(new ArgumentOutOfRangeException(nameof(tileSize)), $"Cannot have tile size of {tileSize}.");
        if (width <= 0) Logger.Exception(new ArgumentOutOfRangeException(nameof(width)), $"Cannot have map of {width} width.");
        if (height <= 0) Logger.Exception(new ArgumentOutOfRangeException(nameof(height)), $"Cannot have map of {height} height.");

        Atlas = atlas;
        TileSize = tileSize;
        Width = width;
        Height = height;
    }

    public void AddDef(TileDef def)
    {
        if (def.Id == 0) Logger.Exception(new ArgumentException(nameof(def)), "Tile ID 0 is reserved for Empty");
        _defs[def.Id] = def;
    }

    public void AddDef(int id, string regionName, TileFlags flags = TileFlags.None, string? tag = null)
    {
        if (Atlas == null) Logger.Exception(new InvalidOperationException("Cannot add tile def without a texture atlas."));
        AddDef(new TileDef(id, Atlas.GetRegion(regionName), flags, tag));
    }

    public int AddTileset(Texture2D texture, int firstId = 1, int margin = 0, int spacing = 0)
    {
        int columns = (texture.Width -(margin * 2) + spacing) / (TileSize.X + spacing);
        int rows = (texture.Height - (margin * 2) + spacing) / (TileSize.Y + spacing);

        int id = firstId;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int x = margin + (col * (TileSize.X + spacing));
                int y = margin + (row * (TileSize.Y + spacing));
                AddDef(new TileDef(id++, new TextureRegion(texture, x, y, TileSize.X, TileSize.Y)));
            }
        }

        return columns * rows;
    }

    public int AddDefsFromAtlas(int firstId = 1, int margin = 0, int spacing = 0)
    {
        if (Atlas == null) Logger.Exception(new InvalidOperationException("This map has no atlas."));
        return AddTileset(Atlas.Texture, firstId, margin, spacing);
    }

    public TileLayer AddLayer(string name)
    {
        var layer = new TileLayer(name, Width, Height)
        {
            Depth = LayerDepthStart + (Layers.Count * LayerDepthStep)
        };
        Layers.Add(layer);
        return layer;
    }

    public TileLayer? GetLayer(string name)
    {
        foreach (var layer in Layers)
        {
            if (layer.Name == name) return layer;
        }
        return null;
    }

    public TileFlags GetFlags(int x, int y)
    {
        var flags = TileFlags.None;
        bool anyCollisionLayer = false;

        foreach (var layer in Layers)
        {
            if (!layer.CollisionEnabled) continue;
            anyCollisionLayer = true;

            if (TryGetDef(layer.Get(x, y), out var def))
            {
                flags |= def.Flags;
            }
        }

        bool outOfBounds = (uint)x >= (uint)Width || (uint)y >= (uint)Height;
        if (BoundsAreSolid && outOfBounds && anyCollisionLayer)
        {
            flags |= TileFlags.Solid;
        }
        return flags;
    }

    public bool IsSolid(int x, int y) => (GetFlags(x, y) & TileFlags.Solid) != 0;

    public bool TryGetDef(int tileId, out TileDef def) => _defs.TryGetValue(tileId, out def);

    public Point WorldToCell(Vector2 worldPos, Vector2 mapWorldPos) => new Point(
        (int)MathF.Floor((worldPos.X - mapWorldPos.X) / TileSize.X),
        (int)MathF.Floor((worldPos.Y - mapWorldPos.Y) / TileSize.Y)
    );
 
    public Rectangle CellWorldRect(int x, int y, Vector2 mapWorldPos) => new Rectangle(
        (int)MathF.Floor(mapWorldPos.X) + (x * TileSize.X),
        (int)MathF.Floor(mapWorldPos.Y) + (y * TileSize.Y),
        TileSize.X,
        TileSize.Y
    );

    public bool TryGetCellRange(Vector2 worldMin, Vector2 worldMax, Vector2 mapWorldPos, out Point min, out Point max)
    {
        min = WorldToCell(worldMin, mapWorldPos);
        max = WorldToCell(worldMax, mapWorldPos);

        min = new Point(Math.Max(min.X, 0), Math.Max(min.Y, 0));
        max = new Point(Math.Min(max.X, Width - 1), Math.Min(max.Y, Height - 1));

        return min.X <= max.X && min.Y <= max.Y;
    }
}