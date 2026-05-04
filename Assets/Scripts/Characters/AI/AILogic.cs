using UnityEngine;


//[CreateAssetMenu(fileName = "New AILogic", menuName = "AILogic/Basic")]
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
