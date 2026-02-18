using UnityEngine;

public class BossBattlePresenter : MonoBehaviour, IBossEvent
{
    [SerializeField] private BossBattleView view;
    [SerializeField] private GameObject diceCanvas;

    private BossBattleModel model;
    private GameManager gameManager;

    // 初期化
    public void Initialize(int difficulty = 4)
    {
        model = new BossBattleModel(difficulty);
    }

    private void OnEnable()
    {
        if (view != null) view.OnRollClicked += HandleRollRequest;
    }

    private void OnDisable()
    {
        if (view != null) view.OnRollClicked -= HandleRollRequest;
    }

    // インターフェースの実装

    public void StartEvent(GameManager gm)
    {
        // 依存関係をメソッド注入で受け取る
        this.gameManager = gm;

        // モデルが未生成ならデフォルトで生成
        if (model == null) Initialize();

        view.Show();
    }

    public void ResolveDiceRoll(int diceRoll)
    {
        // Modelに判定させる
        bool isWin = model.IsWin(diceRoll);
        string message = model.GetResultText(isWin);

        // Viewに結果を表示させる
        view.ShowResult(message);

        // GameManagerに進行を伝える
        if (isWin)
        {
            gameManager.GameClear("Player", true);
        }
        else
        {
            gameManager.EndPlayerTurn();
        }
        
        // イベント終了後の片付けなどが必要ならここで行う
    }


    private void HandleRollRequest()
    {
        if (diceCanvas != null)
        {
            diceCanvas.SetActive(true);
            // ※注意: ここで DiceSystem などの別クラスがサイコロを振り、
            // その結果が ResolveDiceRoll を呼ぶ流れを想定しています
        }
    }
}