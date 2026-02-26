using Microsoft.Xna.Framework;
using WoadEngine.Rendering;

namespace WoadEngine.Tiles;

public readonly struct TiledDef
{
    public readonly int Id;

    public readonly TextureRegion Region;

    public readonly TileFlags Flags;

    public readonly string? Tag;

    public TiledDef(int id, TextureRegion region, TileFlags flags = TileFlags.None, string? tag = null)
    {
        Id = id;
        Region = region;
        Flags = flags;
        Tag = tag;
    }

    public bool IsSolid => (Flags & TileFlags.Solid) != 0;
}

[System.Flags]
public enum TileFlags : ushort
{
    None = 0,
    Solid = 1 << 0,
    OneWay = 1 << 1,
    Damage = 1 << 2,
}