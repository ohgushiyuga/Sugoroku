using UnityEngine;

// すべてのカード効果が従うべきルール
public interface ICardEffect
{
    void Execute(PlayerState player);
}

// 出目を+2する効果
public class PlusTwoRollEffect : ICardEffect
{
    public void Execute(PlayerState player)
    {
        player.rollModifier += 2; // 加算式にしておくと、他の効果と重複できる
        Debug.Log("効果発動：次のサイコロ出目 +2");
    }
}

// 出目を+3する効果
public class PlusThreeRollEffect : ICardEffect
{
    public void Execute(PlayerState player)
    {
        player.rollModifier += 3;
        Debug.Log("効果発動：次のサイコロ出目 +3");
    }
}

// トラップカードなど、手動で使えないもの
public class PassiveEffect : ICardEffect
{
    public void Execute(PlayerState player)
    {
        Debug.Log("このカードは手札から使用できません（持っているだけで効果がある等）");
    }
}

// 効果なし（安全対策用）
public class NoEffect : ICardEffect
{
    public void Execute(PlayerState player) { Debug.Log("何も起こらなかった..."); }
}