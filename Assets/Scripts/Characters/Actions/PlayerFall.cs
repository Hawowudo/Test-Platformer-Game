using UnityEngine;
using InputManagerScripts;

[CreateAssetMenu(fileName = "PlayerFall", menuName = "ActionStateLogic/PlayerFall")]
public class PlayerFall : ActionStateLogic
{
    [Header("Fall Feel")]
    public float fallGravity = 3f;
    public float fallGravityMultiplier = 2f;
    public float coyoteTime = .5f;
    public override void OnEnterState(
        CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);

        actionHandler.GetComponent<Animator>().Play(_animationClip.name);

        Rigidbody2D rb = actionHandler._rigidBody2D;

        actionHandler._blackboard.Set<float>("originalGravity", rb.gravityScale);
        rb.velocity = Vector2.zero;
        rb.gravityScale = fallGravity * fallGravityMultiplier;

        if( !actionHandler.CheckPreviousState(actionHandler.GetState<PlayerJump>()) )
        {
            actionHandler._blackboard.Set<bool>("cancoyotetime", true);
            actionHandler._blackboardTimer.AddTimerToBlackboard("coyotetime", 0f);
        }
    }


    public override void FrameUpdate(
        CharacterActionHandler actionHandler)
    {
        base.FrameUpdate(actionHandler);

        if (actionHandler._blackboard.Get<bool>("hittrigger"))
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerHit>());
            return;
        }
        CalculateMovement(actionHandler);

        if ( actionHandler._blackboard.Get<float>("coyotetime") < coyoteTime 
            && actionHandler._blackboard.Get<bool>("jumppressed")
            && actionHandler._blackboard.Get<bool>("cancoyotetime")
            )
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerJump>());
            return;
        }
        if (Time.frameCount % 30 == 0)
        {
            if (!GroundCheck(actionHandler))
            {
                return;
            }



            bool isMoving = Mathf.Abs(actionHandler._blackboard.Get<Vector2>("moveinput").x) > 0.1f;

            if (isMoving)
            {
                actionHandler.ChangeState( actionHandler.GetState<PlayerRun>());
            }
            else
            {
                actionHandler.ChangeState( actionHandler.GetState<PlayerIdle>());
            }

            return;
        }
    }


    public override void OnExitState(
        CharacterActionHandler actionHandler)
    {
        base.OnExitState(actionHandler);

        actionHandler._rigidBody2D.gravityScale = actionHandler._blackboard.Get<float>("originalGravity");
        actionHandler._blackboard.Set<bool>("cancoyotetime", false);
        actionHandler._blackboardTimer.StopTimer("coyotetime");
        PlayAudio(actionHandler);
    }


    private void CalculateMovement(CharacterActionHandler actionHandler)
    {

        Vector2 moveinput = actionHandler._blackboard.Get<Vector2>("moveinput");
        float moveSpeed = actionHandler._blackboard.Get<float>("movespeed");
        if(moveinput.x != 0)
        {
            actionHandler.FlipSprite(actionHandler.GetSign(actionHandler._blackboard.Get<Vector2>("moveinput")));
        }
        actionHandler._rigidBody2D.velocity = new Vector2(moveinput.x * moveSpeed, actionHandler._rigidBody2D.velocity.y);
    }
    public bool GroundCheck(CharacterActionHandler actionHandler) 
    { 
        Vector2 position = actionHandler.transform.position; 
        Vector2 size = actionHandler.GetComponent<CapsuleCollider2D>().size; 
        Vector2 offset = actionHandler.GetComponent<CapsuleCollider2D>().offset; 
        LayerMask layerMask = LayerMask.GetMask("Default"); 
        RaycastHit2D hit = Physics2D.BoxCast(position + offset, size, 0f, Vector2.down, 0.2f, layerMask);
        DrawDebugBox(position + offset, size); 
        return hit.collider != null; 
    }


    private void DrawDebugBox(
        Vector2 origin,
        Vector2 size)
    {
        Vector2 half = size * .5f;

        Vector2 tl =
            origin +
            new Vector2(-half.x, half.y);

        Vector2 tr =
            origin +
            new Vector2(half.x, half.y);

        Vector2 bl =
            origin +
            new Vector2(-half.x, -half.y);

        Vector2 br =
            origin +
            new Vector2(half.x, -half.y);

        Debug.DrawLine(tl, tr, Color.green);
        Debug.DrawLine(tr, br, Color.green);
        Debug.DrawLine(br, bl, Color.green);
        Debug.DrawLine(bl, tl, Color.green);
    }
}