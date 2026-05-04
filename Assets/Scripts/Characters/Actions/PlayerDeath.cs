using CombatSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDeath", menuName = "ActionStateLogic/PlayerDeath", order = 1)]
public class PlayerDeath : ActionStateLogic
{
    private float _deathDuration;
    public float _despawnDelay = 2f;

    public override void OnEnterState(CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);
        actionHandler.GetComponent<Animator>().Play(_animationClip.name);
        _deathDuration = _animationClip.length;
        actionHandler._rigidBody2D.velocity = Vector2.zero;
        actionHandler._blackboardTimer.AddTimerToBlackboard("deathanimtimer",.0f);
        actionHandler._blackboardTimer.AddTimerToBlackboard("deathspritedisabletimer",.0f);
        actionHandler.GetComponent<CombatEntity>().DisableHitbox();
        actionHandler.GetComponent<CombatEntity>().DisableHurtBox();
        DisableCollision(actionHandler);
    }
    public override void FrameUpdate(CharacterActionHandler actionHandler)
    {
        base.FrameUpdate(actionHandler);
        if (!actionHandler.gameObject.activeSelf)
            return;
        if (actionHandler._blackboard.Get<float>("deathanimtimer") > _deathDuration)
        {
            actionHandler._blackboard.Set<bool>("deathanimfinished", true);
        }
        if (actionHandler._blackboard.Get<bool>("deathanimfinished"))
        {
            if (actionHandler._blackboard.Get<float>("deathspritedisabletimer") > _despawnDelay)
            {
                actionHandler._blackboardTimer.StopTimer("deathspritedisabletimer");
                actionHandler._blackboard.Set<float>("deathspritedisabletimer",0);
                actionHandler.gameObject.SetActive(false);
            }
        }

    }
    public override void OnExitState(CharacterActionHandler actionHandler)
    {
        base.OnExitState(actionHandler);
        actionHandler._blackboard.Set<bool>("deathanimfinished", false);
        actionHandler._blackboard.Set<bool>("isdead", false);
        EnablCollision(actionHandler);
    }
    public override void PhysicsUpdate(CharacterActionHandler actionHandler)
    {
        base.PhysicsUpdate(actionHandler);
    }
    public void DisableCollision(CharacterActionHandler actionHandler)
    {
        actionHandler.GetComponent<Collider2D>().isTrigger = true;
        actionHandler._rigidBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    public void EnablCollision(CharacterActionHandler actionHandler)
    {
        actionHandler.GetComponent<Collider2D>().isTrigger = false;
        actionHandler._rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
