using System.Linq;
using UnityEngine;

public class ChunkEventAssigner
{
    public void AssignEvents(Chunk chunk)
    {
        // 1. ボスイベントを決定
        int eventCount = System.Enum.GetValues(typeof(BossEventType)).Length;
        chunk.BossEvent = (BossEventType)Random.Range(0, eventCount);
        if (chunk.GoalTile != null)
        {
            chunk.GoalTile.BossEvent = chunk.BossEvent;
        }

        // 2. 各タイルに通常のイベントを割り当て
        foreach (var tile in chunk.AllTiles.Where(t => t != chunk.GoalTile && t != chunk.StartTile))
        {
            switch (chunk.BossEvent)
            {
                case BossEventType.Cat:
                    TileEventType[] catEvents = { TileEventType.River, TileEventType.None, TileEventType.Card };
                    tile.EventType = catEvents[Random.Range(0, catEvents.Length)];
                    break;
                case BossEventType.hawk:
                    TileEventType[] hawkEvents = { TileEventType.HawkStop, TileEventType.None, TileEventType.Card };
                    tile.EventType = hawkEvents[Random.Range(0, hawkEvents.Length)];
                    break;
                default:
                    tile.EventType = GetRandomNormalEvent();
                    break;
            }
        }

        // 3. マップの中間地点を探して、イベントを「Save」に上書きする
        if (chunk.MainPath != null && chunk.MainPath.Count > 2)
        {
            int middleIndex = chunk.MainPath.Count / 2;
            TileData midpointTile = chunk.MainPath[middleIndex];
            
            if (midpointTile != chunk.GoalTile && midpointTile != chunk.StartTile)
            {
                midpointTile.EventType = TileEventType.Save;
            }
        }

        // 4. スタートマスのイベントを'None'に設定
        if (chunk.StartTile != null)
        {
            chunk.StartTile.EventType = TileEventType.None;
        }
    }

    private TileEventType GetRandomNormalEvent()
    {
        TileEventType[] normalEvents = {
            TileEventType.None,
            TileEventType.None,
            TileEventType.Card,
            TileEventType.Boar,
            TileEventType.Nuts,
        };
        return normalEvents[Random.Range(0, normalEvents.Length)];
    }
}