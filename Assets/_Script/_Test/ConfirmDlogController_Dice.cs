using UnityEngine;

public class ConfirmDlogOptions_Dice
{
    public System.Action OkDelegete;
    public System.Action CancelDelegete;
    public GameObject canvasToReactivate; //戻す対象のキャンバス
}
public class ConfirmDlogController_Dice : MonoBehaviour
{
    private static GameObject prefab;
    private ConfirmDlogOptions_Dice options;

    public static ConfirmDlogController_Dice Show(ConfirmDlogOptions_Dice op)
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Canvas_Dice");
        }

        GameObject obj = Instantiate(prefab);
        ConfirmDlogController_Dice me = obj.GetComponent<ConfirmDlogController_Dice>();
        me.ShowContent(op);
        return me;
    }

    private void ShowContent(ConfirmDlogOptions_Dice op)
    {
        options = op;
    }

    public void OnTapOK()
    {
        options?.OkDelegete?.Invoke();

        // 戻す対象があれば表示する
        if (options?.canvasToReactivate != null)
        {
            options.canvasToReactivate.SetActive(true);
        }

        Destroy(gameObject);
    }

    public void OnTapCancel()
    {
        options?.CancelDelegete?.Invoke();

        // Cancel時も同様に表示
        if (options?.canvasToReactivate != null)
        {
            options.canvasToReactivate.SetActive(true);
        }

        Destroy(gameObject);
    }
}
