using UnityEngine;
using UnityEngine.UI;

public class CardEventController : MonoBehaviour
{
    [Header("UI参照")]
    [SerializeField] private GameObject acquisitionPanel; // 「カードゲット！」のポップアップ
    [SerializeField] private Image cardImage;             // ゲットしたカードの絵を表示
    [SerializeField] private Text cardNameText;
    [SerializeField] private Button okButton;

    private CardType currentCard;
    private CardRepository cardRepository;
    private CardManager cardManager;

    void Start()
    {
        cardRepository = FindObjectOfType<CardRepository>();
        cardManager = FindObjectOfType<CardManager>();

        if (acquisitionPanel != null) acquisitionPanel.SetActive(false);
        
        if (okButton != null)
        {
            okButton.onClick.AddListener(OnOkClicked);
        }
    }

    // イベントマスから呼ばれるメソッド
    public void StartAcquisitionEvent(CardType type)
    {
        currentCard = type;
        
        // データを取得して表示
        CardData data = cardRepository.GetCardData(type);
        if (data != null)
        {
            if (acquisitionPanel != null) acquisitionPanel.SetActive(true);
            if (cardImage != null) cardImage.sprite = data.cardImage;
            if (cardNameText != null) cardNameText.text = $"{data.cardName} を手に入れた！";
        }
        else
        {
            // データがない場合は即終了などの安全策
            OnOkClicked();
        }
    }

    private void OnOkClicked()
    {
        if (acquisitionPanel != null) acquisitionPanel.SetActive(false);

        // CardManagerを通じてプレイヤーに付与し、ターンエンド等の処理へ
        if (cardManager != null)
        {
            cardManager.ConfirmCardAcquisition(currentCard);
        }
    }
}