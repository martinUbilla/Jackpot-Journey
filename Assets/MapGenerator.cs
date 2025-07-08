using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Configuración de generación")]
    public GameObject[] chunkPrefabs;     // Array de prefabs de chunk
    public int chunkSize = 42;            // Tamaño de cada chunk (42x42)
    public int renderDistance = 1;        // Cuántos chunks mantener activos a la redonda
    public Transform player;              // Referencia al jugador

    [Header("Configuración de Objetos")]
    public GameObject pokerCasePrefab;    // Prefab del PokerCaseDrop
    public bool spawnObjectsInChunks = true;  // Activar/desactivar spawn de objetos
    [Range(0f, 1f)]
    public float objectSpawnProbability = 0.7f;  // Probabilidad de spawn por chunk
    public int minObjectsPerChunk = 1;
    public int maxObjectsPerChunk = 3;

    private Dictionary<Vector2Int, GameObject> spawnedChunks = new();
    private Vector2Int currentPlayerChunk;

    void Start()
    {
        UpdateChunks();
    }

    void Update()
    {
        Vector2Int newPlayerChunk = GetChunkCoordFromPosition(player.position);
        if (newPlayerChunk != currentPlayerChunk)
        {
            currentPlayerChunk = newPlayerChunk;
            UpdateChunks();
        }
    }

    Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int y = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(x, y);
    }

    void UpdateChunks()
    {
        HashSet<Vector2Int> neededChunks = new();

        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int y = -renderDistance; y <= renderDistance; y++)
            {
                Vector2Int chunkCoord = new(currentPlayerChunk.x + x, currentPlayerChunk.y + y);
                neededChunks.Add(chunkCoord);

                if (!spawnedChunks.ContainsKey(chunkCoord))
                {
                    Vector3 chunkWorldPos = new Vector3(
                        chunkCoord.x * chunkSize,
                        chunkCoord.y * chunkSize,
                        0
                    );

                    // Elegir un prefab al azar de la lista
                    GameObject prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
                    GameObject newChunk = Instantiate(prefab, chunkWorldPos, Quaternion.identity, transform);
                    spawnedChunks.Add(chunkCoord, newChunk);

                    // Spawnear objetos en el chunk recién creado
                    if (spawnObjectsInChunks)
                    {
                        SpawnObjectsInChunk(newChunk, chunkWorldPos);
                    }
                }
            }
        }

        // Eliminar chunks fuera del rango
        List<Vector2Int> toRemove = new();
        foreach (var chunk in spawnedChunks)
        {
            if (!neededChunks.Contains(chunk.Key))
            {
                Destroy(chunk.Value);
                toRemove.Add(chunk.Key);
            }
        }

        foreach (var coord in toRemove)
        {
            spawnedChunks.Remove(coord);
        }
    }

    private void SpawnObjectsInChunk(GameObject chunk, Vector3 chunkPosition)
    {
        // Verificar si este chunk debe tener objetos
        if (Random.Range(0f, 1f) > objectSpawnProbability) return;

        if (pokerCasePrefab == null)
        {
            Debug.LogWarning("PokerCasePrefab no asignado en ChunkGenerator");
            return;
        }

        int objectCount = Random.Range(minObjectsPerChunk, maxObjectsPerChunk + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPositionInChunk(chunkPosition);
            if (spawnPos != Vector3.zero)
            {
                GameObject spawnedObject = Instantiate(pokerCasePrefab, spawnPos, Quaternion.identity, chunk.transform);
                Debug.Log($"PokerCase spawneado en chunk en: {spawnPos}");
            }
        }
    }

    private Vector3 GetRandomSpawnPositionInChunk(Vector3 chunkCenter)
    {
        int maxAttempts = 10;
        float spawnRadius = chunkSize * 0.4f; // 40% del tamaño del chunk
        float minDistanceFromCenter = chunkSize * 0.1f; // 10% del tamaño del chunk

        for (int attempts = 0; attempts < maxAttempts; attempts++)
        {
            // Generar posición aleatoria dentro del chunk
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minDistanceFromCenter, spawnRadius);

            Vector3 randomPos = chunkCenter + new Vector3(
                randomDirection.x * randomDistance,
                randomDirection.y * randomDistance,
                0
            );

            // Verificar que no haya obstáculos (opcional, puedes agregar LayerMask si es necesario)
            // Si no tienes obstáculos específicos, puedes comentar esta línea
            if (!Physics2D.OverlapCircle(randomPos, 0.5f))
            {
                return randomPos;
            }
        }

        // Si no se encontró una posición válida, usar el centro del chunk con un pequeño offset
        Vector3 fallbackPos = chunkCenter + new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            0
        );

        return fallbackPos;
    }

    void OnDrawGizmos()
    {
        if (spawnedChunks == null) return;

        Gizmos.color = Color.green;
        foreach (var chunk in spawnedChunks)
        {
            Vector3 center = new Vector3(
                chunk.Key.x * chunkSize + chunkSize / 2f,
                chunk.Key.y * chunkSize + chunkSize / 2f,
                0
            );
            Gizmos.DrawWireCube(center, new Vector3(chunkSize, chunkSize, 0));
        }
    }
}