using System;
using UnityEngine;

[Serializable]
public abstract class ActionStateLogic : ScriptableObject
{
    public AnimationClip _animationClip;
    public virtual void InitializeState(CharacterActionHandler actionHandler) 
    { 
    
    }
    public virtual void OnEnterState(CharacterActionHandler actionHandler)
    {
    }

    public virtual void OnExitState(CharacterActionHandler actionHandler) { }

    public virtual void PhysicsUpdate(CharacterActionHandler actionHandler) { }
    //This would check if the state should change, time counters, checks, etc. 
    public virtual void FrameUpdate(CharacterActionHandler actionHandler) { }
}