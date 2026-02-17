// ファイル名: BossBattlePresenter.cs
using UnityEngine;

public class BossBattlePresenter : MonoBehaviour
{
    [SerializeField] private BossBattleView view; // 表示係への参照
    [SerializeField] private GameObject diceCanvas;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        // Viewからの通知（ボタンクリック）を受け取る設定
        if (view != null)
        {
            view.OnRollClicked += HandleRollRequest;
        }
    }

    private void OnDestroy()
    {
        // オブジェクト破棄時に通知の受け取りを解除
        if (view != null)
        {
            view.OnRollClicked -= HandleRollRequest;
        }
    }

    // GameManagerから呼ばれ、ボス戦を開始する
    public void StartBattle()
    {
        if (view != null) view.Show();
    }

    // Viewから「振る」ボタンが押されたことが通知される
    private void HandleRollRequest()
    {
        if (diceCanvas != null)
        {
            // サイコロUIを表示する（結果はGameManagerが受け取る）
            diceCanvas.SetActive(true);
        }
    }
    
 　　/// GameManagerからサイコロの結果が渡され、勝敗を判定する
    public void ResolveBattle(int roll)
    {
        // クリア条件（4以上）を満たしているか
        if (roll >= 4)
        {
            // 勝利した場合
            if (view != null) view.ShowResult("勝利！");
            // GameManagerにゲームクリアを通知
            gameManager.GameClear("Player", true);
        }
        else
        {
            // 失敗した場合
            if (view != null) view.ShowResult("失敗...次のターンに再挑戦");
            // GameManagerにターン終了を通知
            gameManager.EndPlayerTurn();
        }
    }
}
