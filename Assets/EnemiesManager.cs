using System;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Vector2 spawnArea;
    [SerializeField] float spawnTimer;
    [SerializeField] GameObject player;
    float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            SpawnEnemy();
            timer = spawnTimer;
        }
    }

    private void SpawnEnemy()
    {
        Vector3 position = GenerateRandomPosition();

        position += player.transform.position;

        GameObject newEnemy = Instantiate(enemy);
        newEnemy.transform.position = position;
        newEnemy.GetComponent<EnemyCard0>().SetTarget(player);
    }

    private Vector3 GenerateRandomPosition()
    {
        Vector3 position = new Vector3(UnityEngine.Random.Range(-spawnArea.x, spawnArea.x)
                                        , UnityEngine.Random.Range(-spawnArea.y, spawnArea.y)
                                            , 0f);

        return position;
    }
}
