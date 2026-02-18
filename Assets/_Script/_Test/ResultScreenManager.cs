// ResultScreenManager.cs

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreenManager : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private Text rankingText; // 順位を表示するUIテキスト

    void Start()
    {
        // RankingDataHolderから順位情報を取得
        if (RankingDataHolder.Instance != null && RankingDataHolder.Instance.FinalRankings != null)
        {
            DisplayRankings(RankingDataHolder.Instance.FinalRankings);
        }
        else
        {
            rankingText.text = "順位情報がありません。";
        }
    }

    /// <summary>
    /// 受け取った順位情報をUIに表示する
    /// </summary>
    private void DisplayRankings(List<RankEntry> rankings)
    {
        string resultString = "--- 最終順位 ---\n\n";
        for (int i = 0; i < rankings.Count; i++)
        {
            var rank = rankings[i];
            resultString += $"{i + 1}位: {rank.CharacterName}\n";
        }
        rankingText.text = resultString;
    }
}