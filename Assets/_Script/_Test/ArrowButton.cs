// ファイル名: ArrowButton.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArrowButton : MonoBehaviour
{
    private PlayerMovementController playerMovementController;
    private Vector3 moveDirection;

    /// <summary>
    /// PlayerMovementControllerから呼ばれ、ボタンの情報を初期化する
    /// </summary>
    public void Initialize(PlayerMovementController controller, Vector3 direction)
    {
        this.playerMovementController = controller;
        this.moveDirection = direction;
        
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnArrowClicked);
        }
    }

    /// <summary>
    /// ボタンがクリックされた時に呼び出される
    /// </summary>
    private void OnArrowClicked()
    {
        if (playerMovementController != null)
        {
            playerMovementController.OnArrowClicked(moveDirection);
        }
    }
}