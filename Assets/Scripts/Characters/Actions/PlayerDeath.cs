using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDeath", menuName = "ActionStateLogic/PlayerDeath", order = 1)]
public class PlayerDeath : ActionStateLogic
{
    private float _deathDuration;
    public override void OnEnterState(CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);
        actionHandler.GetComponent<Animator>().Play(_animationClip.name);
        _deathDuration = _animationClip.length;

        actionHandler._blackboardTimer.AddTimerToBlackboard("deathanimtimer",.0f);
    }
    public override void FrameUpdate(CharacterActionHandler actionHandler)
    {
        base.FrameUpdate(actionHandler);
        if (actionHandler._blackboard.Get<float>("deathanimtimer") > _deathDuration)
        {
            actionHandler.gameObject.SetActive(false);
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
}
