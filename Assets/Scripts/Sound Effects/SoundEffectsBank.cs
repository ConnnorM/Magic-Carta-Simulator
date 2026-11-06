using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffectsBank", menuName = "Magic Carta/Sound Effects Bank")]
public class SoundEffectsBank : ScriptableObject
{
    [SerializeField] public SoundEffectEntry[] soundEffectEntries;
    private Dictionary<Sfx, SoundEffectEntry> lookup;

    public bool TryGetEntry(Sfx id, out SoundEffectEntry entry)
    {
        if (lookup == null)
        {
            // Create lookup dictionary
            lookup = new Dictionary<Sfx, SoundEffectEntry>();
            foreach (SoundEffectEntry sfxEntry in soundEffectEntries)
            {
                if (sfxEntry == null)
                {
                    Debug.LogWarning("Sound Effect is null");
                    continue;
                }
                if (!lookup.TryAdd(sfxEntry.id, sfxEntry))
                {
                    Debug.LogWarning($"Duplicate Sound Effect: {sfxEntry.id}");
                }
            }
        }
        return lookup.TryGetValue(id, out entry);
    }

    // A Callback: changes when a value in the inspector changes
    // Use this so that if you change a sound effect, it rebuilds the lookup and you don't need to restart everything
    private void OnValidate()
    {
        lookup = null;
    }
}