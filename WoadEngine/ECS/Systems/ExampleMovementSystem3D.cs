using WoadEngine.ECS.Components.Physics;

namespace WoadEngine.ECS.Systems;

public sealed class MovementSystem3D : ISystem
{
    public void Update(World world, float dt)
    {
        var transforms = world.GetStore<Transform>();
        var velocities = world.GetStore<Velocity>();

        if (transforms.Count <= velocities.Count)
        {
            var ents = transforms.DenseEntities;
            for (int i = 0; i < ents.Length; i++)
            {
                int id = ents[i];
                if (!velocities.Has(id)) continue;

                ref var t = ref transforms.DenseComponents[i];
                ref var v = ref velocities.Get(id);

                t.Position += v.Value * dt;
            }
        }
        else
        {
            var etns = velocities.DenseEntities;
            for (int i = 0; i < ents.Length; i++)
            {
                int id = ents[i];
                if (!transforms.Has(id)) continue;

                ref var v = ref velocities.DenseComponents[i];
                ref var t = ref transforms.Get(id);

                t.Position += v.Value * dt;
            }
        }
    }
}