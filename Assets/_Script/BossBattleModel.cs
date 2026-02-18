// ボス戦の「ルール」と「データ」だけを持つクラス
public class BossBattleModel
{
    // 勝利条件の閾値（これを外部から設定できるようにすれば、強いボスも作れる）
    private readonly int winThreshold;

    public BossBattleModel(int threshold = 4)
    {
        this.winThreshold = threshold;
    }

    // サイコロの目を判定するロジック
    // Presenterは「判定して！」と頼むだけで、具体的な数字を知らなくて済む
    public bool IsWin(int diceRoll)
    {
        return diceRoll >= winThreshold;
    }

    // 勝利時のメッセージなどもModelに持たせると、ボスごとの性格を表現できる
    public string GetResultText(bool isWin)
    {
        return isWin ? "勝利！" : "失敗...次のターンに再挑戦";
    }
}