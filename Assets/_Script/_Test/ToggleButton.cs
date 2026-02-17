using UnityEngine;
using UnityEngine.UI; // Buttonを扱うために必要
using System; // Actionを使うために必要

[RequireComponent(typeof(Button))]
public class ToggleButton : MonoBehaviour
{
    public Image frameImage; // オン・オフで見た目が変わるフレームのImage
    public Sprite spriteOn;  // オン状態の画像
    public Sprite spriteOff; // オフ状態の画像

    // 自分が押されたことを司令塔に伝えるためのイベント
    public event Action<ToggleButton> OnButtonClicked;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked);
    }

    // 自分がクリックされたら、司令塔に自分自身の情報を渡して通知する
    private void OnClicked()
    {
        OnButtonClicked?.Invoke(this);
    }

    /// <summary>
    /// 司令塔から呼ばれ、自分の見た目をオン状態にする
    /// </summary>
    public void SetOn()
    {
        if (frameImage != null)
        {
            frameImage.sprite = spriteOn;
        }
    }

    /// <summary>
    /// 司令塔から呼ばれ、自分の見た目をオフ状態にする
    /// </summary>
    public void SetOff()
    {
        if (frameImage != null)
        {
            frameImage.sprite = spriteOff;
        }
    }
}