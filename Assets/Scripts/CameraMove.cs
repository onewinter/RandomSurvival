using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private GameEvents gameEvents;
    
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private Vector3 offset;
    
    private float speed = 1.25f;
    private float timer;
    private bool moved;
    
    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition;
        offset = targetPosition - Vector3.zero; // find out camera's initial offset from zero
    }

    private void OnEnable()
    {
      gameEvents.MoveCameraEvent.RegisterListener(OnMoveCameraEvent);
    }

    private void OnDisable()
    {
        gameEvents.MoveCameraEvent.UnregisterListener(OnMoveCameraEvent);
    }


    void OnMoveCameraEvent(Vector2 direction)
    {
        //Debug.Log("OnCameraMoveEvent " + direction);
        startPosition = targetPosition;
        timer = 0f;
        moved = true;
        targetPosition = new Vector3(direction.x, 0, direction.y) + offset;
    }


    void Update()
    {
       
        if (transform.position == targetPosition)
        {
            //if(moved) Debug.Log(("camera stop"));
            if(moved) gameEvents.StopCameraEvent.Raise(new Vector2(transform.position.x, transform.position.z));
            moved = false;
        }
        else
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timer / speed);
            timer += Time.deltaTime;
        }
    }
}
