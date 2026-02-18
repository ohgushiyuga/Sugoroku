using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardEventController : MonoBehaviour
{
    [SerializeField] private TurnCard turnCard;
    [SerializeField] private Button cardButton;
    [SerializeField] private Button acquireButton;
    private CardManager cardManager;
    private CardType cardToAcquire;

    void Start()
    {
        cardManager = FindObjectOfType<CardManager>();
        cardButton.onClick.AddListener(OnCardTapped);
        acquireButton.onClick.AddListener(OnAcquireButtonClicked);
    }

    // CardManagerから呼ばれる
    public void Show(CardType card)
    {
        this.cardToAcquire = card;
        gameObject.SetActive(true);
        cardButton.interactable = true;
        acquireButton.gameObject.SetActive(false);
    }

    private void OnCardTapped()
    {
        cardButton.interactable = false;
        StartCoroutine(CardFlipSequence());
    }

    private IEnumerator CardFlipSequence()
    {
        if (turnCard != null) yield return StartCoroutine(turnCard.Turn());
        OnAnimationFinished();
    }

    private void OnAnimationFinished()
    {
        acquireButton.gameObject.SetActive(true);
    }
    
    private void OnAcquireButtonClicked()
    {
        // CardManagerに、どのカードの獲得が確定したかを通知
        if (cardManager != null)
        {
            cardManager.ConfirmCardAcquisition(cardToAcquire);
        }
        gameObject.SetActive(false);
    }
}