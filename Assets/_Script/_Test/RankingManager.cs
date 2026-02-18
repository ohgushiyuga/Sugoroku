using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private SceneLoader sceneLoader;

    // ゴールしたキャラクターのリスト
    private List<RankEntry> finishedCharacters = new List<RankEntry>();

    // キャラクターがゴールした時に呼ぶ
    public void RegisterGoal(string name, bool isPlayer, int turnCount)
    {
        finishedCharacters.Add(new RankEntry(name, isPlayer, turnCount, true));
    }

    // ゲーム終了時に順位を計算してシーン遷移する
    public void FinalizeRankingAndEndGame(PlayerMovementController player, List<RandomNpcController> npcs)
    {
        Debug.Log("--- 順位を決定します ---");

        // 1. ゴールしたキャラクター（1位）
        var firstPlace = finishedCharacters.FirstOrDefault(c => c.IsGoal);

        // 2. まだゴールしていないキャラをリストアップ
        var otherCharacters = new List<RankEntry>();

        // プレイヤーが未ゴールなら追加
        if (firstPlace == null || !firstPlace.IsPlayer)
        {
            if (player != null)
            {
                // ウェイポイントの進捗を取得（未実装なら0扱いでもエラーにはならない）
                int progress = player.GetCurrentWaypointIndex();
                otherCharacters.Add(new RankEntry(player.name, true, progress, false));
            }
        }

        // NPCが未ゴールなら追加
        foreach (var npc in npcs)
        {
            // まだゴールリストに名前がない場合のみ
            if (finishedCharacters.All(c => c.CharacterName != npc.name))
            {
                otherCharacters.Add(new RankEntry(npc.name, false, npc.GetCurrentWaypointIndex(), false));
            }
        }

        // 3. 進んでいる順（ウェイポイントIndexが大きい順）にソート
        var sortedByProgress = otherCharacters.OrderByDescending(c => c.GoalTurn)
                                              .ThenBy(c => !c.IsPlayer) // 同着ならプレイヤー優先など
                                              .ToList();

        // 4. 同着時のサイコロ判定
        ResolveTies(sortedByProgress);

        // 5. 最終リスト作成
        var finalRankings = new List<RankEntry>();

        // ログ出力
        for (int i = 0; i < finalRankings.Count; i++)
        {
            string status = finalRankings[i].IsGoal ? "（ゴール）" : "（未ゴール）";
            Debug.Log($"{i + 1}位: {finalRankings[i].CharacterName} ({status})");
        }

        // 6. データを保存してシーン遷移
        if (RankingDataHolder.Instance != null)
        {
            RankingDataHolder.Instance.SetFinalRankings(finalRankings);
        }

        if (sceneLoader != null)
        {
            sceneLoader.LoadSceneDirectAfterDelay("GameClearScene", 2f);
        }
    }

    private void ResolveTies(List<RankEntry> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            var current = list[i];
            var next = list[i + 1];

            // 両方NPCかつ進捗が同じ場合
            if (!current.IsPlayer && !next.IsPlayer && current.GoalTurn == next.GoalTurn)
            {
                current.TiebreakerRoll = Random.Range(1, 7);
                next.TiebreakerRoll = Random.Range(1, 7);

                if (current.TiebreakerRoll < next.TiebreakerRoll)
                {
                    var temp = list[i];
                    list[i] = list[i + 1];
                    list[i + 1] = temp;
                }
            }
        }
    }
}