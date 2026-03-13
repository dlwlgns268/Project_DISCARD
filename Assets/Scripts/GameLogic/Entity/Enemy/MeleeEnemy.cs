using UnityEngine;

public abstract class MeleeEnemy : Enemy
{
    [SerializeField] protected float atkRange = 1.5f;
    [SerializeField] protected float atkDelay = 1f;
    
    protected float CurrentAtkDelay;

    protected virtual void FixedUpdate()
    {
        if(CurrentAtkDelay > 0f)
            CurrentAtkDelay -= Time.fixedDeltaTime;
    }
    
    protected bool CanAtk()
    {
        return CurrentAtkDelay <= 0f;
    }

    protected bool IsTargetInRange()
    {
        if (!Target) return false;
        return Vector2.Distance(transform.position, Target.position) <= atkRange;
    }

    protected void ResetAtkDelay()
    {
        CurrentAtkDelay = atkDelay;
    }

    protected abstract void Atk();
}
