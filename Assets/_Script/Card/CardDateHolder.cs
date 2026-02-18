using UnityEngine;

public class CardDataHolder : MonoBehaviour
{
    // このカードの全てのデータをInspectorで設定
    [SerializeField] private CardData cardData;

    public CardData Data => cardData;
    
    // CardHandControllerからデータを設定するためのメソッド
    public void SetData(CardData newData)
    {
        cardData = newData;
    }
}