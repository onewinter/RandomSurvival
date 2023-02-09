using System.Collections;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    
    [SerializeField] private float swingDamage = 10f;
    [SerializeField] private float swingDelay = .5f;
    [SerializeField] private float swingRadius = 1f;
    
    [SerializeField] private float shotDelay = 1f;
    [SerializeField] private float shotDamage = 20f;
    [SerializeField] private float shotRange = 5f;
    [SerializeField] private float shotRadius = 3f;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletDuration = .25f;

    private GameObject bullet;
    
    private float attackTimer;
    private Camera cameraMain;

    private Vector3 mousePosition;
    private Vector3 mouseDirection;
    private Animator animator;
    Plane plane = new Plane(Vector3.up, 0); // get mouse position trick
    
    void Start()
    {
        animator = transform.parent.GetComponentInChildren<Animator>();
        cameraMain = Camera.main;
    }
    
    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;

        // spin the player towards the mouse cursor
        Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out var distance))
        {
            mousePosition = ray.GetPoint(distance);
            mouseDirection = (mousePosition - transform.parent.position).normalized;
            
            var parentFacing = mouseDirection;
            parentFacing.y = 0f;
            transform.parent.forward = parentFacing;
        }
        
        // lmb melee attack
        if (Input.GetMouseButtonDown(0) && attackTimer > swingDelay)
        {
            attackTimer = 0;

            Collider[] hitColliders;
            hitColliders = Physics.OverlapSphere(transform.position, swingRadius, LayerMask.GetMask("Enemy"));

            //Debug.Log("swing");
            animator?.SetTrigger("Attack");

            foreach (var hitCollider in hitColliders)
            {
                var enemy = hitCollider.GetComponent<Enemy>();
                if (!enemy) continue;

                Debug.Log("enemy " + enemy + " hit for " + swingDamage);
                enemy.TakeDamage(swingDamage);
            }
        }
        // rmb shot
        else  if (Input.GetMouseButtonDown(1) && attackTimer > shotDelay && gameData.Ammo > 0)
        {
            //Debug.Log(gameData.Ammo);
            attackTimer = 0;
            gameData.Ammo--;
            //Debug.Log(gameData.Ammo);
         
            animator?.SetTrigger("Shoot");
            
            ray = new Ray(transform.position, mouseDirection);
            Debug.DrawRay(transform.position, mouseDirection, Color.red);

            Enemy enemy = null;
            
            if(Physics.SphereCast(ray, shotRadius, out var hit, shotRange, LayerMask.GetMask("Enemy")))
            {
                enemy = hit.collider.GetComponent<Enemy>();
            }

            StartCoroutine(ShootBullet(mousePosition, enemy));
        }
    }
    private IEnumerator ShootBullet(Vector3 targetPosition, Enemy enemy = null)
    {
        // the simplest of pooling since we always shoot slower than the bullet travels
        if(!bullet) bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<TrailRenderer>().Clear();
        bullet.SetActive(true);
        
        var bulletStart = transform.position;
        var timer = 0f;

        do
        {
            bullet.transform.position = Vector3.Lerp(bulletStart, targetPosition, timer / bulletDuration);
            if (enemy && (bullet.transform.position - enemy.transform.position).sqrMagnitude < .1f) yield break; // break coro if close enough to target
            timer += Time.deltaTime;
            yield return null;
        } while (bullet && (bullet.transform.position - bulletStart).sqrMagnitude < shotRange * shotRange && timer <= bulletDuration * 1.25f); // don't travel further than shotrange

        if(bullet) bullet.SetActive(false);
        enemy?.TakeDamage(shotDamage);
        
    }
    
}

