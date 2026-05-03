using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRun", menuName = "ActionStateLogic/PlayerRun", order = 1)]
public class PlayerRun : ActionStateLogic
{
    public override void OnEnterState(CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);
        actionHandler.GetComponent<Animator>().Play(_animationClip.name);
        actionHandler._rigidBody2D.gravityScale = actionHandler._blackboard.Get<float>("originalgravity");
    }
    public override void FrameUpdate(CharacterActionHandler actionHandler)
    {
        base.FrameUpdate(actionHandler);
        if (actionHandler._blackboard.Get<bool>("attackpressed"))
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerAttack>());
            return;
        }
        if (actionHandler._blackboard.Get<Vector2>("moveinput").x == 0f)
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerIdle>());
            return;
        }
        if(actionHandler._blackboard.Get<bool>("hittrigger"))
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerHit>());
            return;
        }
        if (actionHandler._blackboard.Get<bool>("jumppressed"))
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerJump>());
            return;
        }
        if (actionHandler._rigidBody2D.velocity.y < -0.5f)
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerFall>());
        }

    }
    public override void OnExitState(CharacterActionHandler actionHandler)
    {
        base.OnExitState(actionHandler);
    }
    public override void PhysicsUpdate(CharacterActionHandler actionHandler)
    {
        base.PhysicsUpdate(actionHandler);
        CalculateMovement(actionHandler);
    }
    private void CalculateMovement(CharacterActionHandler actionHandler)
    {

        Vector2 moveinput = actionHandler._blackboard.Get<Vector2>("moveinput");
        float moveSpeed = actionHandler._blackboard.Get<float>("movespeed");
        actionHandler.GetComponent<SpriteRenderer>().flipX = actionHandler.GetSign(actionHandler._blackboard.Get<Vector2>("moveinput")) < 0;
        actionHandler._rigidBody2D.velocity = new Vector2((moveinput.x == 0 ? 0 : (moveinput.x > 0 ? 1 : -1)) * moveSpeed, actionHandler._rigidBody2D.velocity.y);
    }
}
