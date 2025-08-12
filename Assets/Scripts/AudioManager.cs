// AudioManager.cs
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Mixer (optional)")]
    public UnityEngine.Audio.AudioMixer mixer; // drag your mixer, expose "SFXVol" "MusicVol" params optionally

    [Header("One-shots")]
    public AudioClip footstepNormal;
    public AudioClip footstepIce;
    public AudioClip dash;
    public AudioClip playerHit;
    public AudioClip keyPickup;
    public AudioClip chestOpen;
    public AudioClip doorOpen;
    public AudioClip puzzleSolved;
    public AudioClip bossRoar;
    public AudioClip death;
    public AudioClip lavaTick;   // short sizzle when lava damages player
    public AudioClip fireball;   // short sizzle when lava damages player

    [Header("Ambience (loops)")]
    public AudioClip ambNormal;
    public AudioClip ambIce;
    public AudioClip ambVision;  // darkness/cone level
    public AudioClip ambLava;
    public AudioClip bgm;

    [Header("Pool")]
    [SerializeField] int poolSize = 10;

    private List<AudioSource> pool = new();
    private AudioSource musicSource;   // for ambience loop

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // make pool
        for (int i = 0; i < poolSize; i++)
        {
            var s = new GameObject("SFX_" + i).AddComponent<AudioSource>();
            s.transform.SetParent(transform);
            s.playOnAwake = false;
            s.spatialBlend = 0f;
            pool.Add(s);
        }
        musicSource = new GameObject("Ambience").AddComponent<AudioSource>();
        musicSource.transform.SetParent(transform);
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = 0.75f;
    }

    AudioSource GetFree()
    {
        foreach (var s in pool) if (!s.isPlaying) return s;
        return pool[0]; // fallback
    }

    public void PlayOneShot(AudioClip clip, float vol = 1f, float pitch = 1f)
    {
        if (!clip) return;
        var s = GetFree();
        s.volume = vol;
        s.pitch = pitch;
        s.clip = clip;
        s.Play();
    }

    public void PlayAtPosition(AudioClip clip, Vector3 pos, float vol = 1f, float pitch = 1f)
    {
        if (!clip) return;
        var s = GetFree();
        s.transform.position = pos;
        s.spatialBlend = 0.5f;   // mild 2D/3D feel
        s.volume = vol;
        s.pitch = pitch;
        s.clip = clip;
        s.Play();
        s.spatialBlend = 0f; // reset for next reuse
    }

    public void SetAmbience(AudioClip loop, float vol = 0.75f)
    {
        if (musicSource.clip == loop) return;
        musicSource.Stop();
        musicSource.clip = loop;
        musicSource.volume = vol;
        if (loop) musicSource.Play();
    }
}
