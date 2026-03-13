using Microsoft.Xna.Framework;

namespace WoadEngine.ECS.Components.Gameplay;

public struct FollowTarget
{
    public int TargetEntity;
    public Vector2 Offset;
    public bool Snap;
}