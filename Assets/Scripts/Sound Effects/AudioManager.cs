using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioClip[] musicList;
    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioSource voicePlayer;
    [SerializeField] private SoundEffectsBank soundEffectsBank;
    [SerializeField] private MusicVolumeOverride[] musicVolumeOverrides;
    private float baseMusicVolume;
    private Coroutine musicFade;
    
    // Public getters to check if sounds are playing
    public bool IsMusicPlaying => musicPlayer.isPlaying;
    public bool IsSfxPlaying => sfxPlayer.isPlaying;
    public bool IsVoicePlaying => voicePlayer.isPlaying;

    public void Awake()
    {
        baseMusicVolume = musicPlayer.volume;
    }

    // Selects a random BGM song and plays it
    public void PlayRandomMusic()
    {
        // If music fade is happening, stop it
        if (musicFade != null) { StopCoroutine(musicFade); musicFade = null; }
        // Restore volume
        musicPlayer.volume = baseMusicVolume;
        
        if (musicList.Length == 0)
        {
            Debug.LogWarning("Music list is empty");
            return;
        }
        // Get the chosen song
        AudioClip chosen = musicList[Random.Range(0, musicList.Length)];
        musicPlayer.clip = chosen;
        // Scale from the source's base volume, so the quiet override doesn't change the others
        musicPlayer.volume = baseMusicVolume * GetTrackVolumeScale(chosen);
        musicPlayer.Play();
    }
    
    // Stops the BGM
    public void StopMusic()
    {
        musicPlayer.Stop();
    }
    
    // Plays a given voice clip
    public void PlayVoice(AudioClip voice)
    {
        // If voice line is missing, do nothing
        if (!voice)
        {
            Debug.LogWarning($"Voice clip is missing: {voice.name}");
            return;
        }
        voicePlayer.clip = voice;
        voicePlayer.Play();
    }

    // Stop the voice player
    public void StopVoice()
    {
        voicePlayer.Stop();
    }
    
    // Plays a given sound effect
    public void PlaySoundEffect(Sfx id)
    {
        // Check for a valid id in the sfx enum
        if (!soundEffectsBank.TryGetEntry(id, out SoundEffectEntry entry))
        {
            Debug.LogWarning($"No sound for ID: {id}");
            return;
        }

        // check if there are audio clips corresponding to that enum value
        if (entry.clips == null || entry.clips.Length == 0)
        {
            Debug.LogWarning($"Could not find audio clips for : {entry.id}");
            return;
        }
        
        // Get a random clip for that sound effect and play it
        sfxPlayer.clip = entry.clips[Random.Range(0, entry.clips.Length)];
        
        // Vary pitch slightly just for fun to make it sound less monotonous
        //sfxPlayer.pitch = Random.Range(0.95f, 1.05f);
        
        // PlayOneShot: layers sounds on one source so they don't cut each other off
        // You have to specify which clip to play and can give it a volume as well
        sfxPlayer.PlayOneShot(sfxPlayer.clip, entry.volume);
    }

    public Coroutine FadeOutMusic(float duration)
    {
        // If the fade is already going, stop it
        if (musicFade != null) StopCoroutine(musicFade);

        musicFade = StartCoroutine(FadeOutMusicRoutine(duration));
        return musicFade;

    }

    private IEnumerator FadeOutMusicRoutine(float duration)
    {
        // Stop if no music is playing
        if (!IsMusicPlaying) yield break;

        float startVolume = musicPlayer.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            musicPlayer.volume =  Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }
        
        StopMusic();
        musicPlayer.volume = baseMusicVolume;
        musicFade = null;
    }
    
    // Returns the multiplier for this track, or 1 if it has no override
    private float GetTrackVolumeScale(AudioClip clip)
    {
        if (musicVolumeOverrides == null) return 1f;
        foreach (MusicVolumeOverride entry in musicVolumeOverrides)
        {
            if (entry.clip == clip) return entry.volume;
        }
        return 1f;
    }
}

// Allows you to set volumes for individual songs as needed
[System.Serializable]
public class MusicVolumeOverride
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 0.5f;   // multiplies the music source's volume
}