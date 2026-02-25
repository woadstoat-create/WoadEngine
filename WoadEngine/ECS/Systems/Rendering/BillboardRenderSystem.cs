

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WoadEngine.ECS.Components.Physics;
using WoadEngine.ECS.Components.Rendering;

namespace WoadEngine.ECS.Systems.Rendering;

public sealed class BillboardRenderSystem : IRenderSystem
{
    private readonly BasicEffect _fx;
    private BillboardItem[] _items = Array.Empty<BillboardItem>();

    private VertexPositionTexture[] _verts = Array.Empty<VertexPositionTexture>();
    private short[] _indices = Array.Empty<short>();

    public BillboardRenderSystem()
    {
        _fx = new BasicEffect(Core.GraphicsDevice)
        {
            TextureEnabled = true,
            VertexColorEnabled = false,
            LightingEnabled = false
        };
    }

    public void Draw(World world, float dt)
    {
        if (!world.TryGetActiveCamera(out int camId, out Camera cam, out Transform camTr))
            return;

        _fx.View = cam.View;
        _fx.Projection = cam.Projection;

        var transforms = world.GetStore<Transform>();
        var billboards = world.GetStore<BillboardSprite>();

        int count = billboards.Count;
        EnsureCapacity(count);

        int v = 0;
        int i = 0;

        var items = GetItemsSpan(count);
        int itemCount = 0;

        for (int di = 0; di < items.Length; di++)
        {
            int entityId = billboards.DenseEntities[di];
            ref var sprite = ref billboards.DenseComponents[di];
        }

        items = items.Slice(0, itemCount);
        items.Sort((a, b) => b.SortKey.CompareTo(a.SortKey));

        Core.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        Core.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
        Core.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        Core.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

        foreach (ref readonly var item in items)
        {
            EmitBillboardQuad(item.Transform, item.Sprite, camTr.Position, Vector3.Up, ref v, ref i);
        }

        if (v == 0)
            return;

        DrawGroupedByTexture(items, v, i, cam, camTr);
        

    }

    private Span<BillboardItem> GetItemsSpan(int needed)
    {
        if (_items.Length < needed)
            _items = new BillboardItem[Math.Max(needed, _items.Length * 2 + 16)];
        return _items.AsSpan(0, needed);
    }

    private void DrawGroupedByTexture(Span<BillboardItem> items, int usedVerts, int usedIndices, Camera cam, Transform camPos)
    {
        for (int idx = 0; idx < items.Length; idx++)
        {
            var item = items[idx];

            // Build just one quad in first 4 verts and 6 indices
            EnsureCapacity(1);
            int v = 0, i = 0;
            EmitBillboardQuad(item.Transform, item.Sprite, camPos.Position, Vector3.Up, ref v, ref i);

            _fx.Texture = item.Sprite.Texture;
            _fx.DiffuseColor = item.Sprite.Tint.ToVector3();
            _fx.Alpha = MathHelper.Clamp(item.Sprite.Alpha, 0f, 1f);

            foreach (var pass in _fx.CurrentTechnique.Passes)
            {
                pass.Apply();
                Core.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    _verts, 0, 4,
                    _indices, 0, 2
                );
            }
        }
    }


    private void EmitBillboardQuad(in Transform tr, in BillboardSprite bb, Vector3 camPos, Vector3 worldUp, ref int v, ref int i)
    {
        Vector3 right, up;

        if (bb.Mode == BillboardMode.Cylindrical)
        {
            var toCam = camPos - tr.Position;
            toCam.Y = 0f;
            if (toCam.LengthSquared() < 1e-6f)
                toCam = Vector3.Forward;

            var forward = Vector3.Normalize(toCam);
            right = Vector3.Normalize(Vector3.Cross(worldUp, forward));
            up = worldUp;
        }
        else
        {
            Matrix invView = Matrix.Invert(GetActiveViewMatrix());
            right = new Vector3(invView.M11, invView.M12, invView.M13);
            up    = new Vector3(invView.M21, invView.M22, invView.M23);
        }

        float halfW = bb.Size.X * 0.5f * tr.Scale.X;
        float halfH = bb.Size.Y * 0.5f * tr.Scale.Y;

        Vector3 p0 = tr.Position + (-right * halfW +  up * halfH);
        Vector3 p1 = tr.Position + ( right * halfW +  up * halfH);
        Vector3 p2 = tr.Position + (-right * halfW + -up * halfH);
        Vector3 p3 = tr.Position + ( right * halfW + -up * halfH);

        // UVs (with optional source rect)
        Vector2 uv0, uv1, uv2, uv3;
        if (bb.SourceRect is Rectangle src)
        {
            float texW = bb.Texture.Width;
            float texH = bb.Texture.Height;

            float u0 = src.X / texW;
            float v0 = src.Y / texH;
            float u1 = (src.X + src.Width) / texW;
            float v1 = (src.Y + src.Height) / texH;

            uv0 = new Vector2(u0, v0);
            uv1 = new Vector2(u1, v0);
            uv2 = new Vector2(u0, v1);
            uv3 = new Vector2(u1, v1);
        }
        else
        {
            uv0 = new Vector2(0, 0);
            uv1 = new Vector2(1, 0);
            uv2 = new Vector2(0, 1);
            uv3 = new Vector2(1, 1);
        }

        // Write vertices
        _verts[0] = new VertexPositionTexture(p0, uv0);
        _verts[1] = new VertexPositionTexture(p1, uv1);
        _verts[2] = new VertexPositionTexture(p2, uv2);
        _verts[3] = new VertexPositionTexture(p3, uv3);

        // Two triangles: 0-1-2, 2-1-3
        _indices[0] = 0; _indices[1] = 1; _indices[2] = 2;
        _indices[3] = 2; _indices[4] = 1; _indices[5] = 3;

        v += 4;
        i += 6;
    }

    private Matrix GetActiveViewMatrix()
    {
        return Matrix.Identity;
    }

    private void EnsureCapacity(int quadCount)
    {
        int neededVerts = quadCount * 4;
        int neededIndices = quadCount * 6;

        if (_verts.Length < neededVerts)
            _verts = new VertexPositionTexture[Math.Max(neededVerts, _verts.Length * 2 + 16)];
        if (_indices.Length < neededIndices)
            _indices = new short[Math.Max(neededIndices, _indices.Length * 2 + 16)];
    }

    private readonly struct BillboardItem
    {
        public readonly int EntityId;
        public readonly float SortKey;
        public readonly Transform Transform;
        public readonly BillboardSprite Sprite;

        public BillboardItem(int entityId, float sortKey, Transform tr, BillboardSprite sprite)
        {
            EntityId = entityId;
            SortKey = sortKey;
            Transform = tr;
            Sprite = sprite;
        }
    }
}