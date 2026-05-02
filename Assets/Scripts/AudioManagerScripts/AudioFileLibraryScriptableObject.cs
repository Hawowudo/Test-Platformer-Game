using UnityEngine;
using AudioManagerPackage;
[CreateAssetMenu(fileName = "AudioFileLibrary", menuName = "Scriptable Objects/AudioFileScriptableObject", order = 1)]
public class AudioFileLibraryScriptableObject : ScriptableObject
{
    public SoundType AudioLibrarySoundType;
    public SoundWrapper[] AudioLibrary;
}
