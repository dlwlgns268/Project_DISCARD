using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    private float _moveInput;
    [SerializeField] private Rigidbody2D _rb;
    

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>().x;
    }
    
    private void FixedUpdate()
    {
        _rb.linearVelocityX = _moveInput * moveSpeed;
    }
}