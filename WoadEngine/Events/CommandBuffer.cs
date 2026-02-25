using System;
using System.Collections.Generic;
using WoadEngine.ECS;

namespace WoadEngine.Events;

public sealed class CommandBuffer
{
    private enum Op : byte
    {
        DestroyEntity,
        AddOrSetComponent,
        RemoveComponent
    }

    private readonly struct Cmd
    {
        public readonly Op Operation;
        public readonly Entity Entity;
        public readonly Type? ComponentType;
        public readonly object? Value;

        public Cmd(Op operation, Entity entity, Type? componentType = null, object? value = null)
        {
            Operation = operation;
            Entity = entity;
            ComponentType = componentType;
            Value = value;
        }
    }

    private readonly List<Cmd> _cmds = new (256);

    private readonly HashSet<int> _pendingDestroyIds = new();

    public void Clear()
    {
        _cmds.Clear();
        _pendingDestroyIds.Clear();
    }

    public void Destroy(Entity e)
    {
        if (_pendingDestroyIds.Add(e.ID))
        {
            _cmds.Add(new Cmd(Op.DestroyEntity, e));
        }
    }

    public void Remove<T>(Entity e) where T : struct
        => _cmds.Add(new Cmd(Op.RemoveComponent, e, typeof(T)));

    public void Add<T>(Entity e, in T component) where T : struct
        => _cmds.Add(new Cmd(Op.AddOrSetComponent, e, typeof(T), component));

    public void Set<T>(Entity e, in T component) where T : struct
        => _cmds.Add(new Cmd(Op.AddOrSetComponent, e, typeof(T), component));

    public void Flush(World world)
    {
        for (int i = 0; i < _cmds.Count; i++)
        {
            var c = _cmds[i];

            if (_pendingDestroyIds.Contains(c.Entity.ID) && c.Operation != Op.DestroyEntity)
                continue;

            switch (c.Operation)
            {
                case Op.DestroyEntity:
                    world.DestroyEntity(c.Entity);
                    break;
                case Op.RemoveComponent:
                    if (world.IsAlive(c.Entity))
                    {
                        var store = world.GetOrCreateStore(c.ComponentType!);
                        store.RemoveEntity(c.Entity.ID);
                    }
                    break;
                case Op.AddOrSetComponent:
                    if (world.IsAlive(c.Entity))
                    {
                        var store = world.GetOrCreateStore(c.ComponentType!);
                        store.SetBoxed(c.Entity.ID, c.Value!);
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Unknown command op: {c.Operation}");
            }
        }

        Clear();
    }

}