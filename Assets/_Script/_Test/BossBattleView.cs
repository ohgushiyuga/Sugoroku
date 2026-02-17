// ファイル名: BossBattleView.cs
using UnityEngine;
using UnityEngine.UI;
using System; // Actionを使うため

public class BossBattleView : MonoBehaviour
{
    [SerializeField] private Button rollButton;
    [SerializeField] private Text resultText;

    // 「サイコロを振る」ボタンが押されたことを司令塔に通知するイベント
    public event Action OnRollClicked;

    void Start()
    {
        rollButton.onClick.AddListener(() => {
            OnRollClicked?.Invoke();
        });
    }

    // 司令塔から命令を受けて、UIを表示する
    public void Show()
    {
        gameObject.SetActive(true);
        rollButton.interactable = true;
        resultText.text = "ボスが現れた！\nサイコロを振って4以上を出せ！";
    }

    // 司令塔から命令を受けて、UIを非表示にする
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // 司令塔から命令を受けて、結果を表示する
    public void ShowResult(string message)
    {
        resultText.text = message;
        rollButton.interactable = false; // 結果が出たらボタンは押せなくする
    }
}