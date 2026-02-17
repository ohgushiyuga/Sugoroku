using System.Collections.Generic;

public class Chunk
{
    public IReadOnlyList<TileData> AllTiles => allTiles;
    private List<TileData> allTiles;

    // メインルートの情報を保持する場所
    public IReadOnlyList<TileData> MainPath { get; }

    public TileData StartTile { get; private set; }
    public TileData GoalTile { get; private set; }
    public BossEventType BossEvent { get; set; }

    // ▼▼▼ コンストラクタが4つの引数を受け取れるように修正 ▼▼▼
    public Chunk(List<TileData> tiles, List<TileData> mainPath, TileData start, TileData goal)
    {
        this.allTiles = tiles;
        this.MainPath = mainPath; // 受け取ったmainPathを保存
        this.StartTile = start;
        this.GoalTile = goal;
    }
}