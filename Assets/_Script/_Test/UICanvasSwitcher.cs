using UnityEngine;

public class UICanvasSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject canvasToHide;
    [SerializeField] private GameObject canvasToShow;
    [SerializeField] private float delayBeforeSwitch = 2f;

    public void SwitchCanvas()
    {
        if (canvasToHide != null)
        {
            canvasToHide.SetActive(false);
            Debug.Log("Canvas 非表示: " + canvasToHide.name);
        }

        if (canvasToShow != null)
        {
            Debug.Log("Canvas 表示予約: " + canvasToShow.name);
            Invoke(nameof(ShowNextCanvas), delayBeforeSwitch);
        }
        else
        {
            Debug.LogWarning("canvasToShow が設定されていません！");
        }
    }

    private void ShowNextCanvas()
    {
        canvasToShow.SetActive(true);
        Debug.Log("Canvas 表示完了: " + canvasToShow.name);
    }
}
