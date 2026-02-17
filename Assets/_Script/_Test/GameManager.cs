using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // Listを使うために必要
using System.Linq; // NPCのリストを管理するために必要

public class GameManager : MonoBehaviour
{
[Header("シーン参照（インスペクターで設定）")]
[SerializeField] private ChunkGenerator chunkGenerator;
[SerializeField] private NpcSpawner npcSpawner;
[SerializeField] private GameObject diceCanvas;

 [Header("ゲーム設定")]
[SerializeField] private int numberOfNpcs = 3;
[SerializeField] private string gameClearSceneName = "GameClearScene";


    // --- 内部コンポーネント ---
private PlayerMovementController playerMovementController;
private CardManager cardManager;
private PlayerState playerState;
private UIManager uiManager;
private SceneLoader sceneLoader;

// --- ゲーム状態管理 ---
public enum GamePhase { Playing, BossBattle, GameClear }
public GamePhase currentPhase { get; private set; }
public enum Turn { Player, NPC }
public Turn currentTurn { get; private set; }
private Vector3? checkpointPosition = null;
private IBossEvent currentBossEvent;
private BossEventType currentBoss;
private List<RandomNpcController> npcControllers = new List<RandomNpcController>();
private List<NpcState> npcStates = new List<NpcState>();
private int currentNpcIndex = 0;
private int turnCount = 0;
private List<RankEntry> finishedCharacters = new List<RankEntry>();
private IBossEvent currentPlayerBossEvent;

    void Start()
    {
        // --- 参照チェック ---
        if (chunkGenerator == null || npcSpawner == null)
        {
            Debug.LogError("ChunkGeneratorまたはNpcSpawnerがインスペクターでセットされていません！");
            return;
        }

        // --- コンポーネントを探す ---
        playerMovementController = FindObjectOfType<PlayerMovementController>();
        cardManager = FindObjectOfType<CardManager>();
        uiManager = FindObjectOfType<UIManager>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        playerState = FindObjectOfType<PlayerState>();

        // 複数のNPCを生成し、リストに格納する
        for (int i = 0; i < 3; i++) // 例として3体のNPCを生成
        {
            RandomNpcController npc = npcSpawner.SpawnNpc();
            if (npc != null)
            {
                 npc.OnTurnEnd += OnNpcTurnEnd;
                Vector3[] waypointPositions = chunkGenerator.GetWaypointPositions();
                npc.SetWaypoints(waypointPositions);
                npcControllers.Add(npc);
                npcStates.Add(npc.GetComponent<NpcState>());
            }
        }
        // --- ゲーム開始処理 ---
        currentPhase = GamePhase.Playing;
        StartPlayerTurn(); // ★必ずプレイヤーのターンから開始
    }

    private void OnEnable() { DiceRoll.OnDiceResult += HandleDiceResult; }
    private void OnDisable() { DiceRoll.OnDiceResult -= HandleDiceResult; }

    // DiceRollからサイコロの結果が通知されたときに呼ばれる
    private void HandleDiceResult(int rawResult)
    {
        // ボス戦中かどうかの判定
        if (currentPhase == GamePhase.BossBattle && currentBossEvent != null)
        {
            currentBossEvent.ResolveDiceRoll(rawResult);
            // サイコロUIを非表示にする
            if (diceCanvas != null)
            {
                diceCanvas.SetActive(false);
            }
            return;
        }

        // プレイヤーのターンでなければ何もしない
        if (currentTurn != Turn.Player) return;

        // CardManagerを使って、生の出目に出目修正値を加算
        int finalResult = cardManager.GetFinalRollValue(playerState, rawResult);

        // UIを更新
        uiManager?.ShowMovementUI();

        // PlayerMovementControllerに最終的な出目で移動を命令
        if (playerMovementController != null)
        {
            playerMovementController.StartMovePhase(finalResult);
        }
    }

    // PlayerMovementControllerから呼ばれ、ボスイベントを開始する
    public void StartBossEvent(BossEventType bossType)
    {
        if (currentPhase != GamePhase.Playing) return;
        currentPhase = GamePhase.BossBattle;
        currentBoss = bossType;
        if (uiManager != null) uiManager.HideAllGameplayUI();
        switch (bossType)
        {
            case BossEventType.Tiger:
                currentBossEvent = new DiceRollCheckEvent(
                title: "ボス：タイガー！",
                message: "サイコロを振り、3以下の目が出れば脱出できる！",
                successCondition: (diceRoll) => { return diceRoll <= 3; }
                );
                break;

            case BossEventType.Cat:
                currentBossEvent = new DiceRollCheckEvent(
                title: "ボス：タイガー！",
                message: "サイコロを振り、3以下の目が出れば脱出できる！",
                successCondition: (diceRoll) => { return diceRoll <= 3; }
                );
                break;
            case BossEventType.hawk:
                currentBossEvent = new DiceRollCheckEvent(
                title: "ボス：タイガー！",
                message: "サイコロを振り、3以下の目が出れば脱出できる！",
                successCondition: (diceRoll) => { return diceRoll <= 3; }
                );
                break;
        }
        if (currentBossEvent != null) currentBossEvent.StartEvent(this);
    }


