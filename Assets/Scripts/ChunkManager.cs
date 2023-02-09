using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;


public class ChunkManager : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private GameEvents gameEvents;
    
    [Header("Chunk Setup")]    
    [SerializeField] private Chunk chunkPrefab;
    [SerializeField] private Tile tilePrefab;
    
    private const float ElevationScale = .05f;
    private const float TemperatureScale = .025f;
    private const float RainfallScale = .15f;

    public IObjectPool<Tile> TilePool;
    private List<Chunk> chunksList;

    private float randomSeed1;
    private float randomSeed2;
    private float randomSeed3;

    #region Unity Lifecycle
    private void OnEnable()
    {
        TilePool = new ObjectPool<Tile>(CreatePooledTile, OnTakeTileFromPool, OnTileReturnedToPool, OnDestroyTile);
        gameEvents.NewGameEvent.RegisterListener(OnNewGame);
    }

    private void OnDisable()
    {
        gameEvents.NewGameEvent.UnregisterListener(OnNewGame);
    }

    #endregion

    #region Events

    void OnNewGame()
    {
        var oldChunks = FindObjectsOfType<Chunk>();
        foreach (var oldChunk in oldChunks)
        {
            if (!oldChunk || !oldChunk.isActiveAndEnabled) continue;
            RecycleBiome(oldChunk);
        }
        
        randomSeed1 = Random.value * 3f;
        randomSeed2 = Random.value * 33f;
        randomSeed3 = Random.value * 333f;
        //chunksList = new List<Chunk> { GenerateBiome(new Vector2(0, 0)) };
        chunksList = new List<Chunk> { GenerateBiome(GameData.FirstBiome) };
    }

    #endregion

    #region Pooling
    private void OnTakeTileFromPool(Tile pooledObject)
    {
        if (!pooledObject) return; // sometimes the pool destroys an object before we can grab it... 
        pooledObject.gameObject.SetActive(true);
    }

    private Tile CreatePooledTile() => Instantiate(tilePrefab);

    private void OnTileReturnedToPool(Tile pooledObject)
    {
        if (!pooledObject) return;  // sometimes the pool destroys an object before we can return it...
        
        pooledObject.ResetBiome();
        pooledObject.gameObject.SetActive(false);
    }

    private void OnDestroyTile(Tile pooledObject) => Destroy(pooledObject.gameObject);

    // create and initialize a pooled tile
    private Tile CreateNewTile(float elevation, float temperature, float rainfall, Vector3 position, Transform parent)
    {
        Tile newTile;
        do
        {
            newTile = TilePool.Get();
        } while (!newTile);  // ensure we actually get an object from the pool
        newTile.transform.position = position;
        newTile.transform.parent = parent;
        newTile.AssignBiome(elevation, temperature, rainfall);
        return newTile;
    }
    #endregion

    #region Biome Generation
    public void CheckStartNewBiome(Vector2 startPosition, Vector2 direction)
    {
        Debug.Log("Start New Biome " + startPosition + " " + direction);

        Vector2 biomeDirection;
        biomeDirection = direction.x is < 0 or > 0 ? direction * GameData.BiomeSizeX : direction * GameData.BiomeSizeY;
        
        var biomePosition = startPosition + biomeDirection;
        
        // tell the player and camera to move to the requested new biome
        gameEvents.MovePlayerEvent.Raise(biomeDirection);
        gameEvents.MoveCameraEvent.Raise(biomePosition);
        
        // if we don't already have a chunk at this position, make one and add it to our list
        if (chunksList.Any(x => x.ChunkPosition == biomePosition)) return;
        chunksList.Add(GenerateBiome(biomePosition));
    }

    public void RecycleBiome(Chunk trashChunk)
    {
        // remove an unused biome
        Debug.Log("RecycleBiome: " + trashChunk);
        if (!chunksList.Contains(trashChunk)) return;

        chunksList.Remove(trashChunk);
        trashChunk.RecycleChunk();
        Destroy(trashChunk.gameObject);
    }

    Chunk GenerateBiome(Vector2 startPosition)
    {
        Debug.Log("Generate Biome " + startPosition);
        
        var chunkPosition = new Vector3(startPosition.x + GameData.BiomeSizeX / 2f - .5f, 0f, startPosition.y  + GameData.BiomeSizeY / 2f - .5f);
        var newChunk = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
        newChunk.transform.parent = transform;
        newChunk.ChunkPosition = startPosition;
        
        // loop through our grid and generate perlin noise for each tile, then assign a biome to each
        for (var i = 0; i < GameData.BiomeSizeX; i++)
        {
            for (var j = 0; j < GameData.BiomeSizeY; j++)
            {
                var elevation =
                    Mathf.PerlinNoise((startPosition.x + i + randomSeed1) * ElevationScale, (startPosition.y + j + randomSeed1) * ElevationScale);
                var temperature =
                    Mathf.PerlinNoise((startPosition.x + i + randomSeed2) * TemperatureScale, (startPosition.y + j + randomSeed2) * TemperatureScale);
                var rainfall =
                    Mathf.PerlinNoise((startPosition.x + i + randomSeed3) * RainfallScale, (startPosition.y + j + randomSeed3) * RainfallScale);
                
                var newPosition = new Vector3(startPosition.x + i, 0f, startPosition.y + j);
                CreateNewTile(elevation, temperature, rainfall, newPosition, newChunk.transform);
            }
        }
        return newChunk;
    }
    #endregion
}
