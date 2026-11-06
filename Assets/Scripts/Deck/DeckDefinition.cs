using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeck", menuName = "Magic Carta/Deck")]
public class DeckDefinition : ScriptableObject
{
    // Create the serialized field to store data which you can load in the editor
    [SerializeField] private string deckName;
    // Create a getter method to simply return some field
    public string DeckName => deckName;
    
    [SerializeField] private string deckTag;
    public string DeckTag => deckTag;
    
    [SerializeField] private TextAsset cardsCSV;
    public TextAsset CardsCSV => cardsCSV;
    
    [SerializeField] private Sprite[] artList;
    public IReadOnlyList<Sprite> ArtList => artList;
    
    [SerializeField] private AudioClip[] voice1List;
    public IReadOnlyList<AudioClip> Voice1List => voice1List;
    
    [SerializeField] private AudioClip[] voice2List;
    public IReadOnlyList<AudioClip> Voice2List => voice2List;
}

