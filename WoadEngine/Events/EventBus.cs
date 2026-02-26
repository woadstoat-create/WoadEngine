using System;
using System.Collections.Generic;

namespace WoadEngine.Events;

public sealed class EventBus
{
    private interface IHandlerList
    {
        void Dispatch(object evt);
    }

    private sealed class HandlerList<T> : IHandlerList
    {
        public readonly List<Action<T>> Handlers = new(4);

        public void Dispatch(object evt)
        {
            var e = (T)evt;
            for (int i = 0; i < Handlers.Count; i++)
                Handlers[i](e);
        }    
    }

    private readonly Dictionary<Type, IHandlerList> _handlers = new();
    private readonly List<object> _queue = new(256);

    public void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);

        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new HandlerList<T>();
            _handlers[type] = list;
        }
        ((HandlerList<T>)list).Handlers.Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var list))
        {
            ((HandlerList<T>)list).Handlers.Remove(handler);
        }
    }

    public void Publish<T>(T evt) => _queue.Add(evt!);

    public void Flush()
    {
        for (int i = 0; i < _queue.Count; i++)
        {
            var evt = _queue[i];
            var type = evt.GetType();

            if (_handlers.TryGetValue(type, out var list))
                list.Dispatch(evt);
        }

        _queue.Clear();
    }

}