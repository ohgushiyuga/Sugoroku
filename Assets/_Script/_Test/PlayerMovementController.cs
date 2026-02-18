using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// プレイヤーの移動、残り歩数の管理、移動先の矢印UI表示など、
/// プレイヤーの移動フェーズに関する全ての処理を管理する司令塔。
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private GameObject arrowButtonPrefab;

    // --- 内部コンポーネント ---
    private GameObject canvasObject;
    private Rigidbody playerRigidbody;
    private EventManager eventManager;
    private GameManager gameManager;

    // --- 状態管理 ---
    private int remainingSteps;
    private List<GameObject> spawnedArrows = new List<GameObject>();
    private Chunk currentChunk;
    private TileData currentTile;
    private readonly List<TileData> pathHistory = new List<TileData>();

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        eventManager = FindObjectOfType<EventManager>();
        gameManager = FindObjectOfType<GameManager>();

        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            canvasObject = uiManager.MovementArrowCanvas;
        }
    }

    // --- 外部からの命令を受け取るメソッド ---

    public void SetChunkData(Chunk chunk)
    {
        this.currentChunk = chunk;
        if (playerRigidbody != null)
        {
            this.currentTile = GetTileAt(playerRigidbody.position);
            // ★履歴をリセットし、スタート地点を追加
            pathHistory.Clear();
            if (this.currentTile != null)
            {
                pathHistory.Add(this.currentTile);
            }
        }
    }

    public void StartMovePhase(int steps)
    {
        remainingSteps = steps;
        ShowMoveOptions();
    }

    public void OnArrowClicked(Vector3 direction)
    {
        if (remainingSteps <= 0) return;
        StartCoroutine(MoveStepByStep(direction));
    }

    /// <summary>
    /// EventManagerから呼ばれ、指定されたマス数だけ後退する
    /// </summary>
    public void ForceMoveBack(int steps)
    {
        StartCoroutine(MoveBackCoroutine(steps));
    }

    /// <summary>
/// GameManagerから呼ばれ、ボス戦失敗時に1マス前のタイルに戻る
/// </summary>
public IEnumerator ExecuteForcedMove()
{
    if (pathHistory.Count < 2)
    {
        Debug.LogError("戻るべき前のマスの履歴がありません。");
        yield break; // 後退できない場合はコルーチンを終了
    }

    // 履歴の最後から2番目＝1つ前のタイルを取得
    TileData targetTile = pathHistory[pathHistory.Count - 2];
    Vector3 targetPosition = new Vector3(targetTile.GridPosition.x, 1f, targetTile.GridPosition.z);
    
    // 最新の履歴を削除
    pathHistory.RemoveAt(pathHistory.Count - 1);
    
    // 1マス前のタイルに戻るコルーチンを開始し、完了を待機
    yield return StartCoroutine(MoveOnceAndEndTurn(targetPosition));

    // ★重要: ここでのターン終了呼び出しを削除
}

