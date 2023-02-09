using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Enemy_", menuName = "Enemy Setup", order = 51)]
public class EnemySetup : ScriptableObject
{
    [SerializeField] private Enemy enemyPrefab;
    
    public string Name;
    public Color MaterialColor;
    public GameObject ModelPrefab;
    public EnemyBrain EnemyBrain;
    public List<BiomeTypes> AllowedBiomes;
    public List<BiomeHeights> AllowedHeights;
    public List<GameObject> Loot;
    public float LootChance = .75f;
    public float Health;
    public float Damage;
    
    public IObjectPool<Enemy> EnemyPool;
    private ChunkManager chunkManager;

    #region Unity Lifecycle

    private void OnEnable()
    {
        EnemyPool = new ObjectPool<Enemy>(CreatePooledEnemy, OnTakeEnemyFromPool, OnEnemyReturnedToPool,
            OnDestroyEnemy);
    }

    void Start()
    {
        chunkManager = FindObjectOfType<ChunkManager>();
    }


    #endregion

    private void OnTakeEnemyFromPool(Enemy pooledObject)
    {
        if (!pooledObject) return; // sometimes the pool destroys an object before we can grab it... 
        pooledObject.gameObject.SetActive(true);
    }

    private Enemy CreatePooledEnemy() => Instantiate(enemyPrefab);

    private void OnEnemyReturnedToPool(Enemy pooledObject)
    {
        if (!pooledObject) return; // sometimes the pool destroys an object before we can return it...

        pooledObject.Reset();
        pooledObject.gameObject.SetActive(false);
    }

    private void OnDestroyEnemy(Enemy pooledObject) => Destroy(pooledObject.gameObject);

    // create and initialize a pooled enemy
    public Enemy CreateNewEnemy(Tile tile, Chunk chunk, Transform parent)
    {
        Enemy newEnemy;
        do
        {
            newEnemy = EnemyPool.Get();
        } while (!newEnemy); // ensure we actually get an object from the pool

        newEnemy.name = Name;
        newEnemy.transform.position = tile.transform.position + Vector3.up;
        newEnemy.transform.parent = parent;
        
        newEnemy.ParentChunk = chunk;
        newEnemy.ParentPool = EnemyPool;
        
        newEnemy.Initialize(this);
        chunk.AddEnemy(newEnemy);
        
        return newEnemy;
    }
}
