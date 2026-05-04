using AudioManagerPackage;
using CombatSystem;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "PlayerAttack", menuName = "ActionStateLogic/PlayerAttack", order = 1)]
public class PlayerAttack : ActionStateLogic
{
    private const string ATTACK_INDEX_KEY = "attackindex";
    private const string ATTACK_COMBO_TIMER_KEY = "attackcombotimer";
    private const string ATTACK_PRESSED_KEY = "attackpressed";
    private const string ATTACK_QUEUED_KEY = "attackqueued";
    private const string HIT_TRIGGER_KEY = "hittrigger";
    private const string MOVE_INPUT_KEY = "moveinput";

    public AnimationClip _animationClip2;
    public float attackqueuedelay = 0.3f;
    public float attackComboDelay = 0.8f;
    public float attackComboBaseCooldown = 1f;
    public override void OnEnterState(CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);

        if (!actionHandler._blackboard.HasKey(ATTACK_COMBO_TIMER_KEY))
            actionHandler._blackboardTimer.AddTimerToBlackboard(ATTACK_COMBO_TIMER_KEY);
        
        //2x animation duration if not player
        actionHandler.GetComponent<Animator>().speed = actionHandler.GetComponent<CombatEntity>().team == Team.Player ? 1f : 0.5f;


        SetupHitbox(actionHandler);
        Attack(actionHandler);
        actionHandler._blackboard.Set(ATTACK_PRESSED_KEY, false);
    }
    public override void FrameUpdate(CharacterActionHandler actionHandler)
    {
        base.FrameUpdate(actionHandler);
        if (actionHandler._blackboard.Get<bool>(HIT_TRIGGER_KEY))
        {
            actionHandler.ChangeState(actionHandler.GetState<PlayerHit>());
            return;
        }

        if (actionHandler._blackboard.Get<Vector2>(MOVE_INPUT_KEY) != Vector2.zero)
            actionHandler._blackboard.Set(ATTACK_QUEUED_KEY, false);

        if (actionHandler.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > attackqueuedelay
            && actionHandler._blackboard.Get<bool>(ATTACK_PRESSED_KEY)
            && !actionHandler._blackboard.Get<bool>(ATTACK_QUEUED_KEY)
            )
            {
            actionHandler._blackboard.Set(ATTACK_PRESSED_KEY, false);
            actionHandler._blackboard.Set(ATTACK_QUEUED_KEY, true);
            }

        if (actionHandler.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < attackComboDelay)
        {
            return;
        }

        if (actionHandler._blackboard.Get<Vector2>(MOVE_INPUT_KEY) != Vector2.zero)
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

        if (actionHandler._blackboard.Get<bool>(ATTACK_QUEUED_KEY))
        {
            actionHandler._blackboard.Set(ATTACK_PRESSED_KEY, false);
            actionHandler._blackboard.Set(ATTACK_QUEUED_KEY, false);
            //actionHandler.GetComponent<Animator>().Play(GetOtherAnimationName(actionHandler));
            Attack(actionHandler);
        }
    }
    public override void OnExitState(CharacterActionHandler actionHandler)
    {
        base.OnExitState(actionHandler);
        actionHandler._blackboard.Set(ATTACK_PRESSED_KEY, false);
        actionHandler._blackboard.Set(ATTACK_QUEUED_KEY, false);
        actionHandler.GetComponentInChildren<CombatEntity>().hitboxHandler.DisableAllHitbox();
        actionHandler.GetComponent<Animator>().speed = 1f;
    }
    public override void PhysicsUpdate(CharacterActionHandler actionHandler)
    {
        base.PhysicsUpdate(actionHandler);
    }
    public string GetOtherAnimationName(CharacterActionHandler actionHandler)
    {
        var currentAnim = actionHandler.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(_animationClip.name) ? _animationClip.name : _animationClip2.name;
        return currentAnim == _animationClip.name ? _animationClip2.name : _animationClip.name;
    }
    private void SetupHitbox(CharacterActionHandler actionHandler)
    {
        actionHandler.GetComponentInChildren<CombatEntity>().hitboxHandler.SetDirection( actionHandler.GetComponent<SpriteRenderer>().flipX ? -1 : 1 );
    }
    private void Attack(CharacterActionHandler actionHandler)
    {
        ComboCheck(actionHandler);
        ResetCooldown(actionHandler);
        int attackIndex = actionHandler._blackboard.Get<int>(ATTACK_INDEX_KEY);
        PlaySlash(actionHandler);
        switch (attackIndex)
        {
            case 0:
                actionHandler.GetComponent<Animator>().Play(_animationClip.name);
                actionHandler._blackboard.Set(ATTACK_INDEX_KEY, 1);
                break;
            case 1:
                actionHandler.GetComponent<Animator>().Play(_animationClip2.name);
                actionHandler._blackboard.Set(ATTACK_INDEX_KEY, 0);
                break;
        }
    }
    private void ResetCooldown(CharacterActionHandler actionHandler)
    {
        actionHandler._blackboard.Set(ATTACK_COMBO_TIMER_KEY, 0f);
    }
    private void ComboCheck(CharacterActionHandler actionHandler)
    {
        if (actionHandler._blackboard.Get<float>(ATTACK_COMBO_TIMER_KEY) > attackComboBaseCooldown)
        {
            actionHandler._blackboard.Set(ATTACK_INDEX_KEY, 0);
            return;
        }
    }
    private void PlaySlash(CharacterActionHandler actionHandler)
    {
        SoundClipInfo newSoundClipWithPosition = _actionAudio.DeepCopy();
        newSoundClipWithPosition.position = actionHandler.transform.position;
        AudioManager.instance.PlayAudioClipInstance(newSoundClipWithPosition);
    }
}
