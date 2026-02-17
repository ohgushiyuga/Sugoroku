// ファイル名: DialogManager.cs
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    /// どのスクリプトからでも呼び出せる、ダイアログ表示の命令
    public static void Show(ConfirmDialogOptions options)
    {
        if (string.IsNullOrEmpty(options.PrefabPath))
        {
            Debug.LogError("PrefabPathが指定されていません！");
            return;
        }

        var prefab = Resources.Load<GameObject>(options.PrefabPath);
        if (prefab == null)
        {
            Debug.LogError(options.PrefabPath + " がResourcesフォルダに見つかりません。");
            return;
        }

        var obj = Instantiate(prefab);
        var controller = obj.GetComponent<ConfirmDialogController>();
        if (controller != null)
        {
            controller.Initialize(options);
        }
    }
}