// ファイル名: CardUseButton.cs
using UnityEngine;
using UnityEngine.UI;

public class CardUseButton : MonoBehaviour
{
    private Button useButton;
    private CardManager cardManager;

    void Start()
    {
        useButton = GetComponent<Button>();
        cardManager = FindObjectOfType<CardManager>();
        
        // ボタンにクリックイベントを登録
        if (useButton != null)
        {
            useButton.onClick.AddListener(OnUseButtonClick);
        }
    }

    private void OnUseButtonClick()
    {
        if (cardManager != null)
        {
            cardManager.UseSelectedCard();
        }
    }
}