    private IEnumerator ProcessNpcBossEvent(RandomNpcController npc, BossEventType bossType)
    {
        Debug.Log($"NPCがボス戦({bossType})に突入しました。");

        int npcRoll = UnityEngine.Random.Range(1, 7);
        Debug.Log($"NPCがサイコロを振って{npcRoll}が出ました。");

        if (bossType == BossEventType.Tiger)
        {
            if (npcRoll <= 3)
            {
                Debug.Log("NPCはボス戦に成功しました。");
                GameClear(npc.gameObject.name, false);
            }
            else
            {
                Debug.Log("NPCはボス戦に失敗しました！1マス後退します。");
                yield return StartCoroutine(npc.ForceMoveBack(1));
            }
        }
    }

    // StartNpcBossEventメソッドを変更
    public void StartNpcBossEvent(RandomNpcController npc, BossEventType bossType)
    {
        // コルーチンを開始
        StartCoroutine(ProcessNpcBossEvent(npc, bossType));
    }

    // ボスイベントからサイコロを振るよう依頼があった時に呼ばれる
    public void RollDiceForBoss()
    {
        if (diceCanvas != null) diceCanvas.SetActive(true);
    }


    public void StartPlayerTurn()
    {
        currentTurn = Turn.Player;
        Debug.Log("--- プレイヤーのターンです ---");

        // サイコロのUIを有効化する
        if (diceCanvas != null)
        {
            diceCanvas.SetActive(true);
        }

        if (playerMovementController != null && playerState != null)
        {
            // 1. プレイヤーがいる現在のマスの情報を取得
            TileData currentTile = playerMovementController.GetCurrentTile();
            // 2. もしNutsマスにいたら、出目+2の効果を適用
            if (currentTile != null && currentTile.EventType == TileEventType.Nuts)
            {
                Debug.Log("Nutsマスの効果発動！次の出目が+2されます。");
                playerState.rollModifier = 2;
            }
        }
        if (uiManager != null) uiManager.ShowPlayerActionUI();
    }



    public void EndPlayerTurn()
    {
        if (currentTurn != Turn.Player) return;
        Debug.Log("プレイヤーのターンが終了しました。");
        if (uiManager != null) uiManager.ShowNpcTurnUI();
        // NPCのターン開始処理を呼び出し
        currentNpcIndex = 0; // 常に最初のNPCから始める
        StartNPCTurn();
    }


    private void StartNPCTurn()
    {
        currentTurn = Turn.NPC;
        if (currentNpcIndex < npcControllers.Count)
        {
            Debug.Log($"--- NPC {currentNpcIndex + 1} のターンです ---");
            npcControllers[currentNpcIndex].StartMyTurn();
        }
        else
        {
            // ★修正：全てのNPCのターンが終了したら、プレイヤーのターンに戻る
            Debug.Log("全てのNPCのターンが終了しました。");
            if (uiManager != null) uiManager.ShowPlayerActionUI();
            StartPlayerTurn();
        }
    }


    private void OnNpcTurnEnd()
    {
        Debug.Log("GameManager: NPCのターン終了を検知しました。");
        // ★修正：次のNPCにターンを渡す
        currentNpcIndex++;
        StartNPCTurn();
    }

    public void GameClear(string characterName, bool isPlayer)
    {
        currentPhase = GamePhase.GameClear;
        Debug.Log($"{characterName} がゴールしました！");

        // 順位付けのロジックを実行
        DetermineRankings(); // ← 呼び出しを維持

        if (sceneLoader != null)
        {
            // シーン遷移を遅延実行
            sceneLoader.LoadSceneDirectAfterDelay("GameClearScene", 2f);
        }
    }



    //public void GameClear()
    //{
       // currentPhase = GamePhase.GameClear;
       // Debug.Log("ゲームクリア！");
       // if (sceneLoader != null)
      //  {
       //     sceneLoader.LoadSceneDirectAfterDelay("GameClearScene", 2f);
      //  }
    //}


