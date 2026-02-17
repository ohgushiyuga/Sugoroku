// ファイル名: TileData.cs
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public Vector3Int GridPosition { get; }
    public List<TileData> NextTiles { get; } = new List<TileData>();
    public TileEventType EventType { get; set; }
    public BossEventType? BossEvent { get; set; } = null;

    public TileData(Vector3Int gridPosition)
    {
        this.GridPosition = gridPosition;
    }
}