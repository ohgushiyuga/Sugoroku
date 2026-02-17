using System;
using System.Collections;
using UnityEngine;
using System.Linq;

public class RandomNpcController : MonoBehaviour
{
    [Header("NPCのコマ")]
    [SerializeField] private Transform pawn;
    [Header("移動の速さ")]
    [SerializeField] private float moveSpeed = 5f;
    [Header("移動中の高さオフセット")]
    [SerializeField] private float moveHeightOffset = 1f;
    [Header("思考時間（秒）")]
    [SerializeField] private float thinkingTime = 1.5f;


    private NpcState npcState;
    private GameManager gameManager;
    private CardManager cardManager;
    private ChunkGenerator chunkGenerator; // マスの情報を取得するため

    private Vector3[] waypoints;
    private enum NpcStateEnum { Idle, Thinking, Acting }
    private NpcStateEnum currentState = NpcStateEnum.Idle;
    private int currentWaypointIndex = 0;
    public event Action OnTurnEnd;

    private void Start()
    {
        // 参照を取得
        npcState = GetComponent<NpcState>();
        gameManager = FindObjectOfType<GameManager>();
        cardManager = FindObjectOfType<CardManager>();
        chunkGenerator = FindObjectOfType<ChunkGenerator>();

        if (npcState == null || cardManager == null || chunkGenerator == null)
        {
            Debug.LogError("NPCの必須コンポーネントが不足しています！");
        }
    }

    public void StartMyTurn()
    {
        if (currentState == NpcStateEnum.Idle)
        {
            // NPCが止まっているマスの情報を取得
            TileData currentTile = GetCurrentTile();

            // Nutsマスの効果を適用
            if (currentTile != null && currentTile.EventType == TileEventType.Nuts)
            {
                Debug.Log("NPCがNutsマスに止まりました！次の出目が+2されます。");
                if (npcState != null)
                {
                    npcState.rollModifier = 2;
                }
            }

            StartCoroutine(TurnCoroutine());
        }
    }

    public void SetWaypoints(Vector3[] generatedWaypoints)
    {
        this.waypoints = generatedWaypoints;
    }

    private IEnumerator TurnCoroutine()
    {
        currentState = NpcStateEnum.Thinking;
        Debug.Log("--- NPC TurnCoroutine: 開始 ---");
        yield return new WaitForSeconds(thinkingTime);

        currentState = NpcStateEnum.Acting;

        TileData currentTile = GetCurrentTile();
        if (currentTile != null && currentTile.EventType == TileEventType.Nuts)
        {
            npcState.rollModifier = 2;
        }

        // サイコロの生の出目を取得
        int rawResult = UnityEngine.Random.Range(1, 7);

        // CardManagerを使って最終的な出目を計算
        int finalResult = rawResult;
        if (cardManager != null && npcState != null)
        {
            finalResult = cardManager.GetFinalRollValue(npcState, rawResult);
        }

        Debug.Log($"NPC TurnCoroutine: サイコロを振って「{finalResult}」が出ました。");
        yield return new WaitForSeconds(0.5f);

        // ★移動が終わったら、止まったマスのイベントをチェック
        yield return StartCoroutine(MovePawn(finalResult));

        // 止まったマスの情報を取得
        TileData endTile = GetCurrentTile();

        if (endTile != null && endTile.EventType == TileEventType.Boar)
        {
            Debug.Log("NPCがBoarマスに止まりました！2マス後退します。");
            // 後退処理を呼び出し、完了するまで待機
            yield return StartCoroutine(MoveBackCoroutine(2));
        }
        else if (endTile != null && endTile.BossEvent.HasValue)
        {
            Debug.Log($"NPCがボスイベントマスに止まりました！イベント開始: {endTile.BossEvent.Value}");
            // NPC用のボスイベント開始をGameManagerに依頼
            if (gameManager != null)
            {
                gameManager.StartNpcBossEvent(this, endTile.BossEvent.Value);
                yield break; // ボス戦が始まったらターンを終了
            }
        }
        // ★後退イベントが完了したら、ターンを終了する
        currentState = NpcStateEnum.Idle;
        OnTurnEnd?.Invoke();
    }

    private IEnumerator MovePawn(int steps)
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("NPC Error: ルート情報がないため移動できません。");
            yield break;
        }

        for (int i = 0; i < steps; i++)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = waypoints.Length - 1;
                break;
            }

            Vector3 targetPosition = waypoints[currentWaypointIndex] + Vector3.up * moveHeightOffset;

            while (Vector3.Distance(pawn.position, targetPosition) > 0.01f)
            {
                pawn.position = Vector3.MoveTowards(pawn.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            pawn.position = targetPosition;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private TileData GetCurrentTile()
    {
        if (chunkGenerator == null) return null;

        Vector3Int gridPos = new Vector3Int(
        Mathf.RoundToInt(pawn.position.x),
        0,
        Mathf.RoundToInt(pawn.position.z)
        );

        Chunk currentChunk = chunkGenerator.GetGeneratedChunk();
        if (currentChunk == null) return null;

        return currentChunk.AllTiles.FirstOrDefault(t => t.GridPosition == gridPos);
    }

    public IEnumerator ForceMoveBack(int steps)
    {
        Debug.Log($"NPCがイベントで{steps}マス後退します。");

        for (int i = 0; i < steps; i++)
        {
            currentWaypointIndex--;
            if (currentWaypointIndex < 0)
            {
                currentWaypointIndex = 0;
                break;
            }

            Vector3 targetPosition = waypoints[currentWaypointIndex] + Vector3.up * moveHeightOffset;

            while (Vector3.Distance(pawn.position, targetPosition) > 0.01f)
            {
                pawn.position = Vector3.MoveTowards(pawn.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            pawn.position = targetPosition;
            yield return new WaitForSeconds(0.2f);
        }
         currentState = NpcStateEnum.Idle;
        OnTurnEnd?.Invoke();
    }


    public IEnumerator MoveBackCoroutine(int steps)

    {
        Debug.Log($"NPCがイベントで{steps}マス後退します。");

        for (int i = 0; i < steps; i++)
        {
            // ワールド座標の配列なので、インデックスを減らすことで後退する
            currentWaypointIndex--;

            // 始点を通り過ぎないようにチェック
            if (currentWaypointIndex < 0)
            {
                currentWaypointIndex = 0;
                break;
            }

            Vector3 targetPosition = waypoints[currentWaypointIndex] + Vector3.up * moveHeightOffset;

            while (Vector3.Distance(pawn.position, targetPosition) > 0.01f)
            {
                pawn.position = Vector3.MoveTowards(pawn.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            pawn.position = targetPosition;
            yield return new WaitForSeconds(0.2f);
        }

        // 後退が終わったら、現在のターンを終了させる
        currentState = NpcStateEnum.Idle;
        OnTurnEnd?.Invoke();
    }

    public int GetCurrentWaypointIndex()
    {
        // ウェイポイントのインデックスを返すロジックを実装
        // このメソッドはNPCが現在どのマスにいるかを示すインデックスを返す
        // あなたのコードの構造に合わせて実装してください
        return currentWaypointIndex; // 既存のcurrentWaypointIndex変数を返す
    }
}
