using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    public enum Turn { Player, NPC }
    public Turn CurrentTurn { get; private set; }

    [Header("参照")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject diceCanvas;
    
    // 外部コンポーネントへの参照（Initで受け取るか、Inspectorで設定）
    private NpcManager npcManager;
    private PlayerMovementController playerController;
    private PlayerState playerState;

    // イベント
    public event Action OnPlayerTurnStarted;

    public void Initialize(NpcManager npcMgr, PlayerMovementController player, PlayerState pState)
    {
        this.npcManager = npcMgr;
        this.playerController = player;
        this.playerState = pState;

        // NPC全員のターンが終わったら、またプレイヤーのターンにする
        if (npcManager != null)
        {
            npcManager.OnAllNpcsFinished += StartPlayerTurn;
        }
    }

    public void StartPlayerTurn()
    {
        CurrentTurn = Turn.Player;
        Debug.Log("--- プレイヤーのターン ---");
        
        OnPlayerTurnStarted?.Invoke();

        if (uiManager != null) uiManager.ShowPlayerActionUI();
        if (diceCanvas != null) diceCanvas.SetActive(true);

        // タイル効果（Nuts）のチェック
        CheckCurrentTileEffect();
    }

    public void EndPlayerTurn()
    {
        if (CurrentTurn != Turn.Player) return;

        Debug.Log("プレイヤーターン終了 -> NPCターン開始");
        if (uiManager != null) uiManager.ShowNpcTurnUI();

        StartNpcTurn();
    }

    private void StartNpcTurn()
    {
        CurrentTurn = Turn.NPC;
        if (npcManager != null)
        {
            npcManager.StartNpcTurnSequence();
        }
    }

    private void CheckCurrentTileEffect()
    {
        if (playerController != null && playerState != null)
        {
            var tile = playerController.GetCurrentTile();
            if (tile != null && tile.EventType == TileEventType.Nuts)
            {
                Debug.Log("Nutsマス効果: 次の出目+2");
                playerState.rollModifier = 2;
            }
        }
    }
}