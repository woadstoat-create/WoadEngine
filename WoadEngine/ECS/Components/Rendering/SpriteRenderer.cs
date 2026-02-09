// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WoadEngine.Rendering;

namespace WoadEngine.ECS.Components.Rendering;

#region Sprite Rendering Component

/// <summary>
/// ECS component describing how an entity should be drawn as a 2D sprite.
///
/// This is a data-only component intended to be consumed by a render system,
/// typically something like <c>SpriteRenderSystem</c> that queries:
/// <c>Transform + SpriteRenderer</c>.
///
/// Notes:
/// - This component should not perform any drawing itself.
/// - Keep it lightweight; behaviour belongs in systems.
/// </summary>
public struct SpriteRenderer
{
    /// <summary>
    /// The texture region (source rectangle + texture reference) to draw.
    /// Commonly used for sprite sheets / atlases.
    /// </summary>
    public TextureRegion Region;

    /// <summary>
    /// Tint colour applied when drawing. Use <see cref="Color.White"/> for no tint.
    /// </summary>
    public Color Color;

    /// <summary>
    /// Origin/pivot point in source-pixel space.
    /// (e.g., <c>(0,0)</c> = top-left, <c>(Region.Width/2, Region.Height/2)</c> = centre).
    /// </summary>
    public Vector2 Origin;

    /// <summary>
    /// Non-uniform scale applied at draw time. Prefer <see cref="Vector2.One"/> as default.
    /// </summary>
    public Vector2 Scale;

    /// <summary>
    /// Optional sprite mirroring (flip horizontally/vertically).
    /// </summary>
    public SpriteEffects Effect;
    
    /// <summary>
    /// SpriteBatch layer depth (typically 0..1).
    /// Higher values usually draw "in front" when using <c>SpriteSortMode.BackToFront</c>.
    /// Exact behaviour depends on your SpriteBatch sort mode.
    /// </summary>
    public float Layer;

    /// <summary>
    /// Convenience: render width in world/screen units assuming 1 unit per pixel pre-scale.
    /// </summary>
    public float Width => Region.Width * Scale.X;

    /// <summary>
    /// Convenience: render height in world/screen units assuming 1 unit per pixel pre-scale.
    /// </summary>
    public float Height => Region.Height * Scale.Y;

    /// <summary>
    /// Creates a SpriteRenderer with sensible defaults (white tint, scale 1, no effects).
    /// This is optional but helps avoid the common "Scale = (0,0)" pitfall when using structs.
    /// </summary>
    public static SpriteRenderer Create(TextureRegion region, Vector2? origin = null, Vector2? scale = null, float layer = 0f)
    {
        return new SpriteRenderer
        {
            Region = region,
            Color = Color.White,
            Origin = origin ?? Vector2.Zero,
            Scale = scale ?? Vector2.One,
            Effect = SpriteEffects.None,
            Layer = layer
        };
    }
}
#endregion