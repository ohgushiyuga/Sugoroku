// ファイル名: PlayerController.cs
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float gridSize = 1.0f;

    private void OnEnable()
    {
        // イベントに関数を登録
        GameEvents.OnMoveRequested += Move;
    }

    private void OnDisable()
    {
        // イベントから関数を解除
        GameEvents.OnMoveRequested -= Move;
    }

    private void Move(Vector3 direction)
    {
        transform.position += direction * gridSize;
    }
}