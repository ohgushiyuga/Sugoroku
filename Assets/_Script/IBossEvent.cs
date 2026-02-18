public interface IBossEvent
{
    // ボスイベントを開始する
    void StartEvent(GameManager gameManager);

    // サイコロの結果を判定する
    void ResolveDiceRoll(int diceRoll);
}