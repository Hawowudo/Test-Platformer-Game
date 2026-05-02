using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTurn", menuName = "ActionStateLogic/PlayerTurn", order = 1)]
public class PlayerTurn : ActionStateLogic
{
    public override void OnEnterState(CharacterActionHandler actionHandler)
    {
        base.OnEnterState(actionHandler);
        actionHandler.GetComponent<Animator>().Play(_animationClip.name);
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
