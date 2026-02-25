using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WoadEngine.Rendering;

namespace WoadEngine.ECS.Components.Rendering;

public enum BillboardMode { Spherical, Cylindrical }

public struct BillboardSprite
{
    public Texture2D Texture;
    public Rectangle? SourceRect;   // optional atlas
    public Vector2 Size;            // world units (width,height)
    public Color Tint;
    public float Alpha;             // 0..1
    public float SortBias;          // optional, small tweak
    public BillboardMode Mode;
}