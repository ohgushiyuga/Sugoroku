using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Fade : MonoBehaviour
{
    private Image image;
    void Start()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            // 黒から透明に1秒でフェード
            StartCoroutine(FadeOutAndDestroy(1f));
        }
    }
    private IEnumerator FadeOutAndDestroy(float duration)
    {
        Color startColor = Color.black;
        Color endColor = new Color(0, 0, 0, 0); // 黒の透明
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            image.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        image.color = endColor;
        // 終わったら自分を破棄
        Destroy(gameObject);
    }
}