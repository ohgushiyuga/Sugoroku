using UnityEngine;

public class EventManager : MonoBehaviour
{
    private CardManager cardManager;
    private GameManager gameManager; // GameManagerへの参照

    void Start()
    {
        cardManager = FindObjectOfType<CardManager>();
        gameManager = FindObjectOfType<GameManager>(); //ゲーム開始時にGameManagerを探しておく
    }

    public void TriggerEvent(TileEventType eventType)
    {
        switch (eventType)
        {
            case TileEventType.Card:
                if (cardManager != null)
                {
                    cardManager.StartCardAcquisitionEvent();
                }
                //カードイベントは、演出後のボタンでターンが終了するので、ここでは何もしない
                break;

            case TileEventType.Nuts:
                Debug.Log("Nutsマスに止まった！次のターン、ここからスタートすると出目に＋2追加される。");
                gameManager.EndPlayerTurn(); //イベントが終わったので、ターンを終了する
                break;
        
            case TileEventType.Save:
                Debug.Log("中間地点に到達！ここが新しいスタート地点になります。");
                if (gameManager != null)
                {
                    PlayerMovementController playerController = FindObjectOfType<PlayerMovementController>();
                    if (playerController != null)
                    {
                        gameManager.SetCheckpoint(playerController.transform.position);
                    }
                }
                gameManager.EndPlayerTurn(); //イベントが終わったので、ターンを終了する
                break;

            case TileEventType.None:
                Debug.Log("なにもないマスだ。");
                gameManager.EndPlayerTurn(); //イベントが終わったので、ターンを終了する
                break;

            case TileEventType.Boar:
                Debug.Log("イノシシに遭遇！2マス後退する。");
                PlayerMovementController player = FindObjectOfType<PlayerMovementController>();
                if (player != null)
                {
                    player.ForceMoveBack(2);
                }
                // ★ターン終了はPlayerMovementControllerが行うので、ここでは呼ばない
                break;

            default:
                Debug.Log(eventType + " のイベント処理はまだ実装されていません。");
                gameManager.EndPlayerTurn(); //未実装のイベントでも、とりあえずターンは終了させる
                break;
        }
    }
}