using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private GameEvents gameEvents;
    
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameUIPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject creditsPanel;
    
    [SerializeField] private GameObject resumeButton;
    
    private bool menuOpen;
    private bool instructionsOpen;
    private bool gameOverOpen;
    private bool creditsOpen;
    private bool started;

    private float oldTimeScale;
    
    private void OnEnable()
    {
        gameEvents.GameOverEvent.RegisterListener(OnGameOver);
    }

    private void OnDisable()
    {
        gameEvents.GameOverEvent.UnregisterListener(OnGameOver);
    }

    void OnGameOver()
    {
        Time.timeScale = 0f;
        resumeButton.SetActive(false);
        started = false;
        gameOverOpen = true;
        
        gameOverPanel.SetActive(true);
    }
    
    public void OnClickNewGame()
    {
        oldTimeScale = 1f;
        Time.timeScale = 1f;

        gameEvents.NewGameEvent.Raise();
        gameOverPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        gameUIPanel.SetActive(true);
        resumeButton.SetActive(true);
        started = true;
    }

    void Start()
    {
        resumeButton.SetActive(false);
        mainMenuPanel.SetActive(true);
        gameUIPanel.SetActive(false);
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        
        if (gameOverOpen)
        {
            OnClickCloseSubmenu();
        }
        else if (creditsOpen)
        {
            OnClickCloseSubmenu();
        }
        else if (instructionsOpen)
        {
            OnClickCloseSubmenu();
        }
        else if (menuOpen && started)
        {
            OnClickResume();
        }
        else if (started)
        {
            OnClickPause();
        }
    }

    public void OnClickPause()
    {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        mainMenuPanel.SetActive(true);
        menuOpen = true;
    }

    public void OnClickResume()
    {
        Time.timeScale = oldTimeScale;
        mainMenuPanel.SetActive(false);
        menuOpen = false;
    }

    public void OnClickInstructions()
    {
        instructionsPanel.SetActive(true);
        instructionsOpen = true;
    }

    public void OnClickCredits()
    {
        creditsPanel.SetActive(true);
        creditsOpen = true;
    }

    public void OnClickCloseSubmenu()
    {
        mainMenuPanel.SetActive(true);
        
        instructionsPanel.SetActive(false);
        instructionsOpen = false;
        
        creditsPanel.SetActive(false);
        creditsOpen = false;
        
        gameOverPanel.SetActive(false);
        gameOverOpen = false;
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
