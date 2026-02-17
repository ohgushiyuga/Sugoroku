// ファイル名: TileView.cs

using UnityEngine;
using TMPro;

public class TileView : MonoBehaviour
{
    public enum TileVisualType { Normal, Start, Goal }
    private GameObject spawnedDecoration; 

    [Header("Tile Settings")]
    public TileVisualType tileType = TileVisualType.Normal; // Inspectorでタイルの種類を設定

    [Header("Debug Info")]
    [SerializeField] private TileEventType currentEvent;
    [SerializeField] private TextMeshPro eventText;

    public TileData Data { get; private set; }
    private Renderer tileRenderer;

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
    }

    public void Initialize(TileData data)
    {
        this.Data = data;
        this.currentEvent = data.EventType;
        UpdateAppearance();
        SpawnDecoration();

        if (eventText != null)
        {
            if (data.BossEvent.HasValue)
            {
                eventText.text = data.BossEvent.Value.ToString();
                eventText.color = Color.white;
            }
            else
            {
                eventText.text = data.EventType == TileEventType.None ? "" : data.EventType.ToString();
                eventText.color = Color.black;
            }
        }
    }
    
    private void SpawnDecoration()
    {
        // 既にある装飾は一度削除する
        if (spawnedDecoration != null)
        {
            Destroy(spawnedDecoration);
        }

        string prefabName = ""; // 読み込むプレハブの名前


        // 自分のマスの種類(EventType)によって、読み込むプレハブ名を変える
        switch (Data.EventType)
        {
            case TileEventType.Card:
                prefabName = "Card_Decoration"; // Resourcesフォルダのプレハブ名
                break;
            case TileEventType.Nuts:
                prefabName = "Nuts_Decoration";
                break;
            case TileEventType.Save:
                prefabName = "Save_Decoration";
                break;
                // 他のマスタイプも同様に追加...
        }

        // 読み込むべきプレハブ名が設定されていた場合のみ、生成処理を行う
        if (!string.IsNullOrEmpty(prefabName))
        {
            GameObject prefab = Resources.Load<GameObject>(prefabName);
            if (prefab != null)
            {
                // 1. 生成する座標を計算するための新しい変数を用意する
                Vector3 spawnPosition = transform.position;


                // 2. その変数のY座標だけを1加算する
                spawnPosition.y += 1f;


                // 3. 計算した新しい座標を使って、装飾を生成する
                spawnedDecoration = Instantiate(prefab, spawnPosition, Quaternion.identity, this.transform);

            }
            else
            {
                Debug.LogWarning(prefabName + " という名前のプレハブがResourcesフォルダに見つかりません。");
            }
        }
    }

    /// タイルの見た目を更新する（色分けロジックをここに集約）
    public void UpdateAppearance()
    {
        if (Data == null || tileRenderer == null) return;

        Color color = Color.black; // デフォルト色

        switch (tileType)
        {
            // スタートマスの場合
            case TileVisualType.Start:
                color = new Color(0.1f, 0.9f, 0.2f); // 明るいライムグリーン
                break;

            // ゴールマスの場合
            case TileVisualType.Goal:
                color = GetBossColor(Data.BossEvent.GetValueOrDefault()); // ボス色
                break;

            // 通常マスの場合
            case TileVisualType.Normal:
            default:
                color = GetEventColor(Data.EventType); // イベント色
                break;
        }
        tileRenderer.material.color = color;
    }

    private Color GetEventColor(TileEventType type)
    {
        switch (type)
        {
            case TileEventType.None: return Color.white;
            case TileEventType.Card: return new Color(0.627f, 0.878f, 0.824f);
            case TileEventType.Boar: return new Color(1f, 0.55f, 0f);
            case TileEventType.BoarStop: return new Color(0.3f, 0.3f, 0.3f);
            case TileEventType.Save: return new Color(0.6f, 0.8f, 1f);
            case TileEventType.Nuts: return new Color(0.3f, 0.7f, 0.3f);
            case TileEventType.River: return new Color(0.4f, 0.6f, 1f);
            case TileEventType.HawkStop: return new Color(0.545f, 0.271f, 0.075f);
            default: return Color.black;
        }
    }

    private Color GetBossColor(BossEventType boss)
    {
        return Color.red; // ゴールマスの色は赤
    }
}