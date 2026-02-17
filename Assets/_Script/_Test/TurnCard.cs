// ファイル名: TurnCard.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurnCard : MonoBehaviour
{
    public Sprite spriteCardFront;
    public Sprite spriteCardBack;

    // staticを外して、カードごとに状態を持つように変更
    private bool isFront = true;
    private float speed = 4.0f;

    private RectTransform rectTransform;
    private Image cardImage;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cardImage = GetComponent<Image>();
    }

    private void Start()
    {
        // 初期状態を設定
        isFront = true;
        cardImage.sprite = spriteCardFront;
        rectTransform.localScale = Vector3.one;
    }

    // publicにして、CardEventControllerから呼び出せるように変更
    public IEnumerator Turn()
    {
        Vector3 startScale = Vector3.one;
        Vector3 endScale = new Vector3(0f, 1.0f, 1.0f);
        float tick = 0f;

        // 表から裏へ
        while (tick < 1.0f)
        {
            tick += Time.deltaTime * speed;
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, tick);
            yield return null;
        }

        // 画像を裏返す
        isFront = !isFront;
        cardImage.sprite = isFront ? spriteCardFront : spriteCardBack;
        
        tick = 0f;

        // 裏から表へ
        while (tick < 1.0f)
        {
            tick += Time.deltaTime * speed;
            rectTransform.localScale = Vector3.Lerp(endScale, startScale, tick);
            yield return null;
        }
    }
}