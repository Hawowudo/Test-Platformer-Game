using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHit", menuName = "ActionStateLogic/PlayerHit", order = 1)]
public class PlayerHit : ActionStateLogic
{
    public float hitDuration = 0.5f;
    public override void OnEnterState(CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);
        actionHandler.GetComponent<Animator>().Play(_animationClip.name);
        actionHandler._blackboard.Set<float>("hittimer", 0);

        actionHandler._rigidBody2D.gravityScale = actionHandler._blackboard.Get<float>("originalgravity");
    }
    public override void FrameUpdate(CharacterActionHandler actionHandler)
    {

        base.FrameUpdate(actionHandler);
        actionHandler._blackboard.Set<float>("hittimer", actionHandler._blackboard.Get<float>("hittimer") + Time.deltaTime);
        if (actionHandler._blackboard.Get<float>("hittimer") >= hitDuration)
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerIdle>());
        }

        if (actionHandler._blackboard.Get<bool>("isdead"))
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerDeath>());
        }
    }
    public override void OnExitState(CharacterActionHandler actionHandler)
    {
        base.OnExitState(actionHandler);
        actionHandler._blackboard.Set<bool>("hittrigger", false);
    }
    public override void PhysicsUpdate(CharacterActionHandler actionHandler)
    {
        base.PhysicsUpdate(actionHandler);
    }
}
