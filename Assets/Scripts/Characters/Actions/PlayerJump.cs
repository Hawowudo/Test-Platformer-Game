using AudioManagerPackage;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerJump", menuName = "ActionStateLogic/PlayerJump")]
public class PlayerJump : ActionStateLogic
{
    public float jumpForce = 10f;

    [Header("Jump Feel")]
    public float normalGravity = 3f;
    public float earlyReleaseGravityMultiplier = 2f;


    public override void OnEnterState(CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);

        actionHandler.GetComponent<Animator>().Play(_animationClip.name);

        var rb = actionHandler._rigidBody2D;

        actionHandler._blackboard.Set("originalGravity",rb.gravityScale);

        rb.velocity = new Vector2( rb.velocity.x,jumpForce);

        rb.gravityScale = normalGravity;

        PlayAudio(actionHandler);
    }


    public override void FrameUpdate(CharacterActionHandler actionHandler)
    {
        base.FrameUpdate(actionHandler);

        if (actionHandler._blackboard.Get<bool>("hittrigger"))
        {
            actionHandler.ChangeState(
                actionHandler.GetState<PlayerHit>());
            return;
        }

        if (CeilingCheck(actionHandler))
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerFall>());
            return;
        }

        if (actionHandler._rigidBody2D.velocity.y <= 0f)
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerFall>());
        }

        if (!actionHandler._blackboard.Get<bool>("jumppressed"))
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerFall>());
        }
    }


    public override void PhysicsUpdate(
        CharacterActionHandler actionHandler)
    {
        base.PhysicsUpdate(actionHandler);

        var rb = actionHandler._rigidBody2D;

        bool jumpHeld = actionHandler._blackboard.Get<bool>("jumppressed");

        if (!jumpHeld && rb.velocity.y > 0f)
        {
            rb.gravityScale = normalGravity *  earlyReleaseGravityMultiplier;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
        CalculateMovement(actionHandler);
    }


    public override void OnExitState(
        CharacterActionHandler actionHandler)
    {
        base.OnExitState(actionHandler);

        actionHandler._rigidBody2D.gravityScale = actionHandler._blackboard.Get<float>("originalGravity");

        actionHandler._blackboard.Set<bool>("jumppressed", false);
    }


    private void CalculateMovement(CharacterActionHandler actionHandler)
    {

        Vector2 moveinput = actionHandler._blackboard.Get<Vector2>("moveinput");
        float moveSpeed = actionHandler._blackboard.Get<float>("movespeed");
        if (moveinput.x != 0)
        {
            actionHandler.GetComponent<SpriteRenderer>().flipX = actionHandler.GetSign(actionHandler._blackboard.Get<Vector2>("moveinput")) < 0;
        }
        actionHandler._rigidBody2D.velocity = new Vector2(moveinput.x * moveSpeed, actionHandler._rigidBody2D.velocity.y);
    }
    private bool CeilingCheck(
        CharacterActionHandler actionHandler)
    {
        CapsuleCollider2D capsule =
            actionHandler.GetComponent<CapsuleCollider2D>();

        Vector2 origin =
            capsule.bounds.center +
            Vector3.up * capsule.bounds.extents.y;

        Vector2 size =
            new Vector2(
                capsule.bounds.size.x * 0.9f,
                0.05f);

        return Physics2D.BoxCast(
            origin,
            size,
            0f,
            Vector2.up,
            0.05f,
            LayerMask.GetMask("Ground"));
    }
}