using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BiomeTypes
{
    Grassland,
    Jungle,
    Forest,
    Desert,
    Tundra,
    Water
}

public enum BiomeHeights
{
    Sunken,
    Flat,
    Hills,
    Mountain
}

public class Chunk : MonoBehaviour
{
    [SerializeField] private GameEvents gameEvents;
    
    public Vector2 ChunkPosition;
    
    private ChunkManager chunkManager;
    private List<Tile> tiles;
    [SerializeField] private List<Enemy> enemies;
    private List<ChunkTrigger> chunkTriggers;

    private SpawnManager spawnManager;
    private Player player;
    private bool chunkActive;

    private int spawnCount;
    private float timer;
    
    private void Awake()
    {
        chunkTriggers = GetComponentsInChildren<ChunkTrigger>().ToList();
        //Debug.Log("chunk awake");
    }

    void Start()
    {
        tiles = GetComponentsInChildren<Tile>().ToList();
        enemies = new List<Enemy>();
        chunkManager = FindObjectOfType<ChunkManager>();
        spawnManager = FindObjectOfType<SpawnManager>();
        player = FindObjectOfType<Player>();
        //Debug.Log("chunk start");
    }

    public void AddEnemy(Enemy enemy) => enemies.Add(enemy);
    
    public void RecycleChunk()
    {
        tiles.ForEach(x => chunkManager.TilePool.Release(x));
        foreach (var enemy in enemies.Where(enemy => enemy)) // need to check if enemy still exists
        {
            enemy.Recycle();
        }
    }

    public void OnChunkTrigger(Vector2 direction)
    {
        //Debug.Log("Start Next Chunk " + ChunkPosition + " " + direction);
        chunkManager.CheckStartNewBiome(ChunkPosition, direction);
    }

    void Update()
    {
        if (!player) return;
        
        // when the player is far enough away, recycle this biome
        var playerDistance = (player.transform.position - transform.position).sqrMagnitude;
        if(playerDistance >= 50f*50f) chunkManager.RecycleBiome(this);

        if (chunkActive) CheckSpawnEnemy();

        timer += Time.deltaTime;
    }

    void CheckSpawnEnemy()
    {
        if (spawnCount >= GameData.SpawnMax) return;
        if (timer < GameData.SpawnRate) return;

        // check to see if we can spawn an enemy here based on our types
        var spawnPosition = tiles[Random.Range(0, tiles.Count)];
        if (!spawnManager.SpawnEnemy(spawnPosition, this)) return; // if no valid enemies, we'll try again
        
        timer = 0;
        spawnCount++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Player>()) return;
        
        //Debug.Log("Chunk Enter");
        EnableChunkTriggers();
        chunkActive = true;
        timer = GameData.SpawnRate * .9f;

        // unpause chunk enemies when player comes back
        enemies.ForEach(enemy => enemy.Paused(false));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Player>()) return;
        
        //Debug.Log("Chunk Exit");
        EnableChunkTriggers(false);
        chunkActive = false;

        // pause all chunk enemies when player exits
        enemies.ForEach(enemy => enemy.Paused(true));
    }

    private void EnableChunkTriggers(bool enable = true)
    {
        chunkTriggers.ForEach(x=>x.EnableTrigger(enable));
    }
    
    
}
