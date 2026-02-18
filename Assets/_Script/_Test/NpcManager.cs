using UnityEngine;
using System.Collections.Generic;
using System;

public class NpcManager : MonoBehaviour
{
    [SerializeField] private NpcSpawner npcSpawner;
    [SerializeField] private ChunkGenerator chunkGenerator; // ルート情報取得用

    private List<RandomNpcController> npcControllers = new List<RandomNpcController>();
    
    // NPCのターンが終わったことを知らせるイベント
    public event Action OnAllNpcsFinished;
    
    private int currentNpcIndex = 0;

    // 初期化：NPCを生成する
    public void InitializeNpcs(int count)
    {
        if (npcSpawner == null || chunkGenerator == null) return;

        Vector3[] waypoints = chunkGenerator.GetWaypointPositions();

        for (int i = 0; i < count; i++)
        {
            RandomNpcController npc = npcSpawner.SpawnNpc();
            if (npc != null)
            {
                // イベント購読（NPC個別のターン終了時）
                npc.OnTurnEnd += OnSingleNpcTurnEnd;
                npc.SetWaypoints(waypoints);
                npcControllers.Add(npc);
            }
        }
    }

    // NPCターン開始の合図
    public void StartNpcTurnSequence()
    {
        currentNpcIndex = 0;
        ProcessNextNpc();
    }

    private void ProcessNextNpc()
    {
        if (currentNpcIndex < npcControllers.Count)
        {
            Debug.Log($"--- NPC {currentNpcIndex + 1} のターン ---");
            npcControllers[currentNpcIndex].StartMyTurn();
        }
        else
        {
            // 全員終わった
            Debug.Log("全てのNPCのターン終了");
            OnAllNpcsFinished?.Invoke();
        }
    }

    // 個別のNPCが「終わったよ」と言ってきた時
    private void OnSingleNpcTurnEnd()
    {
        currentNpcIndex++;
        ProcessNextNpc(); // 次の人へ
    }

    // 外部（RankingManagerなど）向けにリストを公開
    public List<RandomNpcController> GetAllNpcs()
    {
        return npcControllers;
    }
}