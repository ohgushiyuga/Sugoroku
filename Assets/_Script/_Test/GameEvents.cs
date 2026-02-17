// ファイル名: GameEvents.cs
using System;
using UnityEngine;

public static class GameEvents
{
    // 移動要求があったときに発生するイベント (引数は移動方向)
    public static event Action<Vector3> OnMoveRequested;

    // イベントを発生させるメソッド
    public static void RaiseMoveRequested(Vector3 direction)
    {
        OnMoveRequested?.Invoke(direction);
    }
}