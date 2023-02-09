using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data", order = 51)]
public class GameData : ScriptableObject
{
    [SerializeField] private GameEvent updateUIEvent;

    public static readonly Vector2 FirstBiome = new Vector2(33f, 33f);
    public static readonly Vector2 PlayerStart = new Vector2(37f, 37f);

    public const int BiomeSizeX = 30;
    public const int BiomeSizeY = 20;
    
    public const int FoodMax = 10;
    public const float FoodDecayRate = .25f;
    
    public const int HealthMax = 10;
    public const float HealthDecayRate = .1f;
    
    public const int AmmoMax = 10;

    public const float SpawnRate = 3f;
    public const int SpawnMax = 6;
    
    public const float PickupExpireTime = 5f;
    
    private float health;
    public float Health
    {
        get => health;
        set
        {
            health = value;
            if (health > HealthMax) health = HealthMax;
            if (health < 0) health = 0;
            
            updateUIEvent?.Raise();
        }
    }

    private float food;
    public float Food
    {
        get => food;
        set
        {
            food = value;
            if (food > FoodMax) food = FoodMax;
            if (food < 0) food = 0;
            
            updateUIEvent?.Raise();
        }
    }
    
    private int ammo;
    public int Ammo
    {
        get => ammo;
        set
        {
            ammo = value;
            if (ammo > AmmoMax) ammo = AmmoMax;
            if (ammo < 0) ammo = 0;
            
            updateUIEvent?.Raise();
        }
    }

    public void ResetStats()
    {
        health = HealthMax;
        food = FoodMax;
        ammo = AmmoMax;
        
        updateUIEvent?.Raise();
    }
}
