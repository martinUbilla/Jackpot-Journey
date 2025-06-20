using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Configuración de generación")]
    public GameObject[] chunkPrefabs;     // Array de prefabs de chunk
    public int chunkSize = 42;            // Tamaño de cada chunk (42x42)
    public int renderDistance = 1;        // Cuántos chunks mantener activos a la redonda
    public Transform player;              // Referencia al jugador

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
