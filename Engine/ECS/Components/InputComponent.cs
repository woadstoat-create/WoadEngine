using Microsoft.Xna.Framework.Input;

namespace WoadEngine.ECS
{
    public sealed class InputComponent : IComponent
    {
        public Keys UpKey = Keys.W;
        public Keys DownKey = Keys.S;
        public Keys LeftKey = Keys.A;
        public Keys RightKey = Keys.D;

        // How fast this entity moves in units per second
        public float MoveSpeed = 200f;

        // Optional: lock movement to vertical or horizontal only
        public bool VerticalOnly;
        public bool HorizontalOnly;
    }
}