using UnityEngine;

public class VernTurret : RangedEnemy
{
    private bool _facingRight = true;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (!isSpawned || !Target)
            return;
        
        float diffX = Target.position.x - transform.position.x;
        UpdateFacing(diffX);

        if (IsTargetInRange() && CanAtk())
        {
            Atk();
        }
    }

    private void Atk()
    {
        ResetAttackDelay();
        
        //TODO 얘도 공격 모션이나 연출 넣기
        FireProjectile();
    }
    
    private void UpdateFacing(float diffX)
    {
        if (diffX > 0.05f)
            _facingRight = true;
        else if (diffX < -0.05f)
            _facingRight = false;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (_facingRight ? 1f : -1f);
        transform.localScale = scale;
    }
}
