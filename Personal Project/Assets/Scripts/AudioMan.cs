using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioMan : MonoBehaviour
{
    // Fields
    public static AudioMan instance;
    public Sound mainTheme;
    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        // Ensures only 1 instance of audio manager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Creates sound objects
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
            sound.source.ignoreListenerPause = sound.ignoreListenerPause;
        }

        // Creates main theme
        mainTheme.source = gameObject.AddComponent<AudioSource>();
        mainTheme.source.clip = mainTheme.clip;
        mainTheme.source.volume = mainTheme.volume;
        mainTheme.source.loop = mainTheme.loop;
        mainTheme.source.ignoreListenerPause = mainTheme.ignoreListenerPause;
    }

    // Starts playing main theme
    private void Start()
    {
        //mainTheme.source.Play();
    }

    void Update()
    {
        
    }

    // Plays any audio clip
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Play();
        }
    }

    // Stops any audio clip
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null && s.source.isPlaying == true)
        {
            s.source.Stop();
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    public bool loop;

    public bool ignoreListenerPause;

    [HideInInspector]
    public AudioSource source;
}
