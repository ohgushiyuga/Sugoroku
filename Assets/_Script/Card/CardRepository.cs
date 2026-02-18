using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CardRepository : MonoBehaviour
{
    // インスペクターで全種類のカードデータを登録するリスト
    [SerializeField] private List<CardData> allCardData;

    // Typeからデータを引く辞書（高速化用）
    private Dictionary<CardType, CardData> dataCache;

    void Awake()
    {
        // 辞書を作って検索を速くする
        if (allCardData != null)
        {
            dataCache = allCardData.ToDictionary(c => c.cardType, c => c);
        }
    }

    // カードのデータ（見た目など）を取得
    public CardData GetCardData(CardType type)
    {
        if (dataCache != null && dataCache.ContainsKey(type))
        {
            return dataCache[type];
        }
        return null;
    }

    // カードの効果（ロジック）を取得する「工場」
    // ★新しいカードを作ったらここに追加するだけでOK
    public ICardEffect GetCardEffect(CardType type)
    {
        switch (type)
        {
            // Enum.cs に定義されている CardType に合わせて追加してください
            case CardType.PlusTwoRoll:
                return new PlusTwoRollEffect();
            
            // 例: PlusThreeRoll というタイプがあるなら
            // case CardType.PlusThreeRoll: return new PlusThreeRollEffect();

            // トラップ系
            case CardType.Trap: 
                return new PassiveEffect();

            default:
                return new NoEffect();
        }
    }
    
    // 全カードリストを取得（デバッグや図鑑機能用）
    public List<CardData> GetAllCards() => allCardData;
}