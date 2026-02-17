using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    // InspectorでプレイヤーのTransformを設定
    public Transform playerTransform;

    // UI表示位置のオフセット
    public Vector3 offset;

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            // プレイヤーのワールド座標をスクリーン座標に変換してUIの位置を設定
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(playerTransform.position + offset);
            transform.position = screenPosition;
        }
    }
}