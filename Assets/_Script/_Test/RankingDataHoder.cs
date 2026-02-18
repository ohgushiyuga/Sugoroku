using UnityEngine;
using System.Collections.Generic;

public class RankingDataHolder : MonoBehaviour
{
    public static RankingDataHolder Instance { get; private set; }
    public List<RankEntry> FinalRankings { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetFinalRankings(List<RankEntry> rankings)
    {
        FinalRankings = rankings;
    }
}