using UnityEngine.Audio;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        //singleton! (only one can survive!!)
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

        foreach(Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }

        SceneManager.activeSceneChanged += ActiveSceneChanged;
    }

    public void PlaySound (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + "played but not found!");
            return;
        }

        if (!s.source.isPlaying)
        {
            s.source.Play();
        }
        
    }

    public void StopSound (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + "stopped but not found!");
            return;
        }

        s.source.Stop();
    }

    public void StopAllSounds()
    {
        foreach (Sound sound in sounds)
        {
            sound.source.Stop();
        }
    }

    private void Start()
    {
        LoadLevelMusic(0, SceneManager.GetActiveScene().buildIndex);
    }

    private void ActiveSceneChanged(Scene current, Scene next)
    {
        LoadLevelMusic(current.buildIndex, next.buildIndex);
    }

    private void LoadLevelMusic (int current, int next)
    {
        switch (next)
        {
            case 0: //Start Scene
                StopAllSounds();
                PlaySound("StartMusic");
                break;
            case 1: //Game Scene
                StopAllSounds();
                PlaySound("GameMusic");
                break;
            case 2: //Win Scene
                StopSound("GameMusic");
                PlaySound("WinMusic");
                break;
            case 3: //Lose Scene
                StopSound("GameMusic");
                PlaySound("LoseMusic");
                break;
            default:
                break;
        }
    }
}
