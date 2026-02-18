using UnityEngine;
using UnityEngine.UI;

public class SelectedCardDisplay : MonoBehaviour
{
    // 画像を表示するためのImageコンポーネント
    // Unityエディタ上で、パネル内のImageコンポーネントを持つGameObjectをここにドラッグ＆ドロップします。
    public Image displayImage;

    // 外部からこの関数を呼び出して、画像を設定します。
    public void SetCardImage(Sprite newSprite)
    {
        // 画像が設定されていれば、Imageコンポーネントに新しい画像を代入する
        if (displayImage != null)
        {
            displayImage.sprite = newSprite;
            // 画像を有効にする
            displayImage.enabled = true;
        }
    }
}