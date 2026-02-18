using UnityEngine;
using UnityEngine.UI;
using System;

public class CardUseButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    // 初期化メソッド（CardHandControllerから呼ばれる）
    public void Setup(CardData data, Action onClickAction)
    {
        // 画像のセット
        if (iconImage != null && data != null)
        {
            iconImage.sprite = data.cardImage;
        }

        // ボタンが押されたら実行する処理を登録
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickAction?.Invoke());
    }
}