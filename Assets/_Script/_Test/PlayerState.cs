using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public event Action OnHandChanged;
    public List<CardType> heldCards = new List<CardType>();
    public int rollModifier = 0;

    public void AddCard(CardType card)
    {
        heldCards.Add(card);
        
        if (OnHandChanged != null)
        {
            Debug.Log("--- PlayerState: OnHandChangedイベントを発行します。 ---");
            OnHandChanged.Invoke();
        }
        else
        {
            Debug.LogWarning("--- PlayerState: OnHandChangedイベントに誰も登録していません。UIは更新されません。 ---");
        }
    }
    
    public void UseCard(CardType card)
    {
        if (heldCards.Contains(card))
        {
            heldCards.Remove(card);
            OnHandChanged?.Invoke();
        }
    }

    public bool HasCard(CardType card) { return heldCards.Contains(card); }
}