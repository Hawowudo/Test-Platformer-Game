using UnityEngine;

[CreateAssetMenu(fileName = "PlayerIdleASL", menuName = "ActionStateLogic/PlayerIdleASL", order = 1)]
public class PlayerIdleASL : ActionStateLogic
{
    public override void OnEnterState(CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);
        //actionHandler._animator.Play("Idle");
    }
    public override void FrameUpdate(CharacterActionHandler actionHandler)
    {
        base.FrameUpdate(actionHandler);
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
