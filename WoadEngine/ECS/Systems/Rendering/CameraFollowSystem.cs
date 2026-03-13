using Microsoft.Xna.Framework;
using WoadEngine.ECS;
using WoadEngine.ECS.Components.Physics;

namespace WoadEngine.ECS.Systems.Rendering;

public sealed class CameraFollowSystem : ISystem
{
    public void Update(World world, float dt)
    {
        var transforms = world.GetStore<Transform>();
        var follows = world.GetStore<FollowTarget>();

        var entities = follows.DenseEntities;
        for (int i = 0; i < entities.Length; i++)
        {
            int cameraEntity = entities[i];

            if (!transforms.Has(cameraEntity))
                continue;

            ref var follow = ref follows.DenseComponents[i];

            if (!transforms.Has(follow.TargetEntity))
                continue;

            ref var cameraTransform = ref transforms.Get(cameraEntity);
            ref var targetTransform = ref transforms.Get(follow.TargetEntity);

            Vector3 desiredPosition = new Vector3(
                targetTransform.Position.X + follow.Offset.X,
                targetTransform.Position.Y + follow.Offset.Y,
                cameraTransform.Position.Z);

            if (follow.Snap)
            {
                cameraTransform.Position = desiredPosition;
            }
            else
            {
                cameraTransform.Position = Vector3.Lerp(
                    cameraTransform.Position,
                    desiredPosition,
                    10f * dt);
            }
        }
    }
}