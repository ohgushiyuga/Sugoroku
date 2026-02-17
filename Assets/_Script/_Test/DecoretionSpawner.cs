// ファイル名: DecorationSpawner.cs
using UnityEngine;
using System.Collections.Generic; // Listを使うために必要
using System.Linq; // Linqを使うために必要

public class DecorationSpawner : MonoBehaviour
{
    [Header("装飾プレハブ")]
    [SerializeField] private GameObject[] decorationPrefabs; // 複数の装飾プレハブをインスペクターで設定

    [Header("生成設定")]
    [SerializeField] private int distanceFromTile = 2; // マスからの距離
    [SerializeField] [Range(0f, 1f)] private float decorationChance = 0.3f; // 装飾が生成される確率

    /// <summary>
    /// ChunkGeneratorから呼び出され、装飾の生成を開始する
    /// </summary>
    /// <param name="generatedTiles">生成された全てのタイルのリスト</param>
    public void SpawnDecorations(List<TileData> generatedTiles)
    {
        // プレハブが設定されていなければ、警告を出して処理を中断
        if (decorationPrefabs == null || decorationPrefabs.Length == 0)
        {
            Debug.LogWarning("装飾プレハブが設定されていません。");
            return;
        }

        // 既にオブジェクトが配置されている座標を記録するためのリスト
        // （タイル自身と、これから生成する装飾の座標を記録し、重複を防ぐ）
        HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();
        foreach (var tile in generatedTiles)
        {
            occupiedPositions.Add(tile.GridPosition);
        }

        // 全てのタイルをループし、その周りに装飾を生成しようと試みる
        foreach (TileData tile in generatedTiles)
        {
            // 上下左右、斜めの8方向をチェック
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    // (0,0)はタイル自身なのでスキップ
                    if (x == 0 && z == 0) continue;

                    // タイルから指定された距離だけ離れた座標を計算
                    Vector3Int targetGridPos = new Vector3Int(
                        tile.GridPosition.x + (x * distanceFromTile),
                        0,
                        tile.GridPosition.z + (z * distanceFromTile)
                    );

                    // その場所に既に何か（タイルや他の装飾）がなければ、装飾を生成する
                    if (!occupiedPositions.Contains(targetGridPos))
                    {
                        // 指定された確率で装飾を生成するかどうかを決める
                        if (Random.value < decorationChance)
                        {
                            // どの装飾プレハブを使うかランダムに選ぶ
                            GameObject prefabToSpawn = decorationPrefabs[Random.Range(0, decorationPrefabs.Length)];
                            
                            // ワールド座標に変換
                            Vector3 worldPos = new Vector3(targetGridPos.x, 0, targetGridPos.z);
                            
                            // 装飾を生成
                            Instantiate(prefabToSpawn, worldPos, Quaternion.identity, this.transform);
                        }
                        
                        // 一度チェックした場所として記録し、同じ場所に重複して生成されるのを防ぐ
                        occupiedPositions.Add(targetGridPos);
                    }
                }
            }
        }
    }
}