using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CharController : MonoBehaviour
{
    [SerializeField] protected float jumpHeight;
    [SerializeField] protected float groundCheckDistance;

    public bool isJumping = false;

    protected Rigidbody rig = null;
    protected Collider col = null;
    private Animator animator;

    private Vector3 facingDirection = Vector3.zero;

    public bool IsGrounded()
    {
        float colliderBottom = col.bounds.extents.y;

        Debug.DrawRay(transform.position, -transform.up * (colliderBottom + groundCheckDistance), Color.green);
        return Physics.Raycast(transform.position, -transform.up, colliderBottom + groundCheckDistance);
    }

    public void ReinitializeAnimator()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        rig = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();

        if (rig == null || col == null)
        {
            Debug.LogWarning("Character is missing one or more required components!", gameObject);
            enabled = false;
        }

        rig.freezeRotation = true;
        facingDirection = transform.forward;
    }

    public void DoJump()
    {
        if (IsGrounded()) isJumping = true;
    }

    public void Move(float xAxis, float zAxis)
    {
        Vector3 direction = new Vector3(xAxis, 0, zAxis);
        Move(direction);
    }

    public void Move(float xAxis, float zAxis, float moveSpeed)
    {
        Move(xAxis * moveSpeed, zAxis * moveSpeed);
    }

    public void Move(Vector2 direction, float moveSpeed)
    {
        Move(direction.x, direction.y, moveSpeed);
    }
    
    public void Move(Vector3 direction, float moveSpeed)
    {
        Move(direction * moveSpeed);
    }

    void FixedUpdate()
    {
        if (!isJumping) return;
        
        if (IsGrounded())
        {
            animator?.SetTrigger("Jump");
            rig.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        }

        isJumping = false;
    }

    public void Move(Vector3 direction)
    {
        float xAxis = direction.x;
        float yAxis = direction.y == 0 ? rig.velocity.y : direction.y;
        float zAxis = direction.z;

        bool isMoving = xAxis != 0 || zAxis != 0;
        animator?.SetBool("Moving", isMoving);
        
        

        rig.velocity = new Vector3(xAxis, yAxis, zAxis);

        if(rig.velocity.magnitude >= 1.0f)
        {
            
            if (rig.velocity.x != 0 || rig.velocity.z != 0)
            {
                facingDirection = new Vector3(rig.velocity.x, rig.velocity.y, rig.velocity.z);
            }
        }

        transform.forward = facingDirection;
    }

    public void Pause()
    {
        rig.velocity = Vector3.zero;
        animator?.SetBool("Moving", false);
    }

}

