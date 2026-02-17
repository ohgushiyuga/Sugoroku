using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject startTilePrefab;
    public GameObject goalTilePrefab;

    [Header("Settings")]
    public int chunkLength = 10;
    [SerializeField] private DecorationSpawner decorationSpawner;

    [Header("UI Elements")]
    public Button mainDiceButton;

    [SerializeField, Header("Debug Info")]
    private string currentBossName;

    private List<GameObject> spawnedTiles = new List<GameObject>();
    private Chunk generatedChunk;

    void Awake()
    {
        if (mainDiceButton != null)
        {
            mainDiceButton.interactable = false;
        }
        Generate();
    }

    public Chunk GetGeneratedChunk()
    {
        return generatedChunk;
    }

    public void Generate()
    {
        foreach (GameObject tile in spawnedTiles) Destroy(tile);
        spawnedTiles.Clear();

        ChunkLayoutGenerator layoutGenerator = new ChunkLayoutGenerator();
        ChunkEventAssigner eventAssigner = new ChunkEventAssigner();
        var layout = layoutGenerator.GenerateLayout(chunkLength);

        if (layout.allTiles == null || layout.allTiles.Count == 0)
        {
            Debug.LogError("レイアウトの生成に失敗しました。");
            return;
        }

        generatedChunk = new Chunk(layout.allTiles, layout.mainPath, layout.startTile, layout.goalTile);
        eventAssigner.AssignEvents(generatedChunk);
        currentBossName = generatedChunk.BossEvent.ToString();

        foreach (var tileData in generatedChunk.AllTiles)
        {
            GameObject prefab = tilePrefab;
            if (tileData == generatedChunk.StartTile) prefab = startTilePrefab;
            if (tileData == generatedChunk.GoalTile) prefab = goalTilePrefab;

            Vector3 position = new Vector3(tileData.GridPosition.x, 0, tileData.GridPosition.z);
            GameObject tileObj = Instantiate(prefab, position, Quaternion.identity, this.transform);

            TileView tileView = tileObj.GetComponent<TileView>();
            if (tileView != null)
            {
                tileView.Initialize(tileData);
            }
            spawnedTiles.Add(tileObj);
        }

         if (decorationSpawner != null)
        {
            decorationSpawner.SpawnDecorations(generatedChunk.AllTiles.ToList());
        }

        if (mainDiceButton != null)
        {
            mainDiceButton.interactable = true;
            Debug.Log("マップ準備完了。「サイコロ」ボタンを有効化しました。");
        }
        else
        {
            Debug.LogWarning("ChunkGeneratorのインスペクターで、Main Dice Buttonが設定されていません！");
        }

        GameState.isMapReady = true;
    }
    
   /// NPCのルートとなる、メインパスの「座標」の配列を返す
    public Vector3[] GetWaypointPositions()
    {
        if (generatedChunk == null || generatedChunk.MainPath == null)
        {
            return new Vector3[0]; // 空の配列を返す
        }

        // MainPathの各タイルのGridPositionを、ワールド座標(Vector3)に変換して配列にする
        return generatedChunk.MainPath
            .Select(tile => new Vector3(tile.GridPosition.x, 0, tile.GridPosition.z))
            .ToArray();
    }
}