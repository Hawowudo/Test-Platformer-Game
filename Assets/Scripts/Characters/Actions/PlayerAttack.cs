using CombatSystem;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "PlayerAttack", menuName = "ActionStateLogic/PlayerAttack", order = 1)]
public class PlayerAttack : ActionStateLogic
{
    public AnimationClip _animationClip2;
    public float attackqueuedelay = 0.3f;
    public float attackComboDelay = 0.8f;
    public override void OnEnterState(CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);
        SetupHitbox(actionHandler, actionHandler._blackboard.Get<Vector2>("previousmoveinput"));
        actionHandler.GetComponent<Animator>().Play(_animationClip.name);
        actionHandler._blackboard.Set("attackpressed", false);

    }
    public override void FrameUpdate(CharacterActionHandler actionHandler)
    {
        base.FrameUpdate(actionHandler);

        if (actionHandler._blackboard.Get<bool>("hittrigger"))
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerHit>());
            return;
        }

        if (actionHandler._blackboard.Get<Vector2>("moveinput") != Vector2.zero)
            actionHandler._blackboard.Set("attackqueued", false);

        if (actionHandler.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > attackqueuedelay
            && actionHandler._blackboard.Get<bool>("attackpressed")
            && !actionHandler._blackboard.Get<bool>("attackqueued")
            )
            {
                actionHandler._blackboard.Set("attackqueued", true);
            }

        if (actionHandler.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < attackComboDelay)
        {
            return;
        }

        if (actionHandler._blackboard.Get<Vector2>("moveinput") != Vector2.zero)
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerRun>());
        }

        if (
            actionHandler.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
            )
        {
            actionHandler.ChangeState(actionHandler.GetState< PlayerIdle>( ));
            return;
        }

        if (actionHandler._blackboard.Get<bool>("attackqueued"))
        {
            actionHandler._blackboard.Set("attackqueued", false);
            actionHandler.GetComponent<Animator>().Play(GetOtherAnimationName(actionHandler));
        }
    }
    public override void OnExitState(CharacterActionHandler actionHandler)
    {
        base.OnExitState(actionHandler);
        actionHandler._blackboard.Set("attackpressed", false);
        actionHandler._blackboard.Set("attackqueued", false);
        actionHandler.GetComponentInChildren<CombatEntity>().hitboxHandler.DisableAllHitbox();
    }
    public override void PhysicsUpdate(CharacterActionHandler actionHandler)
    {
        base.PhysicsUpdate(actionHandler);
    }
    // TODO: Add combo turning
    // This is a bit messy but it works ¯\_(ツ)_/¯ 
    public string GetOtherAnimationName(CharacterActionHandler actionHandler)
    {
        var currentAnim = actionHandler.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(_animationClip.name) ? _animationClip.name : _animationClip2.name;
        return currentAnim == _animationClip.name ? _animationClip2.name : _animationClip.name;
    }
    private void SetupHitbox(CharacterActionHandler actionHandler, Vector2 direction)
    {
        actionHandler.GetComponentInChildren<CombatEntity>().hitboxHandler.SetDirection(direction.x >= 0 ? 1 : -1);
    }
}
