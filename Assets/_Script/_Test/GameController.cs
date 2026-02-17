// GameControllerなど、ダイアログを呼び出したいスクリプトでの使用例
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Text Result;

    public void OnTapOpen_Pause()
    {
        var options = new ConfirmDialogOptions
        {
            PrefabPath = "Canvas_Pause", // どのプレハブを使うか指定
            OnOK = () => { Result.text = "PauseのOKが押されました"; }
        };
        DialogManager.Show(options);
    }

    public void OnTapOpen_Card()
    {
        var options = new ConfirmDialogOptions
        {
            PrefabPath = "Canvas_Card", 
            OnOK = () => { Result.text = "CardのOKが押されました"; }
        };
        DialogManager.Show(options);
    }

 public void OnTapOpen_Map()
    {
        var options = new ConfirmDialogOptions
        {
            PrefabPath = "Canvas_Map", 
            OnOK = () => { Result.text = "CardのOKが押されました"; }
        };
        DialogManager.Show(options);
    }

    public void OnTapOpen_Sound()
    {
        var options = new ConfirmDialogOptions
        {
            PrefabPath = "Canvas_Sound", 
            Title = "サウンド設定", // タイトルの設定
            OnOK = () => { Result.text = "SoundのOKが押されました"; },
            OnCancel = () => { Result.text = "Soundのキャンセルが押されました"; }
        };
        DialogManager.Show(options);
    }
}