using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3Int startPosition = new Vector3Int(0, 0, 0);

    void Start()
    {
        SpawnPlayerAndSetup();
    }

    void SpawnPlayerAndSetup()
    {
        if (playerPrefab == null) { Debug.LogError("Spawner Error: Player Prefabが設定されていません！"); return; }

        Vector3 worldPos = new Vector3(startPosition.x, 1f, startPosition.z);
        GameObject playerInstance = Instantiate(playerPrefab, worldPos, Quaternion.identity);
        PlayerState playerState = playerInstance.GetComponent<PlayerState>();
        if (playerState == null) { Debug.LogError("Spawner Error: プレイヤーのプレハブにPlayerStateがアタッチされていません！"); return; }

        // 非表示のオブジェクトも含めてCardHandControllerを探す
        CardHandController cardHandController = FindObjectOfType<CardHandController>(true); 
        if (cardHandController != null)
        {
            // CardHandControllerに、どのプレイヤーを監視すればよいか教える
            cardHandController.Setup(playerState);
            Debug.Log("--- PlayerSpawner: CardHandControllerにPlayerStateを正常に設定しました。 ---");
        }
        else
        {
            Debug.LogError("--- PlayerSpawner Error: シーンにCardHandControllerが見つかりません！手札は表示されません。 ---");
        }
        
        // 他のセットアップ処理...
        ChunkGenerator chunkGenerator = FindObjectOfType<ChunkGenerator>();
        if (chunkGenerator != null)
        {
            PlayerMovementController playerController = playerInstance.GetComponent<PlayerMovementController>();
            if (playerController != null)
            {
                playerController.SetChunkData(chunkGenerator.GetGeneratedChunk());
            }
        }
    }
}