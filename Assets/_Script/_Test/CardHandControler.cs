using UnityEngine;
using System.Collections.Generic;

public class CardHandController : MonoBehaviour
{
    private PlayerState playerState;
    [SerializeField] private GameObject cardUIPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private List<CardData> allCardData;
    public CardDataHolder dataHolder;

    public void Setup(PlayerState targetPlayer)
    {
        // 既存のplayerStateがあれば、イベント登録を解除
        if (this.playerState != null)
        {
            this.playerState.OnHandChanged -= Redraw;
        }
    
        this.playerState = targetPlayer;
        // 新しいplayerStateにイベントを登録
        this.playerState.OnHandChanged += Redraw;

        Debug.Log("--- CardHandController: PlayerStateの監視を開始しました。 ---");

        Redraw(); // 初期表示
    }

    private void OnDestroy()
    {
        if (playerState != null) playerState.OnHandChanged -= Redraw;
    }

     public void OpenPanel()
    {
        Debug.Log("CardHandController.OpenPanel() が呼ばれました"); // ← 動作確認用ログ
        gameObject.SetActive(true);   // パネルを必ず開く
        Redraw();                     // 手札を再描画（空なら何も生成されない）
    }

    public void ClosePanel()
    { gameObject.SetActive(false); }

    private void Redraw()
    {
        if (playerState == null || cardUIPrefab == null) return;

        // 既存のUIを全部削除
        foreach (Transform child in gridParent) Destroy(child.gameObject);

        // 手札が空なら何も生成しない（画面は空でOK）
        if (playerState.heldCards.Count == 0)
        {
            Debug.Log("Redraw: 手札は0枚です");
            return;
        }

        // 通常はカードごとにUIを生成
        foreach (CardType cardType in playerState.heldCards)
        {
            CardData data = allCardData.Find(cd => cd.cardType == cardType);
            if (data != null)
            {
                GameObject cardObj = Instantiate(cardUIPrefab, gridParent);

                // CardDataHolderにデータをセット
                var dataHolder = cardObj.GetComponent<CardDataHolder>();
                if (dataHolder != null)
                {
                    dataHolder.SetData(data);
                }

                // CardUIViewのUIを更新
                var cardView = cardObj.GetComponent<CardUIView>();
                if (cardView != null)
                {
                    cardView.UpdateUI();
                }
            }
        }
    }
}