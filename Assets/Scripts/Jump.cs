using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Jump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckBoxSize = new Vector2(0.5f, 0.1f);
    
    private bool _isGrounded;
    private bool _jumpRequested;

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            _jumpRequested = true;
        }
    }

    private void Update()
    {
        _isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0f, groundLayer);
        
        if (_jumpRequested && !_isGrounded && rb.linearVelocity.y < 0)
        {
            _jumpRequested = false;
        }
    }

    private void FixedUpdate()
    {
        if (_jumpRequested && _isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            _jumpRequested = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckBoxSize);
        }
    }
}
