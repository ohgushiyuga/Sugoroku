using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    void Start()
    {
        // 1.5秒後にHideObject関数を呼び出す
        Invoke("HideObject", 1.5f);
    }

    private void HideObject()
    {
        // このGameObjectを非表示にする
        gameObject.SetActive(false);
    }
}