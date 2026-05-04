using CombatSystem;
using UnityEngine;

//Put this on an entity you want to be controlled by either a player or an AI logic.
//It handles everything an entity might use.
public class CharacterLogicHandler : MonoBehaviour
{
    public enum ControlType
    {
        AI,
        Player
    }
    public Blackboard blackboard;
    public BlackboardTimer blackboardTimer;
    public CharacterActionHandler actionHandler;
    public CombatEntity combatEntity;

    [Header("Logic Settings")]
    public ControlType controlType;
    public AILogic aiLogic;
    
    private void Start()
    {
        actionHandler = GetComponent<CharacterActionHandler>();
        combatEntity = GetComponent<CombatEntity>();
        blackboard = GetComponent<Blackboard>();
    }
    void Update()
    {
        if (controlType == ControlType.AI && aiLogic != null)
        {
            aiLogic.FrameUpdate(this);
        }
    }
    public void DisableLogic()
    {
        if (controlType == ControlType.AI && aiLogic != null)
        {
            aiLogic.OnExitLogic(this);
        }

    }
        public void ResetEnemy()
    {
        if (controlType != ControlType.AI || aiLogic == null)
            return;

        aiLogic.OnEnterLogic(this);
        actionHandler.SetToInitialState();
        combatEntity.ResetValues();
        combatEntity.EnableHurtbox();
    }
}
