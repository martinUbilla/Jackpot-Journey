using System;

using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Vector2 spawnArea;
    [SerializeField] float spawnTimer;
    [SerializeField] GameObject player;

    private void Start()
    {
        
        player = GameManager.instance.playerTransform.gameObject;

    }
    public void SpawnEnemy(EnemyData enemyToSpawn)
    {
        Vector3 position = GenerateRandomPosition() + player.transform.position;

        GameObject newEnemy = Instantiate(enemyToSpawn.enemyPrefab);
        newEnemy.transform.position = position;
        newEnemy.GetComponent<IEnemy>().SetTarget(player);
        newEnemy.transform.parent = transform;

        GameObject spriteObject = Instantiate(enemyToSpawn.animatedPrefab);
        spriteObject.transform.parent = newEnemy.transform;
        spriteObject.transform.localPosition = Vector3.zero;
    }

    private Vector3 GenerateRandomPosition()
    {
        Vector3 position = new Vector3(UnityEngine.Random.Range(-spawnArea.x, spawnArea.x)
                                        , UnityEngine.Random.Range(-spawnArea.y, spawnArea.y)
                                            , 0f);

        return position;
    }
}
