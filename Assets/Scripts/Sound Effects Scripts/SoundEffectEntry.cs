using System.Collections.Generic;
using UnityEngine;

// SoundEffectEntry: holds all the data related to one sound effect
[System.Serializable]
public class SoundEffectEntry
{
    // Give it access to the enum
    public Sfx id;
    // An array in case you want different sounds for the same event/effect
    public AudioClip[] clips;
    [Range(0f, 1f)] public float volume = 0.5f;
}