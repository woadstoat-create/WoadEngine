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

        foreach (var entityId in maps.DenseEntities)
        {
            if (!transforms.Has(entityId))
                continue;
            
            var t = transforms.Get(entityId);
            var mapComp = maps.Get(entityId);
            DrawMap(mapComp.Map, new Vector2(t.Position.X, t.Position.Y));
        }
    }

    private void DrawMap(TileMap map, Vector2 mapWorldPos)
    {
        for (int layerIndex = 0; layerIndex < map.Layers.Count; layerIndex++)
        {
            var layer = map.Layers[layerIndex];
            if (!layer.Visible || layer.Opacity <= 0f)
                continue;

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    int tileId = layer.Get(x, y);
                    if (tileId == 0)
                        continue;
                    if (!map.TryGetDef(tileId, out var def))
                        continue;

                    var dest = map.CellWorldRect(x, y, mapWorldPos);

                    map.TryGetDef(tileId, out def);

                    TextureRegion r = def.Region;

                    Core.SpriteBatch.Draw(
                        r.Texture,
                        dest,
                        r.SourceRectangle,
                        Color.White * layer.Opacity,
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        layer.Depth
                    );
                }
            }
        }
    }
}