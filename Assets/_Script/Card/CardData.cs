using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "Game/Card Data")]
public class CardData : ScriptableObject
{
    [Header("カード基本情報")]
    public CardType cardType;      // Enum.cs で定義したタイプ
    public string cardName;        // 表示名
    public Sprite cardImage;       // アイコン画像
    
    [TextArea(3, 5)]
    public string description;     // 説明文
}