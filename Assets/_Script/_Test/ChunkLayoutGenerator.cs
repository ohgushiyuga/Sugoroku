using System.Collections.Generic;
using UnityEngine;

public class ChunkLayoutGenerator
{
    // ▼▼▼ ここの戻り値（返却する情報のセット）に「List<TileData> mainPath」を追加 ▼▼▼
    public (List<TileData> allTiles, List<TileData> mainPath, TileData startTile, TileData goalTile) GenerateLayout(int mainPathLength)
    {
        if (mainPathLength < 3)
        {
            // 戻り値にも new List<TileData>() を追加
            return (new List<TileData>(), new List<TileData>(), null, null);
        }

        List<TileData> allTiles = new List<TileData>();
        List<TileData> mainPath = new List<TileData>();
        Vector2Int currentPos = Vector2Int.zero;

        // ステップ1: メインルートを生成
        for (int i = 0; i < mainPathLength; i++)
        {
            TileData tile = new TileData(new Vector3Int(currentPos.x, 0, currentPos.y));
            allTiles.Add(tile);
            mainPath.Add(tile);
            if (i > 0)
            {
                mainPath[i - 1].NextTiles.Add(tile);
            }
            currentPos += (Random.value < 0.5f) ? Vector2Int.right : Vector2Int.up;
        }

        // ステップ2: 分岐を追加
        for (int i = 0; i < mainPath.Count - 2; i++)
        {
            if (Random.value < 0.5f)
            {
                TileData fromTile = mainPath[i];
                TileData rejoinTile = mainPath[i + 1];
                Vector2Int fromPos = new Vector2Int(fromTile.GridPosition.x, fromTile.GridPosition.z);
                CreateBranch(allTiles, fromTile, rejoinTile, fromPos);
            }
        }
        
        // ▼▼▼ ここのreturnの2番目に「mainPath」を追加 ▼▼▼
        return (allTiles, mainPath, mainPath[0], mainPath[mainPath.Count - 1]);
    }

    private void CreateBranch(List<TileData> allTiles, TileData from, TileData rejoin, Vector2Int fromPos)
    {
        Vector2Int branchDir = (Random.value < 0.5f) ? Vector2Int.left : Vector2Int.down;
        Vector2Int branchPos = fromPos + branchDir * Random.Range(1, 3);
        
        if (allTiles.Exists(t => t.GridPosition == new Vector3Int(branchPos.x, 0, branchPos.y)))
        {
            return;
        }

        TileData branchStart = new TileData(new Vector3Int(branchPos.x, 0, branchPos.y));
        allTiles.Add(branchStart);
        from.NextTiles.Add(branchStart);
        branchStart.NextTiles.Add(rejoin);
    }
}