using UnityEngine;

public class CharacterLogicHandler : MonoBehaviour
{
    public enum ControlType
    {
        AI,
        Player
    }
    public ControlType controlType;
    public Blackboard blackboard;
    public AILogic aiLogic;

    private void Start()
    {
        blackboard = GetComponent<Blackboard>();
        if (controlType == ControlType.AI && aiLogic != null)
        {
            aiLogic.OnEnterLogic(this);
        }
    }
    void Update()
    {
        if (controlType == ControlType.AI && aiLogic != null)
        {
            aiLogic.FrameUpdate(this);
        }
    }
}
