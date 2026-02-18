using UnityEngine;
using UnityEngine.UI;

public class SelectedCardDisplay : MonoBehaviour
{
    [SerializeField] private Image displayImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private GameObject displayPanel; // パネル全体

    void Start()
    {
        // イベント購読：カードが選ばれたらUIを更新
        if (CardManager.Instance != null)
        {
            CardManager.Instance.OnCardSelected += UpdateDisplay;
        }
        
        // 最初は非表示
        Hide();
    }

    private void UpdateDisplay(CardData data)
    {
        if (data == null)
        {
            Hide();
            return;
        }

        // データをUIに反映
        Show();
        if (displayImage != null) displayImage.sprite = data.cardImage;
        if (nameText != null) nameText.text = data.cardName;
        if (descriptionText != null) descriptionText.text = data.description;
    }

    private void Show()
    {
        if (displayPanel != null) displayPanel.SetActive(true);
    }

    private void Hide()
    {
        if (displayPanel != null) displayPanel.SetActive(false);
    }
    
    // オブジェクト破棄時にイベント解除（安全対策）
    void OnDestroy()
    {
        if (CardManager.Instance != null)
        {
            CardManager.Instance.OnCardSelected -= UpdateDisplay;
        }
    }
}