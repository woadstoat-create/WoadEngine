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

    public int Get(int x, int y)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height) return 0;
        return _tiles[(y * Width) + x];
    }

    public void Set(int x, int y, int tileId)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height) return;
        _tiles[(y * Width) + x] = tileId;

        // Mark chunk dirty
    }

    public void Fill(int tileId)
    {
        System.Array.Fill(_tiles, tileId);
       
        // Mark chunk dirty
    }
}