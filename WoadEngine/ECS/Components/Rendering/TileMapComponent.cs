using WoadEngine.Tiles;

namespace WoadEngine.ECS.Components.Rendering;

public struct TileMapComponent
{
    public TileMap Map;

    public static TileMapComponent Create(TileMap map)
    {
        return new TileMapComponent
        {
            Map = map
        };
    }
}