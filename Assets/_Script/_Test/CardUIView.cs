using UnityEngine;
using UnityEngine.UI;

public class CardUIView : MonoBehaviour
{
    // このカードのデータを持つスクリプトへの参照
    public CardDataHolder dataHolder;
    private CardManager cardManager;
    private CardType myCardType;

    [Header("UI設定")]
    [SerializeField] private Text cardNameText;
    [SerializeField] private Image cardImage;

    void Start()
    {
        // ここでCardManagerのインスタンスをキャッシュする
        cardManager = FindObjectOfType<CardManager>();

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnCardClicked);
        }
    }

    void Awake()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnCardClicked);
        }
    }

    // CardHandControllerから呼ばれ、カードの初期設定を行う
    public void Setup(CardData data)
    {
        // dataHolderにデータを設定
        if (dataHolder != null)
        {
            dataHolder.SetData(data); // CardDataHolderにSetDataメソッドが必要
        }

        // UIを更新
        if (cardNameText != null) cardNameText.text = data.cardName;
        if (cardImage != null && data.cardImage != null)
        {
            cardImage.sprite = data.cardImage;
            cardImage.enabled = true;
        }
    }


    public void UpdateUI()
    {
        if (dataHolder != null && dataHolder.Data != null)
        {
            if (cardNameText != null) cardNameText.text = dataHolder.Data.cardName;
            if (cardImage != null && dataHolder.Data.cardImage != null)
            {
                cardImage.sprite = dataHolder.Data.cardImage;
                cardImage.enabled = true; // 画像を表示
            }
        }
    }

    private void OnCardClicked()
    {
        // CardManagerにこのUIビューを渡す
        if (CardManager.Instance != null && dataHolder != null)
        {
            CardManager.Instance.SelectCard(dataHolder.Data.cardType);
        }
    }

    public void ShowEmpty()
    {
        if (cardNameText != null) cardNameText.text = "カードなし";
        if (cardImage != null)
        {
            cardImage.enabled = false; // 画像は非表示
        }
    }
}