using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public enum StageEventType
{
    SpawnEnemy,
    SpawnObject,
    WinStage
}
[Serializable]
public class StageEvent
{
    public StageEventType eventType;
    public float time;
    public string message;
    public EnemyData enemyToSpawn;
    public GameObject objectToSpawn;
    public int count;
}

[CreateAssetMenu]
public class StageData:ScriptableObject
{
    public List<StageEvent> stageEvents;
}
