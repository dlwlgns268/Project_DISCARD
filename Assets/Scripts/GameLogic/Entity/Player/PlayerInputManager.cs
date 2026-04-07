using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLogic.Entity.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public bool JumpRequest { get; set; }
        public bool DashRequest { get; set; }
        public bool AttackRequest { get; set; }
    
        public void OnMove(InputValue value)
        {
            MoveInput = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            JumpRequest = true;
        }
        
        public void OnDash(InputValue value)
        {
            DashRequest = true;
        }
        
        public void OnAttack(InputValue value)
        {
            AttackRequest = true;
        }
    }
}
