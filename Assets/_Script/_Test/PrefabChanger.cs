using UnityEngine;
using UnityEngine.UI;
public class PrefabChanger : MonoBehaviour
{
    public GameObject currentPrefabInstance; // 今表示中のプレハブインスタンス
    public GameObject newPrefab;             // 次に表示するプレハブ（プレハブアセット）
    public Button actionButton;              // 押すボタン
    public Transform spawnParent;            // 新しいプレハブを配置する親（任意）
    void Start()
    {
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(ReplacePrefab);
        }
    }
    void ReplacePrefab()
    {
        // 現在のプレハブを削除
        if (currentPrefabInstance != null)
        {
            Destroy(currentPrefabInstance);
        }
        // 新しいプレハブを生成・表示
        if (newPrefab != null)
        {
            Vector3 spawnPosition = Vector3.zero; // 必要なら位置調整
            Quaternion spawnRotation = Quaternion.identity;
            // 親のTransformがある場合はそれにぶら下げる
            currentPrefabInstance = Instantiate(newPrefab, spawnPosition, spawnRotation, spawnParent);
        }
    }
}
