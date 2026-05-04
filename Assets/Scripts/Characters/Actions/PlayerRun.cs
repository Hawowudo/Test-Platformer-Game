using AudioManagerPackage;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRun", menuName = "ActionStateLogic/PlayerRun", order = 1)]
public class PlayerRun : ActionStateLogic
{
    public float footstepAudioTimer = 1f;
    public override void OnEnterState(CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);
        actionHandler.GetComponent<Animator>().Play(_animationClip.name);
        actionHandler._rigidBody2D.gravityScale = actionHandler._blackboard.Get<float>("originalgravity");
        actionHandler._blackboardTimer.AddTimerToBlackboard("footsteptimer",0f);
        PlayFootstep(actionHandler);
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
        if (actionHandler._rigidBody2D.velocity.y < -1f)
        {
            if (!GroundCheck(actionHandler))
                actionHandler.ChangeState(actionHandler.GetState<PlayerFall>());
        }
        if (actionHandler._blackboard.Get<float>("footsteptimer") > footstepAudioTimer)
        {
            actionHandler._blackboard.Set<float>("footsteptimer", 0f);
            actionHandler._blackboard.Set<bool>("playfootstep", true);
        }
    }
    public override void OnExitState(CharacterActionHandler actionHandler)
    {
        base.OnExitState(actionHandler);


        actionHandler._blackboard.Set<bool>("playfootstep", false);
        actionHandler._blackboardTimer.StopTimer("footsteptimer");

    }
    public override void PhysicsUpdate(CharacterActionHandler actionHandler)
    {
        base.PhysicsUpdate(actionHandler);
        CalculateMovement(actionHandler);

        if (actionHandler._blackboard.Get<bool>("playfootstep") == true)
        {
            actionHandler._blackboard.Set<bool>("playfootstep", false);
            PlayFootstep(actionHandler);
        }
    }
    private void CalculateMovement(CharacterActionHandler actionHandler)
    {

        Vector2 moveinput = actionHandler._blackboard.Get<Vector2>("moveinput");
        float moveSpeed = actionHandler._blackboard.Get<float>("movespeed");
        actionHandler.FlipSprite(actionHandler.GetSign(actionHandler._blackboard.Get<Vector2>("moveinput")));
        actionHandler._rigidBody2D.velocity = new Vector2((moveinput.x == 0 ? 0 : (moveinput.x > 0 ? 1 : -1)) * moveSpeed, actionHandler._rigidBody2D.velocity.y);
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
    private void PlayFootstep(CharacterActionHandler actionHandler)
    {
        SoundClipInfo newSoundClipWithPosition = _actionAudio.DeepCopy();
        newSoundClipWithPosition.position = actionHandler.transform.position;
        AudioManager.instance.PlayAudioClipInstance(newSoundClipWithPosition);
    }
}
