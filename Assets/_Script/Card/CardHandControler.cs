using UnityEngine;
using System.Collections.Generic;

public class CardHandController : MonoBehaviour
{
    [Header("UI Prefab")]
    [SerializeField] private GameObject cardButtonPrefab; // カード1枚を表すボタンPrefab
    [SerializeField] private Transform handContainer;     // ボタンを並べる親オブジェクト

    private CardRepository cardRepository;
    private PlayerState playerState;

    void Start()
    {
        cardRepository = FindObjectOfType<CardRepository>();
        playerState = FindObjectOfType<PlayerState>();

        // イベント購読：カードを使ったら手札を再描画する
        if (CardManager.Instance != null)
        {
            CardManager.Instance.OnCardUsed += (type) => RefreshHand();
        }

        // 初回描画
        RefreshHand();
    }

    // 手札UIの更新
    public void RefreshHand()
    {
        if (playerState == null || cardRepository == null) return;

        // 今あるボタンを全削除
        foreach (Transform child in handContainer)
        {
            Destroy(child.gameObject);
        }

        // 持っているカードの分だけボタン生成
        foreach (var cardType in playerState.heldCards)
        {
            CardData data = cardRepository.GetCardData(cardType);
            if (data == null) continue;

            GameObject btnObj = Instantiate(cardButtonPrefab, handContainer);
            
            // ★ボタンに画像やクリックイベントを設定する処理
            // （CardUseButtonなどのスクリプトがPrefabについている想定）
            var buttonScript = btnObj.GetComponent<CardUseButton>(); 
            if (buttonScript != null)
            {
                // ボタンが押されたら CardManager.SelectCard を呼ぶように設定
                buttonScript.Setup(data, () => CardManager.Instance.SelectCard(cardType));
            }
        }
    }
}