using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private AudioClip[] musicList;
    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioSource voicePlayer;
    [SerializeField] private SoundEffectsBank soundEffectsBank;
    
    // Public getters to check if sounds are playing
    public bool IsMusicPlaying => musicPlayer.isPlaying;
    public bool IsSfxPlaying => sfxPlayer.isPlaying;
    public bool IsVoicePlaying => voicePlayer.isPlaying;


    // Selects a random BGM song and plays it
    public void PlayRandomMusic()
    {
        if (musicList.Length == 0)
        {
            Debug.LogWarning("Music list is empty");
            return;
        }
        musicPlayer.clip = musicList[Random.Range(0, musicList.Length)];
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
}