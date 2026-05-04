using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AudioManagerPackage;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public VolumeValues currentVolumeValues = new();

    public AudioFileLibraryScriptableObject SFX;
    public AudioFileLibraryScriptableObject Music;
    private List<Sound> _soundLibrary = new List<Sound>();
    private HashSet<AudioPool> _audioPools = new HashSet<AudioPool>();

    public bool DebugMode;
    public bool soundTrigger;
    public bool musicTrigger;
    public bool stopMusicTrigger;
    public SoundClipInfo debugSoundName = new();
    public UniqueSoundClipInfo debugMusicName = new();

    public void DebugPlaySoundClip()
    {
        SoundClipInfo soundClipInfoCopy = debugSoundName.DeepCopy();
        PlayAudioClipInstance(soundClipInfoCopy);
    }
    public void DebugPlayMusicClip()
    {
        PlayAudioClipUnique(debugMusicName);
    }
    //should not be changed manually, this is just a reference to all the sounds in the libraries.
    //example: source = music_1, audioID = "MUSIC"
    // source = ambience_1, audioID ="AMBIENCE"
    private Dictionary<string, (AudioSource source1, AudioSource source2)> _persistentAudios = new Dictionary<string, (AudioSource source1, AudioSource source2)>();
    [SerializeField] private HashSet<SoundClipInfo> _soundInstanceList = new HashSet<SoundClipInfo>();
    void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

    }
    private void Start()
    {
        AddToSounds(SFX.AudioLibrary);
        AddToSounds(Music.AudioLibrary);

        SetupSounds();
        SetNewVolumeValues(GetSavedVolumeValues());

    }
    private void Update()
    {
        if (!DebugMode)
            return;
        if(soundTrigger)
        {
            soundTrigger = false;
            DebugPlaySoundClip();
        }
        if (musicTrigger)
        {
            musicTrigger = false;
            DebugPlayMusicClip();
        }
        if (stopMusicTrigger)
        {
            stopMusicTrigger = false;
            if (_persistentAudios.ContainsKey(debugMusicName.uniqueTrackID))
            {
                var audioSources = _persistentAudios[debugMusicName.uniqueTrackID];
            }
        }

    }
    #region AudioSetup

    public void SetNewVolumeValues(VolumeValues newValues)
    {
        currentVolumeValues = newValues;
        PlayerPrefs.SetFloat("MasterVolume", currentVolumeValues.masterVolume);
        PlayerPrefs.SetFloat("SFXVolume", currentVolumeValues.sfxVolume);
        PlayerPrefs.SetFloat("CharacterVolume", currentVolumeValues.characterVolume);
        PlayerPrefs.SetFloat("MusicVolume", currentVolumeValues.musicVolume);
        PlayerPrefs.Save();
        UpdateAllVolumes();
    }
    VolumeValues GetSavedVolumeValues()
    {
        VolumeValues savedValues = new VolumeValues();
        savedValues.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        savedValues.sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        savedValues.characterVolume = PlayerPrefs.GetFloat("CharacterVolume", 1f);
        savedValues.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        return savedValues;
    }
    void UpdateAllVolumes()
    {
        UpdateVolume(SFX);
        UpdateVolume(Music);
        UpdateActiveSoundSourceVolumes();
        UpdateUniqueAudioVolumes();
    }
    void UpdateVolume(AudioFileLibraryScriptableObject audioLibrary)
    {
        foreach (SoundWrapper sw in audioLibrary.AudioLibrary)
        {
            Sound s = sw.sound;
            s.source.volume = s.volume * currentVolumeValues.masterVolume * GetSoundTypeVolume(audioLibrary.AudioLibrarySoundType);
        }

    }
    void UpdateActiveSoundSourceVolumes()
    {
        foreach (SoundClipInfo s in _soundInstanceList)
        {
            if (s.soundClipAudioSource == null)
            {
                continue;
            }
            SoundType librarySoundType = GetLibraryOfSoundName(s.soundClipAudioSource.clip.name).AudioLibrarySoundType;
            s.soundClipAudioSource.volume = FindSoundInReferenceLibrary(s.soundName).volume * currentVolumeValues.masterVolume * GetSoundTypeVolume(librarySoundType);
        }
    }
    AudioFileLibraryScriptableObject GetLibraryOfSoundName(string name)
    {
        foreach (AudioFileLibraryScriptableObject library in new AudioFileLibraryScriptableObject[] { SFX,   Music })
        {
            foreach (SoundWrapper sw in library.AudioLibrary)
            {
                if (sw.sound.name.ToUpper() == name.ToUpper())
                {
                    return library;
                }
            }
        }
        return null;
    }
    float GetSoundTypeVolume(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.SFX:
                return currentVolumeValues.sfxVolume;
            case SoundType.Music:
                return currentVolumeValues.musicVolume;
            default:
                return 1f;
        }
    }
    void UpdateUniqueAudioVolumes()
    {
        foreach (KeyValuePair<string, (AudioSource source1, AudioSource source2)> pair in _persistentAudios)
        {
            UpdateAudioSourceVolume(pair.Value.source1);
            UpdateAudioSourceVolume(pair.Value.source2);
        }

        void UpdateAudioSourceVolume(AudioSource asource)
        {
            if(asource == null || asource.clip == null)
            {
                return;
            }
            SoundType librarySoundType = GetLibraryOfSoundName(asource.clip.name).AudioLibrarySoundType;
            Sound reference = FindSoundInReferenceLibrary(asource.clip.name);
            asource.volume = reference.volume * currentVolumeValues.masterVolume * GetSoundTypeVolume(librarySoundType);

        }
    }
    void AddToSounds(SoundWrapper[] wrapper)
    {
        List<Sound> soundsInSoundsArray = new List<Sound>();
        foreach (Sound s in _soundLibrary)
        {
            soundsInSoundsArray.Add(s);
        }
        foreach (SoundWrapper s in wrapper) 
        {
            soundsInSoundsArray.Add(s.sound);
        }

        _soundLibrary = soundsInSoundsArray;
    }
    void SetupSounds()
    {
        //if there's thousands of sounds, it might cause some performance issues, but for a reasonable amount of sounds it should be fine.
        //If performance becomes an issue, consider using coroutine to load the sounds over multiple frames.
        foreach (Sound s in _soundLibrary)
        {
            if (s.source == null)
            {
                // add initial audioSourceReference
                s.source = gameObject.AddComponent<AudioSource>();
            }
            s.source.playOnAwake = s.PlayOnAwake;
            s.source.loop = s.Loop;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            if (s.PlayOnAwake == true)
            {
                s.source.Play();
            }

        }
    }
    Sound FindSoundInReferenceLibrary(string name)
    {
        try
        {
            foreach (Sound s in _soundLibrary)
            {
                if (s.name.ToUpper() == name.ToUpper())
                {
                    return s;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        Debug.Log("no reference in library" + name);

        return null;
    }
    #endregion
    public void QuickPlayAudio(string audioName, Vector2 pos)
    {
        SoundClipInfo soundClipInfo = new SoundClipInfo(pos, audioName);
        PlayAudioClipInstance(soundClipInfo);
    }
    //Use this function for single instance audio clips, like footsteps, gunshots, and the type of stuff that can be played multiple times at once without cutting each other off.
    //For music , or clips that should not be played multiple times at once, use PlayAudioClipUnique
    public void PlayAudioClipInstance(SoundClipInfo soundClipInfo)
    {
        if(String.IsNullOrEmpty(soundClipInfo.soundName))
        {
            Debug.LogWarning("Sound name is empty");
            return;
        }
        if(soundClipInfo == null)
        {
            Debug.LogWarning("SoundClipInfo is null");
            return;
        }
        StartCoroutine(PlayAudioRoutine(FindSoundInReferenceLibrary(soundClipInfo.soundName), soundClipInfo));
    }
    private IEnumerator PlayAudioRoutine(Sound soundReference, SoundClipInfo soundClipInfo)
    {
        if (DebugMode)
        {
            Debug.Log("Sound played: " + soundReference.name + 
                (soundClipInfo.position != Vector2.zero ? ") (Position: " + soundClipInfo.position : "") + 
                (soundClipInfo.pitch != 1 ?" (Pitch: " + soundClipInfo.pitch + ")" : ""));
        }

        float originalPitch = soundReference.pitch;
        soundClipInfo.soundClipAudioSource = GetAudioSource(soundClipInfo.position,soundClipInfo.soundName);

        soundClipInfo.soundClipAudioSource.volume = soundReference.volume;
        soundClipInfo.soundClipAudioSource.clip = soundReference.clip;
        soundClipInfo.soundClipAudioSource.spatialBlend = 1f;
        soundClipInfo.soundClipAudioSource.pitch = soundClipInfo.pitch * originalPitch;
        soundClipInfo.soundClipAudioSource.Play();
        float length = soundReference.clip.length;
        Debug.Log("Sound length: " + length);

        _soundInstanceList.Add(soundClipInfo);
        yield return new WaitForSecondsRealtime(length / Mathf.Abs(soundClipInfo.soundClipAudioSource.pitch));
        if (DebugMode)
        {
            Debug.Log("Sound stopped: " + soundReference.name +
                (soundClipInfo.position != Vector2.zero ? ") (Position: " + soundClipInfo.position : "") +
                (soundClipInfo.pitch != 1 ? " (Pitch: " + soundClipInfo.pitch + ")" : ""));
        }

        soundClipInfo.soundClipAudioSource.Stop();
        soundClipInfo.soundClipAudioSource.gameObject.SetActive(false);
        _soundInstanceList.Remove(soundClipInfo);


        AudioSource GetAudioSource(Vector2 position, string soundName = "")
        {
            AudioPool audioPool = GetAudioPool(soundName);
            GameObject pooledObject = audioPool.GetInactiveObject();

            if(pooledObject == null )
            {
                GameObject gameObject = new GameObject("Audio Source (" + soundName + ")");
                audioPool.AddNewObject(gameObject);
                pooledObject = gameObject;
                pooledObject.AddComponent(typeof(AudioSource));
            }
            pooledObject.transform.parent = this.transform;
            pooledObject.SetActive(true);
            pooledObject.transform.position = new Vector3(position.x, position.y, FindAnyObjectByType<AudioListener>().transform.position.z);
            pooledObject.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
            pooledObject.GetComponent<AudioSource>().minDistance = 7f;
            pooledObject.GetComponent<AudioSource>().maxDistance = 7.5f;


            return pooledObject.GetComponent<AudioSource>();
        }

    }
    
    // stops the first instance of a sound with the given name. If there are multiple instances of the same sound, only the first one found will be stopped.
    // Use with caution, it's better to keep track of the SoundClipInfo reference if you want to stop a specific instance of a sound.
    void StopSoundInstance(string name)
    {
        foreach (SoundClipInfo soundClipInfo in _soundInstanceList)
        {
            if (soundClipInfo.soundName.ToUpper() == name.ToUpper())
            {
                soundClipInfo.soundClipAudioSource.Stop();
                UnityEngine.Object.Destroy(soundClipInfo.soundClipAudioSource.gameObject);
                _soundInstanceList.Remove(soundClipInfo);
                return;
            }
        }
    }
    public void PlayAudioClipUnique( UniqueSoundClipInfo uniqueSoundClipInfo )
    {
        if (String.IsNullOrEmpty(uniqueSoundClipInfo.soundName))
        {
            Debug.LogWarning("Sound name is empty");
            return;
        }
        (AudioSource source1, AudioSource source2) audioSourceGameObject;

        if (!_persistentAudios.ContainsKey(uniqueSoundClipInfo.uniqueTrackID))
        {
            GameObject audioListenerGameObject = FindAnyObjectByType<AudioListener>().gameObject;
            GameObject AudioSourceGO = new GameObject("Unique Audio Source (" + uniqueSoundClipInfo.uniqueTrackID + ")");
            AudioSourceGO.transform.position = audioListenerGameObject.transform.position;
            AudioSourceGO.transform.parent = audioListenerGameObject.transform;

            audioSourceGameObject.source1 = AudioSourceGO.AddComponent<AudioSource>();
            audioSourceGameObject.source2 = AudioSourceGO.AddComponent<AudioSource>();
            audioSourceGameObject.source1.playOnAwake = false;
            audioSourceGameObject.source2.playOnAwake = false;
            _persistentAudios[uniqueSoundClipInfo.uniqueTrackID] = (audioSourceGameObject.source1,audioSourceGameObject.source2);
        }
        audioSourceGameObject = _persistentAudios[uniqueSoundClipInfo.uniqueTrackID];
        CopySoundReferenceValuesToAudioSource(FindSoundInReferenceLibrary(uniqueSoundClipInfo.soundName), audioSourceGameObject.source2);
    }
    void CopySoundReferenceValuesToAudioSource(Sound reference, AudioSource source)
    {
        if (source == null)
        {
            return;
        }
        if (reference == null )
        {
            source.clip = null;
            return;
        }
        SoundType librarySoundType = GetLibraryOfSoundName(reference.clip.name).AudioLibrarySoundType;
        source.volume = reference.volume * currentVolumeValues.masterVolume * GetSoundTypeVolume(librarySoundType);
        source.clip = reference.clip;
        source.spatialBlend = reference.source.spatialBlend;
        source.pitch = reference.pitch;
    }

    private AudioPool GetAudioPool(string audioName)
    {
        foreach (AudioPool pool in _audioPools)
        {
            if (pool.soundName.ToUpper() == audioName.ToUpper())
            {
                return pool;
            }
        }

        _audioPools.Add(new AudioPool(audioName));
        return GetAudioPool(audioName);


    }
}

namespace AudioManagerPackage
{
    public enum SoundType
    {
        SFX,
        Music
    }
    [System.Serializable]
    public class Sound
    {
        public string name;

        public AudioClip clip;

        [Range(0, 1)]
        public float volume = 0.1f;
        public float newVolume = 0.1f;
        [Range(0.1f, 3)]
        public float pitch = 1;

        [HideInInspector]
        public AudioSource source;
        public bool PlayOnAwake;
        public bool Loop;

        public Sound()
        {
            volume = 0.1f;
            newVolume = 0.1f;
            pitch = 1;
        }
        private void UpdateName()
        {
            if (clip != null && String.IsNullOrEmpty(name))
            {
                name = clip.name.ToUpper();
            }
        }
    }
    [Serializable]
    public class SoundWrapper
    {
        public Sound sound;
    }
    [System.Serializable]
    public class UniqueSoundClipInfo
    {
        public string soundName;
        public float fadeInDuration = 3f;
        public float previousSoundFadeOutDuration = 3f;
        public string uniqueTrackID = "";
        public UniqueSoundClipInfo(string name = "", float fadeInDuration = 3f, float previousSoundFadeOutDuration = 3f, string uniqueTrackID = "")
        {
            this.soundName = name;
            this.fadeInDuration = fadeInDuration;
            this.previousSoundFadeOutDuration = previousSoundFadeOutDuration;
            this.uniqueTrackID = uniqueTrackID.ToUpper();
        }

        void UpdateTrackID()
        {
            if (!String.IsNullOrEmpty(uniqueTrackID))
            {
                uniqueTrackID = uniqueTrackID.ToUpper();
            }
        }
        void UpdateName()
        {
            if (!String.IsNullOrEmpty(soundName))
            {
                soundName = soundName.ToUpper();
            }
        }
    }
    //use this class when you want to play a sound clip. It contains all the info needed to play the clip, such as pitch, position, fade in and fade out duration, etc.
    [System.Serializable]
    public class SoundClipInfo
    {
        public string soundName;
        public float pitch;
        public Vector2 position;
        public float fadeInDuration;
        public float fadeOutDuration;
        
        public AudioSource soundClipAudioSource;
        public SoundClipInfo(string name = "", float pitch =1, float fadeInDuration = 3f, float fadeOutDuration = 3f, AudioSource soundClipAudioSource = null)
        {
            this.soundName = name;
            this.pitch = pitch;
            this.fadeInDuration = fadeInDuration;
            this.fadeOutDuration = fadeOutDuration;
            Vector2 position = Vector2.zero;
            this.soundClipAudioSource = soundClipAudioSource;
        }
        public SoundClipInfo(Vector2 position, string name = "", float pitch = 1, float fadeInDuration = 3f, float fadeOutDuration = 3f, AudioSource soundClipAudioSource = null)
        {
            this.soundName = name;
            this.pitch = pitch;
            this.fadeInDuration = fadeInDuration;
            this.fadeOutDuration = fadeOutDuration;
            this.position = position;
            this.soundClipAudioSource = soundClipAudioSource;
        }
        void UpdateName()
        {
            if (!String.IsNullOrEmpty(soundName))
            {
                soundName = soundName.ToUpper();
            }
        }

        public SoundClipInfo DeepCopy()
        {
            return new SoundClipInfo(this.position, this.soundName, this.pitch, this.fadeInDuration, this.fadeOutDuration, soundClipAudioSource);
        }
    }

    [System.Serializable]
    public class VolumeValues
    {
        [Range(0, 1)]
        public float masterVolume = 0.1f;
        [Range(0, 1)]
        public float sfxVolume = 1;
        [Range(0, 1)]
        public float characterVolume = 1;
        [Range(0, 1)]
        public float musicVolume = 1;

        public VolumeValues(float masterVolume = 0.1f, float sfxVolume = 1,   float characterVolume = 1,   float musicVolume = 1)
        {
            this.masterVolume = masterVolume;
            this.sfxVolume = sfxVolume;
            this.characterVolume = characterVolume;
            this.musicVolume = musicVolume;
        }
    }
    [System.Serializable]
    public class AudioPool
    {
        public string soundName;
        public HashSet<GameObject> objects = new HashSet<GameObject>();

        public AudioPool(string soundName)
        {
            this.soundName = soundName;
        }

        public GameObject GetInactiveObject()
        {
            foreach (GameObject obj in objects)
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }
            return null;
        }

        public void AddNewObject(GameObject objectToAdd)
        {
            objects.Add(objectToAdd);
        }
    }
}
