using Microsoft.Xna.Framework;

namespace WoadEngine.Tiles;

public readonly record struct TileHitEvent(int EntityId, Vector2 Normal);

public readonly record struct TileDamageEvent(int EntityId, Point Cell);