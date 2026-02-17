// ファイル名: ConfirmDialogController.cs
using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmDialogController : MonoBehaviour
{
    [SerializeField] private Text titleText;

    private Action onOK;
    private Action onCancel;

    /// DialogManagerから呼び出され、内容を初期化する
    
    public void Initialize(ConfirmDialogOptions options)
    {
        // nullチェックで、プレハブにTextがなくてもエラーにならないようにする
        if (titleText != null)
        {
            titleText.text = options.Title;
        }

        onOK = options.OnOK;
        onCancel = options.OnCancel;
    }

    public void OnTapOK()
    {
        onOK?.Invoke();
        Destroy(gameObject);
    }

    public void OnTapCancel()
    {
        onCancel?.Invoke();
        Destroy(gameObject);
    }
}