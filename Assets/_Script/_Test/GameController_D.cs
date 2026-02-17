using UnityEngine;

public class GameController_Dice : MonoBehaviour
{
    public GameObject canvasToHide;

    public void OnTapOpen()
    {
        if (canvasToHide != null)
        {
            canvasToHide.SetActive(false);
        }

        ConfirmDlogOptions_Dice op = new ConfirmDlogOptions_Dice
        {
            canvasToReactivate = canvasToHide, // 戻すキャンバスを渡す
            OkDelegete = () => Debug.Log("OK押した"),
            CancelDelegete = () => Debug.Log("キャンセル押した")
        };

        ConfirmDlogController_Dice.Show(op);
    }
}
