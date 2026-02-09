using Microsoft.Xna.Framework;

namespace WoadEngine.ECS.Components;

public struct Velocity3D
{
    public Vector3 Value;
    public Velocity3D(Vector3 v) => Value = v;
}