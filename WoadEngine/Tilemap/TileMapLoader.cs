
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WoadEngine.Diagnostics;

namespace WoadEngine.Tiles;

public static class TileMapLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static TileMap Load(string path)
    {
        MapFile file; 
        using (var stream = TitleContainer.OpenStream(Path.Combine(Core.Content.RootDirectory, path)))
        {
            file = JsonSerializer.Deserialize<MapFile>(stream, Options) ?? throw new InvalidDataException($"{path} is empty.");
        }
        
        if (file.Layers.Count == 0) Logger.Exception(new InvalidOperationException($"Map file at {path} has no layers."));

        int height = file.Layers[0].Rows.Count;
        int width = SplitRow(file.Layers[0].Rows[0]).Length;
        var map = new TileMap(null, new Microsoft.Xna.Framework.Point(file.TileWidth, file.TileHeight), width, height);

        foreach (var tileset in file.Tilesets)
        {
            map.AddTileset(Core.Content.Load<Texture2D>(tileset.Texture), tileset.FirstId, tileset.Margin, tileset.Spacing);
        }

        foreach (var tile in file.Tiles)
        {
            if (!map.TryGetDef(tile.Id, out var def))
                Logger.Exception(new InvalidOperationException($"Map file at {path} has tile with id {tile.Id} but no corresponding tileset."));
            
            var flags = Enum.Parse<TileFlags>(tile.Flags, ignoreCase: true);
            map.AddDef(new TileDef(tile.Id, def.Region, flags, tile.Tag));
        }

        foreach (var layerFile in file.Layers)
        {
            if (layerFile.Rows.Count != height)
                Logger.Exception(new InvalidOperationException($"Map file at {path} has layer {layerFile.Name} with {layerFile.Rows.Count} rows but expected {height}."));

            var layer = map.AddLayer(layerFile.Name);
            layer.Visible = layerFile.Visible;
            layer.Opacity = layerFile.Opacity;
            layer.CollisionEnabled = layerFile.Collision;
            if (layerFile.Depth is float depth) layer.Depth = depth;
            if (layerFile.Parallax is { Length: 2 } p) layer.Parallax = new Microsoft.Xna.Framework.Vector2(p[0], p[1]);

            for (int y = 0; y < height; y++)
            {
                string[] ids = SplitRow(layerFile.Rows[y]);
                if (ids.Length != width)
                    Logger.Exception(new InvalidOperationException($"Map file at {path} has layer {layerFile.Name} with row {y} containing {ids.Length} columns but expected {width}."));
                
                for (int x = 0; x < width; x++)
                    layer.Set(x, y, ids[x] == "." ? 0 : int.Parse(ids[x]));
            }
        }

        return map;
    }

    private static string[] SplitRow(string row) => row.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    private sealed class MapFile
    {
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public List<TilesetFile> Tilesets { get; set; } = new();
        public List<TileFile> Tiles { get; set; } = new();
        public List<LayerFile> Layers { get; set; } = new();
    }

    private sealed class TilesetFile
    {
        public string Texture { get; set; } = "";
        public int FirstId { get; set; } = 1;
        public int Margin { get; set; }
        public int Spacing { get; set; }
    }
 
    private sealed class TileFile
    {
        public int Id { get; set; }
        public string Flags { get; set; } = "None";
        public string? Tag { get; set; }
    }
 
    private sealed class LayerFile
    {
        public string Name { get; set; } = "";
        public bool Visible { get; set; } = true;
        public float Opacity { get; set; } = 1f;
        public float? Depth { get; set; }
        public float[]? Parallax { get; set; }
        public bool Collision { get; set; }
        public List<string> Rows { get; set; } = new();
    }
    
}