using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

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

    public void ChangePitch (string name, float newPitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + "pitch shifted but not found!");
            return;
        }

        if (s.source.pitch != newPitch)
        {
            newPitch = Mathf.Clamp(newPitch, 0, 1);
            StartCoroutine(ChangePitchOvertime(s, s.source.pitch, newPitch, 1));
        }
    }

    IEnumerator ChangePitchOvertime(Sound s, float oldValue, float newValue, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            s.source.pitch = Mathf.Lerp(oldValue, newValue, t / duration);
            yield return null;
        }
        s.source.pitch = newValue;
    }

    public void ChangeVolume(string name, float newVolume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + "volume shifted but not found!");
            return;
        }

        newVolume *= s.volume;
        newVolume = Mathf.Clamp(newVolume, 0, 1);

        if (s.source.volume != newVolume)
        {
            StartCoroutine(ChangeVolumeOvertime(s, s.source.volume, newVolume, 1));
        }
    }

    IEnumerator ChangeVolumeOvertime(Sound s, float oldValue, float newValue, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            s.source.volume = Mathf.Lerp(oldValue, newValue, t / duration);
            yield return null;
        }
        s.source.volume = newValue;
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
        StopAllSounds();
        switch (next)
        {
            case 0: //Start Scene
                PlaySound("StartMusic");
                break;
            case 1: //Game Scene
                PlaySound("GameMusic");
                break;
            case 2: //Win Scene
                PlaySound("WinMusic");
                break;
            case 3: //Lose Scene (candle)
            case 4: //Lose Scene (jack) [intentional fallthrough]
                PlaySound("LoseMusic");
                break;
            default: //this is probably a test scene, play game music and play a debug message
                Debug.LogWarning("This scene has no music set, playing default music of GameMusic!");
                PlaySound("GameMusic");
                break;
        }
    }
}
