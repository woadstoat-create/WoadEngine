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
    public Point TileSize { get; }
    public int Width { get; }
    public int Height { get; }

    public TextureAtlas Atlas { get; }

    private readonly Dictionary<int, TiledDef> _defs = new();
    public IReadOnlyDictionary<int, TiledDef> Defs => _defs;
    public List<TileLayer> Layers { get; } = new();

    public TileMap(TextureAtlas atlas, Point tileSize, int width, int height)
    {
        Atlas = atlas;
        TileSize = tileSize;
        Width = width;
        Height = height;
    }

    public void AddDef(TiledDef def)
    {
        if (def.Id == 0) Logger.Exception(new ArgumentException(), "Tile ID 0 is reserved for Empty");
        _defs[def.Id] = def;
    }

    public bool TryGetDef(int tileId, out TiledDef def) => _defs.TryGetValue(tileId, out def);

    public TileLayer AddLayer(string name)
    {
        var layer = new TileLayer(name, Width, Height);
        Layers.Add(layer);
        return layer;
    }

    public Rectangle CellWorldRect(int x, int y, Vector2 mapWorldPos) => new Rectangle(
        (int)mapWorldPos.X + (x * TileSize.X),
        (int)mapWorldPos.Y + (y * TileSize.Y),
        TileSize.X,
        TileSize.Y
    );
}