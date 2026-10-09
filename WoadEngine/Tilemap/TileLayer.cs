using System;
using Microsoft.Xna.Framework;
using WoadEngine.Diagnostics;

namespace WoadEngine.Tiles;

public sealed class TileLayer
{
    public string Name { get; }
    public int Width { get; }
    public int Height { get; }

    private readonly int[] _tiles;

    public bool Visible { get; set; } = true;
    public float Opacity { get; set; } = 1f;

    public float Depth { get; set; } = 0.5f;

    public Vector2 Parallax { get; set; } = Vector2.One;

    public bool CollisionEnabled { get; set; } = false;

    public TileLayer(string name, int width, int height)
    {
        if (width <= 0) Logger.Exception(new ArgumentOutOfRangeException(nameof(width)), $"Cannot have layer of {width} width.");
        if (height <= 0) Logger.Exception(new ArgumentOutOfRangeException(nameof(width)), $"Cannot have layer of {height} height.");

        Name = name;
        Width = width;
        Height = height;
        _tiles = new int[width * height];
    }

    public bool InBounds(int x, int y) => (uint)x < (uint)Width && (uint)y < (uint)Height;

    public int Get(int x, int y)
    {
        if (!InBounds(x, y)) return 0;
        return _tiles[(y * Width) + x];
    }

    public void Set(int x, int y, int tileId)
    {
        if (!InBounds(x, y)) return;
        _tiles[(y * Width) + x] = tileId;
    }

    public void Fill(int tileId) => Array.Fill(_tiles, tileId);

    public void FillRect(int x, int y, int width, int height, int tileId)
    {
        for (int cy = y; cy < y + height; cy++)
        {
            for (int cx = x; cx < x + width; cx++)
            {
                Set(cx, cy, tileId);
            }
        }
    }

    public void Clear() => Array.Clear(_tiles);
}