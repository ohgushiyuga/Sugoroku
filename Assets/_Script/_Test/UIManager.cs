using UnityEngine;

/// ゲーム内の主要なUIパネル（CanvasやPanel）の表示・非表示を専門に管理する。
/// GameManagerなどの司令塔から「〇〇用の画面にして」という大まかな命令を受け取り、
/// 具体的なSetActive(true/false)の処理を実行する。
public class UIManager : MonoBehaviour
{
    [Header("ターンUIパネル（インスペクターで設定）")]
    [SerializeField] private GameObject playerActionPanel; // プレイヤーが行動を選択するUI (Canvas_Play)
    [SerializeField] private GameObject npcTurnPanel;    // NPCのターンであることを示すUI (Canvas_NPC)

    [Header("ゲームプレイUI（インスペクターで設定）")]
    [SerializeField] private GameObject movementArrowCanvas; // 移動用の矢印UI (Canvas_Div)

    public GameObject MovementArrowCanvas { get { return movementArrowCanvas; } }

    void Start()
    {
        // ゲーム開始時は、プレイヤーが行動を選択する画面から始める
        ShowPlayerActionUI();
    }

    /// プレイヤーが行動を選択するUIを表示する
    public void ShowPlayerActionUI()
    {
        Debug.Log("--- UIManager: ShowPlayerActionUIが呼ばれました ---");
        if (playerActionPanel != null) playerActionPanel.SetActive(true);
        if (npcTurnPanel != null) npcTurnPanel.SetActive(false);
        if (movementArrowCanvas != null) movementArrowCanvas.SetActive(false);
    }

    /// NPCのターンであることを示すUIを表示する
    public void ShowNpcTurnUI()
    {
        if (playerActionPanel != null) playerActionPanel.SetActive(false);
        if (npcTurnPanel != null) npcTurnPanel.SetActive(true);
        if (movementArrowCanvas != null) movementArrowCanvas.SetActive(false);
    }

    /// プレイヤーが移動するための矢印UIを表示する
    public void ShowMovementUI()
    {
        if (playerActionPanel != null) playerActionPanel.SetActive(false);
        if (npcTurnPanel != null) npcTurnPanel.SetActive(false);
        if (movementArrowCanvas != null) movementArrowCanvas.SetActive(true);
    }
    
     /// ゲームプレイに関連する全てのUIを非表示にする
    public void HideAllGameplayUI()
    {
        // 矢印UIを隠す
        if (movementArrowCanvas != null) movementArrowCanvas.SetActive(false);
        
        // 他にもゲームプレイ中に表示されるUIがあれば、ここに追加していく
        // 例: if (cardEventCanvas != null) cardEventCanvas.SetActive(false);
    }
}