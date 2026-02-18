using UnityEngine;
using System.Collections;
using System;

public class BossBattleManager : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private GameObject diceCanvas;
    [SerializeField] private UIManager uiManager;

    private IBossEvent currentBossEvent;
    private BossEventType currentBossType;
    
    // イベント
    public event Action<bool> OnPlayerBossBattleResult; // trueなら勝利、falseなら敗北
    public event Action OnBossBattleStarted;

    // プレイヤーのボス戦開始
    public void StartPlayerBossBattle(BossEventType bossType, GameManager gameManager)
    {
        currentBossType = bossType;
        OnBossBattleStarted?.Invoke();

        if (uiManager != null) uiManager.HideAllGameplayUI();

        // ボスの種類に応じたイベント生成
        switch (bossType)
        {
            case BossEventType.Tiger:
                currentBossEvent = new DiceRollCheckEvent(
                    title: "ボス：タイガー！",
                    message: "サイコロを振り、3以下の目が出れば脱出できる！",
                    successCondition: (diceRoll) => { return diceRoll <= 3; }
                );
                break;
            // 他のボスも同様に追加可能
            default:
                // デフォルトの挙動（例：Catと同じなど）
                 currentBossEvent = new DiceRollCheckEvent(
                    title: "ボス：謎の敵！",
                    message: "サイコロを振り、3以下の目が出れば脱出できる！",
                    successCondition: (diceRoll) => { return diceRoll <= 3; }
                );
                break;
        }

        if (currentBossEvent != null)
        {
            currentBossEvent.StartEvent(gameManager);
        }
    }

    // サイコロの結果を受け取る
    public void ResolvePlayerDiceRoll(int diceRoll)
    {
        if (currentBossEvent != null)
        {
            currentBossEvent.ResolveDiceRoll(diceRoll);
            
            // UIを閉じる
            if (diceCanvas != null) diceCanvas.SetActive(false);
        }
    }

    // NPCのボス戦処理（コルーチン）
    public IEnumerator ProcessNpcBossBattle(RandomNpcController npc, BossEventType bossType, Action<bool> onComplete)
    {
        Debug.Log($"NPC {npc.name} がボス戦({bossType})に突入");

        // 演出待ち
        yield return new WaitForSeconds(1.0f);

        int npcRoll = UnityEngine.Random.Range(1, 7);
        Debug.Log($"NPCの出目: {npcRoll}");

        bool isWin = false;
        
        // 簡単のため、全ボス共通で「3以下なら勝ち」とする（必要に応じてswitch文で分岐）
        if (npcRoll <= 3)
        {
            Debug.Log("NPC勝利");
            isWin = true;
        }
        else
        {
            Debug.Log("NPC敗北");
            // 失敗時のペナルティ（後退など）を実行
            yield return npc.ForceMoveBack(1);
            isWin = false;
        }

        onComplete?.Invoke(isWin);
    }
    
    // UIからの呼び出し用
    public void ShowDiceUI()
    {
        if (diceCanvas != null) diceCanvas.SetActive(true);
    }
}