using Microsoft.Xna.Framework;
using WoadEngine.Rendering;

namespace WoadEngine.Tiles;

[System.Flags]
public enum TileFlags : ushort
{
    None = 0,
    Solid = 1 << 0,
    OneWay = 1 << 1,
    Damage = 1 << 2,
}

public readonly struct TileDef
{
    public readonly int Id;

    public readonly TextureRegion Region;

    public readonly TileFlags Flags;

    public readonly string? Tag;

    public TileDef(int id, TextureRegion region, TileFlags flags = TileFlags.None, string? tag = null)
    {
        Id = id;
        Region = region;
        Flags = flags;
        Tag = tag;
    }

    public bool IsSolid => (Flags & TileFlags.Solid) != 0;
    public bool IsOneWay => (Flags & TileFlags.OneWay) != 0;
    public bool IsDamage => (Flags & TileFlags.Damage) != 0;
}