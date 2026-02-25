// --------------------------------------------------------------------------------------------
// WoadEngine
// Copyright (c) 2026 Woad Stoat Studios, All Rights Reserved.
// This source code may not be copied, modified, disclosed or distributed without permission.
//---------------------------------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace WoadEngine.ECS.Components.Rendering;

#region Mesh Rendering Component

/// <summary>
/// ECS component describing how an entity should be drawn as a 3D mesh/model.
///
/// This is a data-only component intended to be consumed by a render system,
/// typically something like <c>MeshRenderSystem</c> that queries:
/// <c>Transform3D + MeshRenderer</c>.
///
/// Notes:
/// - Keep this component lightweight (no ContentManager, no GraphicsDevice, no draw calls).
/// - Prefer handles/IDs rather than storing <c>Model</c> or <c>Effect</c> directly.
/// - Rendering policy (culling, sorting, instancing) belongs in the render system.
/// </summary>
public struct MeshRenderer
{
    /// <summary>
    /// Asset identifier/handle for the mesh/model to render.
    /// Example values: "models/crate", "models/player", or an integer handle from an AssetRegistry.
    /// </summary>
    public string MeshId;

    /// <summary>
    /// Asset identifier/handle for the material/shader configuration to use.
    /// This might map to an Effect + textures + parameters in your engine.
    /// </summary>
    public string MaterialId;

    /// <summary>
    /// Optional tint/override colour (useful for debug, damage flashes, team colouring).
    /// Whether this is used depends on your material system.
    /// </summary>
    public Color Color;

    /// <summary>
    /// Optional explicit per-entity visibility toggle.
    /// If false, the render system should skip drawing this entity.
    /// </summary>
    public bool Visible;

    /// <summary>
    /// Whether this mesh should cast shadows (if/when you add shadows).
    /// </summary>
    public bool CastShadows;

    /// <summary>
    /// Whether this mesh should receive shadows (if/when you add shadows).
    /// </summary>
    public bool ReceiveShadows;

    /// <summary>
    /// Optional render layer / bucket. Useful if you want coarse grouping:
    /// e.g. Opaque = 0, Cutout = 1, Transparent = 2, UI/Debug = 3.
    /// Your render system defines what these values mean.
    /// </summary>
    public int RenderLayer;

    /// <summary>
    /// Optional depth-sort hint used primarily for transparent rendering.
    /// Render systems can compute depth automatically from camera distance;
    /// this is an override if you need it.
    /// </summary>
    public float SortKey;

    /// <summary>
    /// Creates a mesh renderer with sensible defaults.
    /// </summary>
    public static MeshRenderer Create(string meshId, string materialId)
    {
        return new MeshRenderer
        {
            MeshId = meshId,
            MaterialId = materialId,
            Color = Color.White,
            Visible = true,
            CastShadows = true,
            ReceiveShadows = true,
            RenderLayer = 0,
            SortKey = 0f
        };
    }
}
#endregion