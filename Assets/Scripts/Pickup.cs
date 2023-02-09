using System;
using UnityEngine;

public enum PickupTypes{Food, Health, Ammo}

public class Pickup : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private GameEvents gameEvents;
    
    [SerializeField] private PickupTypes pickupType;
    [SerializeField] private int amount;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (!player) return;

        string message = "Received " + amount;
        
        switch(pickupType)
        {
            case PickupTypes.Food:
                gameData.Food += amount;
                message += " Food.";
                break;
            case PickupTypes.Health:
                gameData.Health += amount;
                message += " Health.";
                break;
            case PickupTypes.Ammo:
                gameData.Ammo += amount;
                message += " Ammo.";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        gameEvents.MessageEvent.Raise(message, 1f);
        
        Destroy(gameObject);
    }

    void Start()
    {
        Destroy(gameObject, GameData.PickupExpireTime); // pickups expire after N secs
    }

}
