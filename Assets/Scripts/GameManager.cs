using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private GameEvents gameEvents;
    
    [Header("Game Setup")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerStart;

    private bool started;
    
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
        Time.timeScale = 1f;
        var oldPlayer = FindObjectOfType<Player>();
        if(oldPlayer)
        {
            oldPlayer.StopAllCoroutines();
            Destroy(oldPlayer.gameObject);
        }
        
        gameData.ResetStats();
        var startPosition = new Vector3(GameData.PlayerStart.x, 10f, GameData.PlayerStart.y);
        var player = Instantiate(playerPrefab, startPosition, Quaternion.identity);
        player.transform.parent = playerStart;
        gameEvents.MoveCameraEvent.Raise(GameData.FirstBiome);
        started = true;
    }

    private void Update()
    {
        if (!started) return; 
            
        gameData.Food -= GameData.FoodDecayRate * Time.deltaTime;
        if(gameData.Food <=0) gameData.Health -= GameData.HealthDecayRate * Time.deltaTime;
        
        if (!(gameData.Health <= 0)) return;
        started = false;
        gameEvents.GameOverEvent.Raise();
    }
}