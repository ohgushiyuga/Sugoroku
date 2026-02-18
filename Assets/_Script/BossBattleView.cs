using UnityEngine;
using UnityEngine.UI;
using System;

public class BossBattleView : MonoBehaviour
{
    [SerializeField] private Button rollButton;
    [SerializeField] private Text resultText;

    public event Action OnRollClicked;

    void Start()
    {
        rollButton.onClick.AddListener(() => OnRollClicked?.Invoke());
    }

    public void Show()
    {
        gameObject.SetActive(true);
        rollButton.interactable = true;
        resultText.text = "ボスが現れた！\nサイコロを振って4以上を出せ！";
    }

    public void Hide() => gameObject.SetActive(false);

    public void ShowResult(string message)
    {
        resultText.text = message;
        rollButton.interactable = false;
    }
}