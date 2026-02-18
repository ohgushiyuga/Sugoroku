using UnityEngine;
using System;

public class CardManager : MonoBehaviour
{
    [Header("依存関係")]
    [SerializeField] private CardRepository cardRepository;
    
    // シングルトン
    public static CardManager Instance { get; private set; }

    // 現在選択中のカード
    private CardType? selectedCard = null;

    // イベント：UIを更新させるための通知
    public event Action<CardData> OnCardSelected; 
    public event Action<CardType> OnCardUsed;

    // 参照キャッシュ
    private PlayerState playerState;
    private GameManager gameManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 必要なコンポーネントの取得
        if (cardRepository == null) cardRepository = FindObjectOfType<CardRepository>();
        playerState = FindObjectOfType<PlayerState>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // --- カード選択 ---
    // UI（ボタン）から呼ばれる
    public void SelectCard(CardType cardType)
    {
        // 同じカードを再タップしたら選択解除
        if (selectedCard == cardType)
        {
            DeselectCard();
            return;
        }

        selectedCard = cardType;
        
        // リポジトリからデータを取得してUIに通知
        CardData data = cardRepository.GetCardData(cardType);
        OnCardSelected?.Invoke(data);
    }

    // 選択解除
    public void DeselectCard()
    {
        selectedCard = null;
        OnCardSelected?.Invoke(null); // nullを送ってUIを消させる
    }

    // --- カード使用 ---
    // 「使う」ボタンから呼ばれる
    public void UseSelectedCard()
    {
        if (selectedCard == null || playerState == null) return;
        
        CardType type = selectedCard.Value;

        // 1. 持っているか確認
        if (!playerState.HasCard(type))
        {
            Debug.LogWarning("そのカードは持っていません");
            DeselectCard();
            return;
        }

        // 2. 効果の取得と実行（Strategyパターン）
        ICardEffect effect = cardRepository.GetCardEffect(type);
        effect.Execute(playerState);

        // 3. 消費処理
        playerState.UseCard(type);
        OnCardUsed?.Invoke(type); // 手札UI更新用

        // 4. 後片付け
        DeselectCard();
    }

    // --- その他（GameManager連携など） ---

    // カード獲得時の処理（イベントマスなどで呼ばれる）
    public void ConfirmCardAcquisition(CardType card)
    {
        if (playerState != null) playerState.AddCard(card);
        // ターン終了処理へ（必要なら）
        if (gameManager != null) gameManager.EndPlayerTurn();
    }

    // 出目の計算（GameManagerから呼ばれる）
    public int GetFinalRollValue(PlayerState player, int rawRoll)
    {
        if (player == null) return rawRoll;
        
        int final = rawRoll + player.rollModifier;
        
        // 修正値を使ったのでリセット
        player.rollModifier = 0; 
        
        return final;
    }
}