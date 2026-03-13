using Microsoft.Xna.Framework;

namespace WoadEngine.ECS.Components.Rendering;

public struct Camera2D
{
    public bool IsActive;
    public float Zoom;
    public float Rotation;
    public Vector2 Origin;
    public Matrix View;

    public static Camera2D Create(bool active = true, float zoom = 1f)
    {
        return new Camera2D
        {
            IsActive = active,
            Zoom = zoom,
            Rotation = 0f,
            Origin = Vector2.Zero,
            View = Matrix.Identity
        };
    }
}