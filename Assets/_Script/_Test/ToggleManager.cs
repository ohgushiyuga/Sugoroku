using System.Collections.Generic;
using UnityEngine;

public class ToggleManager : MonoBehaviour
{
    // 管理下にある全てのToggleButtonを保持するリスト
    private List<ToggleButton> toggleButtons = new List<ToggleButton>();
    
    // 現在オンになっているボタン
    private ToggleButton currentOnButton;

    void Start()
    {
        // 自分の子オブジェクトから全てのToggleButtonを探し出す
        GetComponentsInChildren<ToggleButton>(true, toggleButtons);

        // 各ボタンのクリックイベント（OnButtonClicked）を購読（リッスン）する
        foreach (var button in toggleButtons)
        {
            button.OnButtonClicked += HandleButtonClicked;
        }

        // 初期状態として、最初のボタンをオンにしておく（任意）
        if (toggleButtons.Count > 0)
        {
            HandleButtonClicked(toggleButtons[0]);
        }
    }

    /// <summary>
    /// いずれかのボタンがクリックされたときに呼ばれる
    /// </summary>
    private void HandleButtonClicked(ToggleButton clickedButton)
    {
        // もしクリックされたボタンが、既にオンのボタンでなければ
        if (currentOnButton != clickedButton)
        {
            // まず、全てのボタンをオフ状態にする
            foreach (var button in toggleButtons)
            {
                button.SetOff();
            }

            // クリックされたボタンだけをオン状態にする
            clickedButton.SetOn();

            // 現在オンのボタンとして記憶する
            currentOnButton = clickedButton;
            
            Debug.Log(clickedButton.name + " が選択されました。");
        }
    }

    private void OnDestroy()
    {
        // オブジェクトが破棄される際に、イベントの購読を解除
        foreach (var button in toggleButtons)
        {
            button.OnButtonClicked -= HandleButtonClicked;
        }
    }
}