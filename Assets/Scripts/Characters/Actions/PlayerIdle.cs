using UnityEngine;

[CreateAssetMenu(fileName = "PlayerIdle", menuName = "ActionStateLogic/PlayerIdleASL", order = 1)]
public class PlayerIdle : ActionStateLogic
{
    public override void OnEnterState(CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);
        actionHandler.GetComponent<Animator>().Play(_animationClip.name);
        if(actionHandler._blackboard.Get<float>("originalgravity") == 0)
        {
            actionHandler._blackboard.Set<float>("originalgravity", actionHandler._rigidBody2D.gravityScale);  
        }
        actionHandler._rigidBody2D.gravityScale = actionHandler._blackboard.Get<float>("originalgravity");
        actionHandler._rigidBody2D.velocity = new Vector2(0, actionHandler._rigidBody2D.velocity.y);    
    }
    public override void FrameUpdate(CharacterActionHandler actionHandler)
    {
        base.FrameUpdate(actionHandler);
        if (actionHandler._blackboard.Get<bool>("attackpressed"))
        {
            actionHandler.ChangeState(actionHandler.GetState< PlayerAttack>());
            return;
        }

        if (actionHandler._blackboard.Get<Vector2>("moveinput").x != 0)
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerRun>());
            return;
        }

        if (actionHandler._blackboard.Get<bool>("hittrigger"))
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerHit>());
            return;
        }

        if (actionHandler._blackboard.Get<bool>("jumppressed"))
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerJump>());
            return;
        }
        if (actionHandler._rigidBody2D.velocity.y < -1f && !GroundCheck(actionHandler))
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
    }
    public bool GroundCheck(CharacterActionHandler actionHandler)
    {
        Vector2 position = actionHandler.transform.position;
        Vector2 size = actionHandler.GetComponent<CapsuleCollider2D>().size;
        Vector2 offset = actionHandler.GetComponent<CapsuleCollider2D>().offset;
        LayerMask layerMask = LayerMask.GetMask("Default");
        RaycastHit2D hit = Physics2D.BoxCast(position + offset, size, 0f, Vector2.down, 0.2f, layerMask);
        return hit.collider != null;
    }
}
