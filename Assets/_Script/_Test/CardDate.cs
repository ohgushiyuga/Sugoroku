using UnityEngine;
using System;

[Serializable]
public class CardData
{
    public CardType cardType;
    public string cardName;
    public Sprite cardImage; // カードの画像
}

public enum CardType
{
    None,
    PlusTwoRoll,
    Trap
}