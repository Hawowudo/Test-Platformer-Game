using UnityEngine;

// class is the base class for all AI logic.
// Can be used to handle different logic, IE flyer logic, Patrol and chase logic, boss logic, etc.
public abstract class AILogic : ScriptableObject
{
    public virtual void OnEnterLogic(CharacterLogicHandler logicHandler)
    {

    }
    public virtual void OnExitLogic(CharacterLogicHandler logicHandler)
    {

    }
    public virtual void FrameUpdate(CharacterLogicHandler logicHandler)
    {

    }
    public virtual Blackboard GetBlackboard(CharacterLogicHandler logicHandler)
    {
        return logicHandler.blackboard;
    }
}
