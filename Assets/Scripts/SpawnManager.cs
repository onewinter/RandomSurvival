using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private GameEvents gameEvents;

    // enemy types SO list
    [SerializeField] private List<EnemySetup> enemySetups;

    #region Events

    private void OnEnable()
    {
        gameEvents.NewGameEvent.RegisterListener(OnNewGame);
    }

    private void OnDisable()
    {
        gameEvents.NewGameEvent.UnregisterListener(OnNewGame);
    }

    void OnNewGame()
    {
        var enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            if (!enemy || !enemy.isActiveAndEnabled) continue;
            enemy.Recycle();
        }
    }

    #endregion

    public bool SpawnEnemy(Tile tile, Chunk chunk)
    {
        // spawn a random enemy fitting the provided parameters
        var validSetups = enemySetups.Where(setup =>
            setup.AllowedBiomes.Contains(tile.BiomeType) && setup.AllowedHeights.Contains(tile.BiomeHeight)).ToList();
        
        // if no enemies found, return false
        if (validSetups.Count == 0) return false;
        
        // call the factory method on the SO to get a new spawn
        var newSetup = validSetups[Random.Range(0, validSetups.Count)];
        newSetup.CreateNewEnemy(tile, chunk, transform);
        
        return true;
    }
}


    