    private void DetermineRankings()
    {
        Debug.Log("--- 順位を決定します ---");

        // 1. ゴールしたキャラクター（1位）を確定
        var firstPlace = finishedCharacters.FirstOrDefault(c => c.IsGoal);

        // 2. 残りのキャラクターの順位を決定
        var otherCharacters = new List<RankEntry>();

        // プレイヤーがまだゴールしていない場合のみ、順位付け対象に含める
        if (firstPlace == null || !firstPlace.IsPlayer)
        {
            if (playerMovementController != null)
            {
                // デバッグログを追加して、プレイヤーのウェイポイントインデックスを確認
                int playerWaypointIndex = playerMovementController.GetCurrentWaypointIndex();
                Debug.Log($"プレイヤーの現在のウェイポイントインデックス: {playerWaypointIndex}");
                otherCharacters.Add(new RankEntry(playerMovementController.gameObject.name, true, playerWaypointIndex, isGoal: false));
            }
        }

        // NPCがまだゴールしていない場合、順位付け対象に含める
        foreach (var npc in npcControllers)
        {
            if (finishedCharacters.All(c => c.CharacterName != npc.gameObject.name))
            {
                otherCharacters.Add(new RankEntry(npc.gameObject.name, false, npc.GetCurrentWaypointIndex(), isGoal: false));
            }
        }

        // 3. ウェイポイントインデックスの降順（進んでいる順）でソート
        var sortedByProgress = otherCharacters.OrderByDescending(c => c.GoalTurn)
                                            .ThenBy(c => !c.IsPlayer)
                                            .ToList();

        // 4. NPC同士の同着をサイコロで解決
        for (int i = 0; i < sortedByProgress.Count - 1; i++)
        {
            var current = sortedByProgress[i];
            var next = sortedByProgress[i + 1];

            if (!current.IsPlayer && !next.IsPlayer && current.GoalTurn == next.GoalTurn)
            {
                current.TiebreakerRoll = UnityEngine.Random.Range(1, 7);
                next.TiebreakerRoll = UnityEngine.Random.Range(1, 7);

                if (current.TiebreakerRoll < next.TiebreakerRoll)
                {
                    var temp = sortedByProgress[i];
                    sortedByProgress[i] = sortedByProgress[i + 1];
                    sortedByProgress[i + 1] = temp;
                }
            }
        }

        // 5. 最終的な順位リストを作成
        var finalRankings = new List<RankEntry>();
        if (firstPlace != null)
        {
            finalRankings.Add(firstPlace);
        }
        finalRankings.AddRange(sortedByProgress);

        for (int i = 0; i < finalRankings.Count; i++)
        {
            string status = finalRankings[i].IsGoal ? "（ゴール）" : "（未ゴール）";
            Debug.Log($"{i + 1}位: {finalRankings[i].CharacterName} (進捗: {finalRankings[i].GoalTurn} {status})");
        }

        // 6. 最終的なシーン切り替え
        if (sceneLoader != null)
        {
            // 順位情報をRankingDataHolderに渡す
            if (RankingDataHolder.Instance != null)
            {
                RankingDataHolder.Instance.SetFinalRankings(finalRankings);
            }
        }
        else
        {
            Debug.LogError("SceneLoaderの参照がありません。インスペクターで設定してください。");
        }
    }

    
    public class RankEntry
    {
        public string CharacterName;
        public bool IsPlayer;
        public int GoalTurn;
        public bool IsGoal;
        public int TiebreakerRoll;

        public RankEntry(string name, bool isPlayer, int turn, bool isGoal)
        {
            CharacterName = name;
            IsPlayer = isPlayer;
            GoalTurn = turn;
            IsGoal = isGoal;
        }
    }

  public void OnBossBattleFailed()
{
    Debug.Log("ボス戦に失敗したため、ペナルティを実行します。");
    if (playerMovementController != null)
    {
        // ボス戦失敗時の専用コルーチンを開始
        StartCoroutine(ProcessFailedBossBattle());
    }
}


private IEnumerator ProcessFailedBossBattle()
{
    // 後退処理の完了を待機
    yield return StartCoroutine(playerMovementController.ExecuteForcedMove());
    
    // 後退処理が完了したら、プレイヤーのターンを終了
    EndPlayerTurn();
}



    private TileData GetTileAt(Vector3 worldPosition)
    {
        Chunk currentChunk = chunkGenerator.GetGeneratedChunk();
        if (currentChunk == null) return null;

        Vector3Int gridPos = new Vector3Int(
            Mathf.RoundToInt(worldPosition.x),
            0,
            Mathf.RoundToInt(worldPosition.z)
        );
        return currentChunk.AllTiles.FirstOrDefault(t => t.GridPosition == gridPos);
    }
    
    public void SetCheckpoint(Vector3 position) { checkpointPosition = position; }
    public Vector3 GetRespawnPosition() { return checkpointPosition ?? new Vector3(0, 1, 0); }
}