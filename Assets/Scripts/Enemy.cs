using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public enum EnemyState
{
    Attacking,
    Sleeping
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharController))]
public class Enemy : MonoBehaviour
{
    public EnemyState currentState = EnemyState.Sleeping;
    public float distanceToPlayer = 0.9f;
    public Player player = null;
    public float timer = 0.0f;
    public float dashTimer = 0.0f;
    public float pauseTimer = 0.0f;
    
    public Rigidbody rig = null;
    public Animator animator;
    public Vector3 moveDirection = Vector3.zero;
    public CharController charController;
    public Vector3 startingPosition;
    public Vector3 startingForward;
    public Material material = null;
    public Color originalColor = Color.white;

    public float MaxHealth => enemySetup.Health;
    public float Health;
    public Chunk ParentChunk;
    public IObjectPool<Enemy> ParentPool;

   [SerializeField] private bool paused = true;
    
    [SerializeField] private EnemyBrain enemyBrain;
    [SerializeField] private EnemySetup enemySetup;

    public void Paused(bool pause) => paused = pause;

    // called to setup a new enemy via pool
    public void Initialize(EnemySetup setup)
    {
        enemySetup = setup;
        Health = enemySetup.Health;
        enemyBrain = enemySetup.EnemyBrain;
        var model = Instantiate(enemySetup.ModelPrefab, transform);
        model.name = "Model";
        
        enemyBrain.OnStart(this);
        charController.ReinitializeAnimator();
        player = FindObjectOfType<Player>();
        StartCoroutine("SetDistanceToPlayer");
        paused = false;
    }

    public void Reset()
    {
        paused = true;
        var model = transform.Find("Model");
        Destroy(model.gameObject);
        StopAllCoroutines();
    }

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        charController = GetComponent<CharController>();
        material = GetComponent<Renderer>().material;
    }

    protected void Start()
    {
        player = FindObjectOfType<Player>();
        rig.freezeRotation = true;
    }

    private void Update()
    {
        if(Mathf.Abs(transform.position.y) > 25f) Recycle(); // weeeeeeeeee
        if (paused || !player) return;

        if (rig.velocity != Vector3.zero)
        {
            var facingDir = new Vector3(rig.velocity.x, transform.forward.y, rig.velocity.z); // when moving, always face in the direction we're moving
            if (facingDir != Vector3.zero) transform.forward = facingDir;
        }
        
        switch (currentState)
        {
            case EnemyState.Attacking:
                enemyBrain.DoAttackAction(this);
                break;
            case EnemyState.Sleeping:
                enemyBrain.DoSleepAction(this);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        timer += Time.deltaTime;
    }

    private IEnumerator SetDistanceToPlayer()
    {
        // only check distance to player every quarter sec
        while (true)
        {
            if(player) distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            yield return new WaitForSeconds(.25f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if(!player) return;
        
        player.TakeDamage(enemySetup.Damage);
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        
        if(Health<=0) OnDeath();
    }

    void OnDeath()
    {
        if (enemySetup.Loot.Count!=0 && Random.Range(0f, 1f) < enemySetup.LootChance)
        {
            Instantiate(enemySetup.Loot[Random.Range(0, enemySetup.Loot.Count)], transform.position,
                Quaternion.identity);
        }

        Recycle();
    }

    public void Recycle()
    {
        // try/catch in case we try to recycle the same enemy twice 
        try
        {
            ParentPool.Release(this);
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}
