using UnityEngine;

public class VERN_COIL : MeleeEnemy
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EnemyAttackHitbox attackHitbox;
    [SerializeField] private Transform attackPoint;
    private bool _facingRight = true;

    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!isSpawned || !Target)
            return;
        
        float diffX = Target.position.x - transform.position.x;
        UpdateFacing(diffX);

        if (IsTargetInRange())
        {
            rb.linearVelocityX = 0f;

            if (CanAtk())
                Atk();
        }
        else
        {
            float dirX = Mathf.Sign(diffX);
            rb.linearVelocityX = dirX * moveSpeed;
        }
    }

    protected override void Atk()
    {
        ResetAtkDelay();

        //TODO animator.SetTrigger("Attack");
        EnableAttackHitbox();
        Invoke(nameof(DisableAttackHitbox), 0.12f);
    }
    
    public void EnableAttackHitbox()
    {
        attackHitbox.transform.position = attackPoint.position;
        attackHitbox.Activate(atkPower);
    }

    public void DisableAttackHitbox()
    {
        attackHitbox.Deactivate();
    }
    
    private void UpdateFacing(float diffX)
    {
        if (diffX > 0.05f)
            _facingRight = false;
        else if (diffX < -0.05f)
            _facingRight = true;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (_facingRight ? 1f : -1f);
        transform.localScale = scale;
    }
}
