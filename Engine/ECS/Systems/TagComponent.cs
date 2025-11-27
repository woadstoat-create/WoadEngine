
using System;

[Flags]
public enum Tags
{
    None = 0,
    Player = 1 << 0,
    Player2 = 1 << 2,
    Ball = 1 << 3,
    UI = 1 << 4
    
}

namespace WoadEngine.ECS
{
    public sealed class TagComponent : IComponent
    {
        public Tags Tag = Tags.None;
    }
}