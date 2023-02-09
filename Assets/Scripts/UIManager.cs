using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private GameEvents gameEvents;
    
    [SerializeField] private RectTransform foodBar;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform ammoBar;
    
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;
    
    private const int BarMaxSize = 675;
    
    private readonly Color fullAlpha = new Color(1f, 1f, 1f, 0f);
    private readonly Color noAlpha = new Color(1f, 1f, 1f, 1f);

    private string lastMessage;
    private float lastMessageLength;
    private float messagetimer;
   
    
    #region Unity Lifecycle
    private void OnEnable()
    {
        gameEvents.UpdateUIEvent.RegisterListener(OnUpdateUI);
       gameEvents.MessageEvent.RegisterListener(OnMessage);
    }

    private void OnDisable()
    {
        gameEvents.UpdateUIEvent.UnregisterListener(OnUpdateUI);
        gameEvents.MessageEvent.UnregisterListener(OnMessage);
    }

    private void Start()
    {
        
    }
    
    
    private void Update()
    {
        if (messagetimer < lastMessageLength) messageText.text = lastMessage;
        else messageText.text = string.Empty;
        messagePanel.SetActive(messageText.text != string.Empty);
        
        healthBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            (gameData.Health / GameData.HealthMax) * BarMaxSize);
        foodBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            (gameData.Food / GameData.FoodMax) * BarMaxSize);
        ammoBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            ((float)gameData.Ammo / GameData.AmmoMax) * BarMaxSize);
        

        messagetimer += Time.deltaTime;
    }
    
    #endregion

    #region Events

    void OnMessage(string text, float duration)
    {
        lastMessage = text;
        lastMessageLength = duration;
        messagetimer = 0;
    }
    
    private void OnUpdateUI()
    {

    }

    #endregion
    
}
