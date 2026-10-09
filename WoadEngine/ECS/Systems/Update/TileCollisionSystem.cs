

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WoadEngine.ECS.Components.Physics;
using WoadEngine.ECS.Components.Rendering;
using WoadEngine.Tiles;

namespace WoadEngine.ECS.Systems.Update;

public class TileCollisionSystem : ISystem
{
    private const float OneWayTolerance = 0.01f;

    private readonly List<(TileMap map, Vector2 pos)> _maps = new();

     public void Update(World world, float dt)
    {
        var transforms = world.GetStore<Transform>();
        var velocities = world.GetStore<Velocity>();
        var colliders = world.GetStore<Collider2D>();
        var tileMaps = world.GetStore<TileMapComponent>();
 
        _maps.Clear();
        foreach (int mapId in tileMaps.DenseEntities)
        {
            if (!transforms.Has(mapId)) continue;
            var p = transforms.Get(mapId).Position;
            _maps.Add((tileMaps.Get(mapId).Map, new Vector2(p.X, p.Y)));
        }
 
        var ents = colliders.DenseEntities;
        for (int i = 0; i < ents.Length; i++)
        {
            int id = ents[i];
            if (!transforms.Has(id) || !velocities.Has(id)) continue;
 
            ref var t = ref transforms.Get(id);
            ref var v = ref velocities.Get(id);
            ref var c = ref colliders.DenseComponents[i];
 
            // Collider box in world space (same maths as DebugColliderRenderSystem).
            Vector2 boxOffset = c.Offset + new Vector2(c.Rect.X, c.Rect.Y);
            Vector2 size = new(c.Rect.Width, c.Rect.Height);
            Vector2 box = new Vector2(t.Position.X, t.Position.Y) + boxOffset;
            Vector2 move = new Vector2(v.Value.X, v.Value.Y) * dt;
 
            // Move one axis at a time so walls and floors resolve independently.
            if (move.X != 0f)
            {
                box.X += move.X;
                foreach (var (map, mapPos) in _maps)
                {
                    if (SweepX(map, mapPos, ref box, size, move.X))
                    {
                        v.Value.X = 0f;
                        world.Events.Publish(new TileHitEvent(id, new Vector2(-MathF.Sign(move.X), 0f)));
                    }
                }
            }
 
            if (move.Y != 0f)
            {
                box.Y += move.Y;
                foreach (var (map, mapPos) in _maps)
                {
                    if (SweepY(map, mapPos, ref box, size, move.Y))
                    {
                        v.Value.Y = 0f;
                        world.Events.Publish(new TileHitEvent(id, new Vector2(0f, -MathF.Sign(move.Y))));
                    }
                }
            }
 
            t.Position.X = box.X - boxOffset.X;
            t.Position.Y = box.Y - boxOffset.Y;
 
            foreach (var (map, mapPos) in _maps)
            {
                if (TryFindCell(map, CellRange(box.X, size.X, mapPos.X, map.TileSize.X),
                                     CellRange(box.Y, size.Y, mapPos.Y, map.TileSize.Y),
                                     TileFlags.Damage, out Point cell))
                {
                    world.Events.Publish(new TileDamageEvent(id, cell));
                }
            }
        }
    }

    private static bool SweepX(TileMap map, Vector2 mapPos, ref Vector2 box, Vector2 size, float dx)
    {
        float tw = map.TileSize.X;
        var rows = CellRange(box.Y, size.Y, mapPos.Y, map.TileSize.Y);
 
        if (dx > 0f)
        {
            float right = box.X + size.X;
            int from = FloorCell(right - dx - mapPos.X, tw);
            int to = LastCell(right - mapPos.X, tw);
 
            for (int col = from; col <= to; col++)
            {
                if (TryFindCell(map, (col, col), rows, TileFlags.Solid, out _))
                {
                    box.X = mapPos.X + (col * tw) - size.X;
                    return true;
                }
            }
        }
        else
        {
            int from = LastCell(box.X - dx - mapPos.X, tw);
            int to = FloorCell(box.X - mapPos.X, tw);
 
            for (int col = from; col >= to; col--)
            {
                if (TryFindCell(map, (col, col), rows, TileFlags.Solid, out _))
                {
                    box.X = mapPos.X + ((col + 1) * tw);
                    return true;
                }
            }
        }
 
        return false;
    }

    private static bool SweepY(TileMap map, Vector2 mapPos, ref Vector2 box, Vector2 size, float dy)
    {
        float th = map.TileSize.Y;
        var cols = CellRange(box.X, size.X, mapPos.X, map.TileSize.X);
 
        if (dy > 0f)
        {
            float bottom = box.Y + size.Y;
            float prevBottom = bottom - dy;
            int from = FloorCell(prevBottom - mapPos.Y, th);
            int to = LastCell(bottom - mapPos.Y, th);
 
            for (int row = from; row <= to; row++)
            {
                float rowTop = mapPos.Y + (row * th);
                var mask = prevBottom <= rowTop + OneWayTolerance
                    ? TileFlags.Solid | TileFlags.OneWay
                    : TileFlags.Solid;
 
                if (TryFindCell(map, cols, (row, row), mask, out _))
                {
                    box.Y = rowTop - size.Y;
                    return true;
                }
            }
        }
        else
        {
            int from = LastCell(box.Y - dy - mapPos.Y, th);
            int to = FloorCell(box.Y - mapPos.Y, th);
 
            for (int row = from; row >= to; row--)
            {
                if (TryFindCell(map, cols, (row, row), TileFlags.Solid, out _))
                {
                    box.Y = mapPos.Y + ((row + 1) * th);
                    return true;
                }
            }
        }
 
        return false;
    }

    private static bool TryFindCell(TileMap map, (int Min, int Max) cols, (int Min, int Max) rows, TileFlags mask, out Point cell)
    {
        for (int y = rows.Min; y <= rows.Max; y++)
        {
            for (int x = cols.Min; x <= cols.Max; x++)
            {
                if ((map.GetFlags(x, y) & mask) != 0)
                {
                    cell = new Point(x, y);
                    return true;
                }
            }
        }
 
        cell = Point.Zero;
        return false;
    }

    private static (int Min, int Max) CellRange(float start, float length, float mapStart, float tileSize) =>
        (FloorCell(start - mapStart, tileSize), LastCell(start + length - mapStart, tileSize));
 
    private static int FloorCell(float offset, float tileSize) => (int)MathF.Floor(offset / tileSize);
 
    private static int LastCell(float offset, float tileSize) => (int)MathF.Ceiling(offset / tileSize) - 1;

}