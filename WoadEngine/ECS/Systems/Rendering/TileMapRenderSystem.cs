using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WoadEngine.ECS;
using WoadEngine.ECS.Components.Physics;
using WoadEngine.ECS.Components.Rendering;
using WoadEngine.Rendering;
using WoadEngine.Tiles;

namespace WoadEngine.ECS.Systems.Rendering;

public sealed class TileMapRenderSystem : IRenderSystem
{
    public void Draw(World world, float dt)
    {
        var transforms = world.GetStore<Transform>();
        var maps = world.GetStore<TileMapComponent>();

        GetView(world, out Vector2 camPos, out Vector2 viewMin, out Vector2 viewMax);

        foreach (var entityId in maps.DenseEntities)
        {
            if (!transforms.Has(entityId))
                continue;
            
            var t = transforms.Get(entityId);
            var mapComp = maps.Get(entityId);
            var mapPos = new Vector2(t.Position.X, t.Position.Y);

            DrawMap(mapComp.Map, mapPos, camPos, viewMin, viewMax);
        }
    }

        private static void DrawMap(TileMap map, Vector2 mapPos, Vector2 camPos, Vector2 viewMin, Vector2 viewMax)
    {
        foreach (var layer in map.Layers)
        {
            if (!layer.Visible || layer.Opacity <= 0f)
                continue;
 
            // Parallax: a factor of 1 moves with the world; less than 1 lags behind the camera.
            Vector2 layerPos = mapPos + (camPos * (Vector2.One - layer.Parallax));
 
            if (!map.TryGetCellRange(viewMin, viewMax, layerPos, out Point min, out Point max))
                continue;
 
            Color tint = Color.White * layer.Opacity;
 
            for (int y = min.Y; y <= max.Y; y++)
            {
                for (int x = min.X; x <= max.X; x++)
                {
                    int tileId = layer.Get(x, y);
                    if (tileId == 0 || !map.TryGetDef(tileId, out var def))
                        continue;
 
                    Core.SpriteBatch.Draw(
                        def.Region.Texture,
                        map.CellWorldRect(x, y, layerPos),
                        def.Region.SourceRectangle,
                        tint,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        layer.Depth
                    );
                }
            }
        }
    }
 
    /// <summary>
    /// Works out the camera position and the world-space area currently on screen.
    /// With no active Camera2D, world space is treated as screen space.
    /// </summary>
    private static void GetView(World world, out Vector2 camPos, out Vector2 viewMin, out Vector2 viewMax)
    {
        var vp = Core.GraphicsDevice.Viewport;
        camPos = Vector2.Zero;
        viewMin = Vector2.Zero;
        viewMax = new Vector2(vp.Width, vp.Height);
 
        int camId = world.GetActiveCameraId();
        var cameras = world.GetStore<Camera2D>();
        var transforms = world.GetStore<Transform>();
 
        if (!cameras.Has(camId) || !transforms.Has(camId))
            return;
 
        var cam = cameras.Get(camId);
        if (!cam.IsActive)
            return;
 
        var camTr = transforms.Get(camId);
        camPos = new Vector2(camTr.Position.X, camTr.Position.Y);
 
        // Map the screen corners back into world space and take their bounding box
        // (handles zoom and rotation).
        Matrix screenToWorld = Matrix.Invert(cam.View);
        Vector2 a = Vector2.Transform(Vector2.Zero, screenToWorld);
        Vector2 b = Vector2.Transform(new Vector2(vp.Width, 0), screenToWorld);
        Vector2 c = Vector2.Transform(new Vector2(0, vp.Height), screenToWorld);
        Vector2 d = Vector2.Transform(new Vector2(vp.Width, vp.Height), screenToWorld);
 
        viewMin = Vector2.Min(Vector2.Min(a, b), Vector2.Min(c, d));
        viewMax = Vector2.Max(Vector2.Max(a, b), Vector2.Max(c, d));
    }
}