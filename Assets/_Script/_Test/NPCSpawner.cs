// ファイル名: NpcSpawner.cs
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
   [Header("NPCの設定")]
    // ★修正: npcPrefabをGameObjectの配列に変更
   [SerializeField] private GameObject[] npcPrefabs;
   [SerializeField] private Transform spawnPoint; // NPCを生成する場所

   [Header("生成する高さのオフセット")]
   [Tooltip("マスの表面からどれだけ浮かせて生成するか")]
[SerializeField] private float spawnHeightOffset = 0.5f;

    /// NPCを生成し、そのコントローラーを返す
    /// <returns>生成されたNPCのRandomNpcController</returns>
    public RandomNpcController SpawnNpc()
    {
        // ★修正: 配列が設定されていない場合のチェック
        if (npcPrefabs == null || npcPrefabs.Length == 0 || spawnPoint == null)
        {
            Debug.LogError("NpcSpawnerにプレハブまたは生成場所が設定されていません！");
            return null;
        }

        // ★追加: 配列からランダムにプレハブを選択
        int randomIndex = Random.Range(0, npcPrefabs.Length);
        GameObject npcPrefab = npcPrefabs[randomIndex];

        // 元々の生成場所の真上に、オフセット分だけ高い位置を計算
        Vector3 spawnPosition = spawnPoint.position + Vector3.up * spawnHeightOffset;
        
        // 選択したプレハブを、計算した位置と角度でインスタンス化（生成）する
         GameObject npcInstance = Instantiate(npcPrefab, spawnPosition, spawnPoint.rotation);

         RandomNpcController npcController = npcInstance.GetComponent<RandomNpcController>();

         if (npcController != null)
        {
            Debug.Log($"NPC「{npcInstance.name}」をSpawnPointに生成しました。");
            return npcController;
        }
        else
        {
           Debug.LogError($"'{npcPrefab.name}' プレハブにRandomNpcControllerコンポーネントがありません！");
          return null;
        }
    }
}