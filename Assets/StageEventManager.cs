using System;
using UnityEngine;

public class StageEventManager : MonoBehaviour
{
    [SerializeField] StageData stageData;
    [SerializeField] EnemiesManager enemiesManager;
    StageTime stageTime;
    int eventIndexer;
    PlayerWin playerWin;
    private void Awake()
    {
        stageTime = GetComponent<StageTime>();
    }
    private void Start()
    {
        playerWin =  FindAnyObjectByType<PlayerWin>();
    }
    private void Update()
    {
        if (eventIndexer >= stageData.stageEvents.Count) { return; }


        if (stageTime.time > stageData.stageEvents[eventIndexer].time)
        {
            switch (stageData.stageEvents[eventIndexer].eventType)
            {
                case StageEventType.SpawnEnemy:
                    for (int i = 0; i < stageData.stageEvents[eventIndexer].count; i++)
                    {
                        enemiesManager.SpawnEnemy();
                    }
                    break;                
                case StageEventType.SpawnObject:

                    break;
                case StageEventType.WinStage:
                    WinStage();
                    break;
            }

            
            Debug.Log(stageData.stageEvents[eventIndexer].message);
            eventIndexer += 1;
        }
    }

    private void WinStage()
    {
        playerWin.Win();
    }
}
