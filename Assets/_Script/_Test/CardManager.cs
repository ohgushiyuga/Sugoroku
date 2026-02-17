// ファイル名: CardManager.cs
using UnityEngine;
using System; 
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    private PlayerState playerState;
    private CardAcquisitionController acquisitionController;
    private GameManager gameManager;
    private CardType? selectedCard = null;
    public event Action<CardType> OnCardSelected;
    [SerializeField] private SelectedCardDisplay selectedCardDisplay; // 右側のパネル
    [SerializeField] private List<CardData> allCardData; 
    public List<CardData> AllCardData => allCardData;


    void Start()
    {
        playerState = FindObjectOfType<PlayerState>();
        acquisitionController = FindObjectOfType<CardAcquisitionController>(true); 
        gameManager = FindObjectOfType<GameManager>();
    }
   void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartCardAcquisitionEvent()
    {
        if (acquisitionController != null)
        {
            CardType newCard = CardType.PlusTwoRoll;
            acquisitionController.Show(newCard);
        }
    }
    
    public void ConfirmCardAcquisition(CardType card)
    {
        if (playerState != null)
        {
            playerState.AddCard(card);
        }
        if (gameManager != null)
        {
            gameManager.EndPlayerTurn();
        }
    }

   /// カードをクリックした時に呼ばれ、カードを選択状態にする
    public void SelectCard(CardType cardType)
    {
         Debug.Log($"--- CardManager: SelectCardメソッドが呼び出されました。受け取ったCardType: {cardType.ToString()} ---");

        selectedCard = cardType;
        Debug.Log($"{selectedCard.ToString()}が選択されました。");
        
        // CardTypeからCardDataを探す
        CardData data = allCardData.Find(cd => cd.cardType == cardType);
        
        // データが見つかったか確認
        if (data == null)
        {
            Debug.LogError($"--- CardManager: CardDataが見つかりませんでした。CardType: {cardType.ToString()} ---");
        }

        // 右のパネルに画像を表示
        if (selectedCardDisplay != null && data != null)
        {
            selectedCardDisplay.SetCardImage(data.cardImage);
        }
        
        OnCardSelected?.Invoke(selectedCard.Value);
    }

    /// カード使用ボタンから呼ばれ、選択中のカードを使用する
    public void UseSelectedCard()
    {
        if (selectedCard.HasValue)
        {
            UseCard(playerState, selectedCard.Value);
            // カード使用後、選択状態をリセット
            selectedCard = null;
        }
    }

    /// CardUIViewから「このカードを使いたい」と要請があった時に呼ばれる
    public void RequestUseCard(CardType card)
    {
        // 選択されたカードを追跡
        SelectCard(card);
    }
    

    private void UseCard(PlayerState player, CardType card)
    {
        if (player == null || !player.HasCard(card)) return;
        switch (card)
        {
            case CardType.PlusTwoRoll:
                player.rollModifier = 2;
                Debug.Log("カード効果発動！次の出目が+2されます。");
                player.UseCard(card);
                break;
            case CardType.Trap:
                Debug.Log("Trapカードは手動では使用できません。");
                break;
        }
    }

    public int GetFinalRollValue(PlayerState player, int rawRoll)
    {
        if (player == null) return rawRoll;

        int finalRoll = rawRoll + player.rollModifier;
        
        if (player.rollModifier != 0)
        {
            Debug.Log("出目修正効果！ " + rawRoll + " -> " + finalRoll);
        }

        player.rollModifier = 0;
        
        return finalRoll;
    }

     public int GetFinalRollValue(NpcState npc, int rawRoll)
    {
        if (npc == null) return rawRoll;

        int finalRoll = rawRoll + npc.rollModifier;
        
        if (npc.rollModifier != 0)
        {
            Debug.Log("出目修正効果！ " + rawRoll + " -> " + finalRoll);
        }

        npc.rollModifier = 0;
        
        return finalRoll;
    }

    public bool TryToBlockObstruction(PlayerState player)
    {
        if (player.HasCard(CardType.Trap))
        {
            player.UseCard(CardType.Trap);
            return true;
        }
        return false;
    }
}