private IEnumerator MoveOnceAndEndTurn(Vector3 targetPosition)
{
    HideArrows();
    if (playerRigidbody != null) { playerRigidbody.MovePosition(targetPosition); }
    yield return new WaitForFixedUpdate();
    currentTile = GetTileAt(playerRigidbody.position);
}


    // --- メインの処理（コルーチン） ---

    private IEnumerator MoveStepByStep(Vector3 direction)
    {
        remainingSteps--;
        foreach (var arrow in spawnedArrows) Destroy(arrow);
        spawnedArrows.Clear();

        // 移動前に現在地を記憶する必要はもうない（履歴があるため）
        if (playerRigidbody != null)
        {
            playerRigidbody.MovePosition(playerRigidbody.position + direction);
        }
        yield return new WaitForFixedUpdate();

        currentTile = GetTileAt(playerRigidbody.position);
        
        // ★移動した新しいタイルを履歴に追加
        if(currentTile != null) pathHistory.Add(currentTile);

        if (currentTile == currentChunk.GoalTile)
        {
            remainingSteps = 0;
        }
        
        ShowMoveOptions();
    }

    private IEnumerator MoveBackCoroutine(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            if (pathHistory.Count < 2) break;

            TileData targetTile = pathHistory[pathHistory.Count - 2];
            Vector3 targetPosition = new Vector3(targetTile.GridPosition.x, 1f, targetTile.GridPosition.z);
            
            if (playerRigidbody != null) { playerRigidbody.MovePosition(targetPosition); }
            yield return new WaitForFixedUpdate();

            pathHistory.RemoveAt(pathHistory.Count - 1);
        }

        currentTile = GetTileAt(playerRigidbody.position);
        gameManager.EndPlayerTurn();
    }

    // --- UIとイベント処理 ---

    private void ShowMoveOptions()
    {
        if (remainingSteps <= 0)
        {
            HandleMoveEnd();
            return;
        }

        if (currentTile == null || canvasObject == null || arrowButtonPrefab == null) return;
        if (currentTile.NextTiles.Count == 0) return;

        foreach (TileData nextTile in currentTile.NextTiles)
        {
            Vector3 moveDirection = new Vector3(
                nextTile.GridPosition.x - currentTile.GridPosition.x, 0,
                nextTile.GridPosition.z - currentTile.GridPosition.z
            );
            GameObject arrowObj = Instantiate(arrowButtonPrefab, canvasObject.transform);

            Vector3 nextTileWorldPos = new Vector3(nextTile.GridPosition.x, 0, nextTile.GridPosition.z);
            Vector3 arrowWorldPos = Vector3.Lerp(playerRigidbody.position, nextTileWorldPos, 0.5f);
            arrowObj.transform.position = Camera.main.WorldToScreenPoint(arrowWorldPos);
            float angle = Mathf.Atan2(moveDirection.z, moveDirection.x) * Mathf.Rad2Deg;
            arrowObj.transform.eulerAngles = new Vector3(0, 0, angle - 90f);
            var arrowButton = arrowObj.GetComponent<ArrowButton>();
            if (arrowButton != null) { arrowButton.Initialize(this, moveDirection); }
            spawnedArrows.Add(arrowObj);
        }
    }

    private void HandleMoveEnd()
    {
        // ★追加: ゲームの状態が「プレイ中」でなければ、何もしない
        if (gameManager.CurrentPhase != GameManager.GamePhase.Playing)
        {
            HideArrows();
            return;
        }

        HideArrows();
        Debug.Log("移動終了！");

        if (gameManager != null && currentTile != null)
        {
            if (currentTile.BossEvent.HasValue)
            {
                // ボスイベントがある場合
                gameManager.StartBossEvent(currentTile.BossEvent.Value);
            }
            else
            {
                // 通常のイベントがある場合
                eventManager.TriggerEvent(currentTile.EventType);
            }
        }
        // イベントがないか、またはイベント処理が完了した場合にターンを終了
        else if (gameManager != null)
        {
            gameManager.EndPlayerTurn();
        }
    }

    private void HideArrows()
    {
        foreach (var arrow in spawnedArrows) Destroy(arrow);
        spawnedArrows.Clear();
    }

    public int GetCurrentWaypointIndex()
    {
        // ウェイポイントのインデックスを返すロジックを実装
        // このメソッドはプレイヤーが現在どのマスにいるかを示すインデックスを返す
        // あなたのコードの構造に合わせて実装してください
        return 0; // 仮の値
    }

    // --- ヘルパーメソッド ---
    public TileData GetCurrentTile() { return this.currentTile; }

    private TileData GetTileAt(Vector3 worldPosition)
    {
        Vector3Int gridPos = new Vector3Int(Mathf.RoundToInt(worldPosition.x), 0, Mathf.RoundToInt(worldPosition.z));
        if (currentChunk == null) return null;
        return currentChunk.AllTiles.FirstOrDefault(tile => tile.GridPosition == gridPos);
    }
}