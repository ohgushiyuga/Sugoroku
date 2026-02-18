using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // サブマネージャー
    [Header("Sub Managers")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private NpcManager npcManager;
    [SerializeField] private BossBattleManager bossBattleManager;
    [SerializeField] private RankingManager rankingManager;
    [SerializeField] private CardManager cardManager;

    [Header("Scene Refs")]
    [SerializeField] private PlayerMovementController playerController;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private UIManager uiManager;

    // ゲーム状態
    public enum GamePhase { Playing, BossBattle, GameClear }
    public GamePhase CurrentPhase { get; private set; }

    void Start()
    {
        // 依存関係の解決（もしインスペクターで設定し忘れていても自動で探す）
        if (turnManager == null) turnManager = GetComponent<TurnManager>() ?? gameObject.AddComponent<TurnManager>();
        if (npcManager == null) npcManager = GetComponent<NpcManager>() ?? gameObject.AddComponent<NpcManager>();
        if (bossBattleManager == null) bossBattleManager = GetComponent<BossBattleManager>() ?? gameObject.AddComponent<BossBattleManager>();
        if (rankingManager == null) rankingManager = GetComponent<RankingManager>() ?? gameObject.AddComponent<RankingManager>();
        
        if (playerController == null) playerController = FindObjectOfType<PlayerMovementController>();
        if (cardManager == null) cardManager = FindObjectOfType<CardManager>();
        if (playerState == null) playerState = FindObjectOfType<PlayerState>();
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();

        // 初期化処理
        npcManager.InitializeNpcs(3); // 3体のNPC生成
        turnManager.Initialize(npcManager, playerController, playerState);

        // ゲーム開始
        CurrentPhase = GamePhase.Playing;
        turnManager.StartPlayerTurn();
    }

    private void OnEnable()
    {
        DiceRoll.OnDiceResult += HandleDiceResult;
    }

    private void OnDisable()
    {
        DiceRoll.OnDiceResult -= HandleDiceResult;
    }

    // サイコロのイベントハンドリング（ここだけは中継役として残る）
    private void HandleDiceResult(int rawResult)
    {
        // A. ボス戦中の場合 -> BossManagerへ
        if (CurrentPhase == GamePhase.BossBattle)
        {
            bossBattleManager.ResolvePlayerDiceRoll(rawResult);
            return;
        }

        // B. 通常プレイ中（プレイヤーのターン） -> 移動処理へ
        if (CurrentPhase == GamePhase.Playing && turnManager.CurrentTurn == TurnManager.Turn.Player)
        {
            if (uiManager != null) uiManager.ShowMovementUI();
            
            // カード効果などで出目を補正
            int finalResult = cardManager != null ? cardManager.GetFinalRollValue(playerState, rawResult) : rawResult;
            
            // プレイヤー移動開始
            if (playerController != null)
            {
                playerController.StartMovePhase(finalResult);
            }
        }
    }

    // --- 外部からの呼び出し口（インターフェース実装やイベントから呼ばれる） ---

    // プレイヤーがボス戦マスに止まった
    public void StartBossEvent(BossEventType bossType)
    {
        CurrentPhase = GamePhase.BossBattle;
        bossBattleManager.StartPlayerBossBattle(bossType, this);
    }

    // NPCがボス戦マスに止まった
    public void StartNpcBossEvent(RandomNpcController npc, BossEventType bossType)
    {
        StartCoroutine(bossBattleManager.ProcessNpcBossBattle(npc, bossType, (isWin) => 
        {
            if (isWin)
            {
                GameClear(npc.name, false);
            }
            // 負けた場合はNPC側で後退処理などが終わっているので何もしなくてOK
        }));
    }

    // ボス戦UIから「振る」ボタンが押された時
    public void RollDiceForBoss()
    {
        bossBattleManager.ShowDiceUI();
    }

    // ボス戦に負けた時（プレイヤー）
    public void OnBossBattleFailed()
    {
        Debug.Log("プレイヤー: ボス戦敗北");
        StartCoroutine(ProcessFailedPlayerBossBattle());
    }

    private IEnumerator ProcessFailedPlayerBossBattle()
    {
        // プレイヤーの後退処理
        yield return StartCoroutine(playerController.ExecuteForcedMove());
        
        // 終わったらターン終了して通常フェーズに戻る
        CurrentPhase = GamePhase.Playing;
        EndPlayerTurn();
    }

    // 誰かがゴールした
    public void GameClear(string characterName, bool isPlayer)
    {
        Debug.Log($"{characterName} ゴール！");
        
        // ランキングに登録
        // ※ turnCountの取得ロジックが必要ならここに追加（今回は簡易化のため0またはplayerControllerから取得）
        int turns = 0; 
        rankingManager.RegisterGoal(characterName, isPlayer, turns);

        // プレイヤーがゴールした、または全員終わった等の条件でゲーム終了処理へ
        if (isPlayer)
        {
            CurrentPhase = GamePhase.GameClear;
            rankingManager.FinalizeRankingAndEndGame(playerController, npcManager.GetAllNpcs());
        }
    }

    // プレイヤーの移動完了などでターンを終わらせる用
    public void EndPlayerTurn()
    {
        CurrentPhase = GamePhase.Playing; // 万が一ボス戦後なら戻す
        turnManager.EndPlayerTurn();
    }
    
    // 互換性維持のためのヘルパー（もし他のスクリプトがGameManager経由でcheckpointを使っていたら）
    private Vector3? checkpointPosition = null;
    public void SetCheckpoint(Vector3 pos) => checkpointPosition = pos;
    public Vector3 GetRespawnPosition() => checkpointPosition ?? new Vector3(0, 1, 0);
}