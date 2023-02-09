using UnityEngine;

[RequireComponent(typeof(CharController))]
public class Player : MonoBehaviour
{
    private bool locked;
    private CharController charController;

    
    [SerializeField] private GameData gameData;
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private float speed = 7f;
    private void OnEnable()
    {
        gameEvents.MovePlayerEvent.RegisterListener(OnPlayerMove);
        gameEvents.StopCameraEvent.RegisterListener(OnCameraStop);
    }

    private void OnDisable()
    {
        gameEvents.MovePlayerEvent.UnregisterListener(OnPlayerMove);
        gameEvents.StopCameraEvent.UnregisterListener(OnCameraStop);
    }

    private void OnPlayerMove(Vector2 direction)
    {
        LockMovement(true);
        var newDirection = new Vector3(direction.x, 0, direction.y).normalized;
        charController.Move(newDirection, speed * 1.5f);
        //Debug.Log("player move " + newDirection);
    }

    private void OnCameraStop(Vector2 direction)
    {
        LockMovement(false);
    }


    private void LockMovement(bool lockMovement)  {
        locked = lockMovement;
        if(locked) charController.Pause();
    }

    private void Awake()
    {
        charController = GetComponent<CharController>();
    }

    private void Update()
    {
        if (locked) return;
        
        float xAxis = Input.GetAxisRaw("Horizontal");
        float zAxis = Input.GetAxisRaw("Vertical");

        charController.Move(xAxis, zAxis, speed);
        if(Input.GetKeyDown(KeyCode.Space)) charController.DoJump();
    }

    public void TakeDamage(float damage)
    {
        gameData.Health -= damage;
    }
}
