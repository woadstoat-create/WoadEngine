using Microsoft.Xna.Framework;

namespace WoadEngine.ECS.Components;

public struct Transform3D
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;

    public Transform3D(Vector3 position)
    {
        Position = position;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
    }
}