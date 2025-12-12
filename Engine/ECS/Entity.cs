using System;
using System.Collections.Generic;

namespace WoadEngine.ECS
{
    public sealed class Entity
    {
        private static int _nextId = 1;
        public int Id { get; }

        private readonly Dictionary<Type, IComponent> _components = new();

        public Entity()
        {
            Id = _nextId++;
        }

        public T AddComponent<T>(T component) where T : class, IComponent
        {
            _components[typeof(T)] = component;
            return component;
        }

        public bool TryGetComponent<T>(out T? component) where T : class, IComponent
        {
            if (_components.TryGetValue(typeof(T), out var c))
            {
                component = (T)c;
                return true;
            }

            component = null;
            return false;
        }

        public bool HasComponent<T>() where T : class, IComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        public bool RemoveComponent<T>() where T : class, IComponent
        {
            return _components.Remove(typeof(T));
        }
    }
}