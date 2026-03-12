using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    private float _moveInput;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    

    [SerializeField] private Dash dash;
    
    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>().x;
    }
    
    private void FixedUpdate()
    {
        if (dash != null && dash.IsDashing) return;
        
        rb.linearVelocityX = _moveInput * moveSpeed;
        
        if (_moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (_moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}