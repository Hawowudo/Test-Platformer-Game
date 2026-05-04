using AudioManagerPackage;
using System;
using UnityEngine;

[Serializable]
public abstract class ActionStateLogic : ScriptableObject
{
    [SerializeField] protected AnimationClip _animationClip;
    [SerializeField] protected SoundClipInfo _actionAudio;
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
    public virtual void PlayAudio(CharacterActionHandler actionHandler)
    {
        SoundClipInfo newSoundClipWithPosition = _actionAudio.DeepCopy();
        newSoundClipWithPosition.position = actionHandler.transform.position;
        AudioManager.instance.PlayAudioClipInstance(newSoundClipWithPosition);
    }
}