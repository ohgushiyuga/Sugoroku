// RankingDataHolder.cs

using System.Collections.Generic;
using UnityEngine;

public class RankingDataHolder : MonoBehaviour
{
    // シングルトンインスタンス
    public static RankingDataHolder Instance { get; private set; }

    // 順位情報を格納する変数
    public List<GameManager.RankEntry> FinalRankings { get; private set; }

    void Awake()
    {
        // インスタンスがまだ存在しない場合、自身をシングルトンとして設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されないようにする
        }
        // 既にインスタンスが存在する場合、重複を防ぐために自身を破棄
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// GameManagerから順位情報を受け取り、保存する
    /// </summary>
    public void SetFinalRankings(List<GameManager.RankEntry> rankings)
    {
        FinalRankings = rankings;
    }
}