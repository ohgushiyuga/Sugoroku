// ファイル名: DiceRollCheckEvent.cs
using UnityEngine;
using System; // Funcを使うために必要

public class DiceRollCheckEvent : IBossEvent
{
    private GameManager gameManager;
    private string title;
    private string message;
    private Func<int, bool> successCondition; // 成功条件を判定する「関数」そのものを保持する

    /// このイベントのルールを設定する
    public DiceRollCheckEvent(string title, string message, Func<int, bool> successCondition)
    {
        this.title = title;
        this.message = message;
        this.successCondition = successCondition;
    }

    public void StartEvent(GameManager manager)
    {
        this.gameManager = manager;

        // 設定された内容でダイアログを表示
        var options = new ConfirmDialogOptions
        {
            PrefabPath = "Canvas_Tiger", // あなたのダイアログプレハブ名
            Title = this.title,
            Message = this.message,
            OnOK = () => {
                gameManager.RollDiceForBoss();
            }
        };
        DialogManager.Show(options);
    }

    public void ResolveDiceRoll(int diceRoll)
    {
        Debug.Log("出目判定イベント！出目は... " + diceRoll);

        // 成功時の処理
        if (successCondition(diceRoll))
        {
            Debug.Log(title + "に成功しました！");
            // GameManager.GameClearに引数を追加
            // プレイヤーの名前を "Player" として渡す
            gameManager.GameClear("Player", true);
        }
        else
        {
            Debug.Log(title + "に失敗しました。");
            gameManager.OnBossBattleFailed();
        }

    }
}