using UnityEngine;
using Utils;

namespace GameLogic.Entity.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Execution), typeof(Collider2D), typeof(PlayerDash))]
    [RequireComponent(typeof(PlayerMove), typeof(PlayerJump), typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer), typeof(PlayerInputManager))]
    public class Player : SingleMono<Player>
    {
        public Execution execution;
        public Collider2D selfCollider;
        public PlayerDash playerDash;
        public PlayerMove playerMove;
        public PlayerJump playerJump;
        public Rigidbody2D rb;
        public SpriteRenderer spriteRenderer;
        public PlayerInputManager playerInputManager;
        
        [ContextMenu("Collect All Required Components")]
        public void CollectAllRequiredComponents()
        {
            execution = GetComponent<Execution>();
            selfCollider = GetComponent<Collider2D>();
            playerDash = GetComponent<PlayerDash>();
            playerMove = GetComponent<PlayerMove>();
            playerJump = GetComponent<PlayerJump>();
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerInputManager = GetComponent<PlayerInputManager>();
        }
    }
}
