using UnityEngine;

public class Card
{
    public int id { get; private set; }
    public string deckID { get; private set; }
    public string cardName { get; private set; }
    public AudioClip voice1 { get; private set; }
    public AudioClip voice2 { get; private set; }
    public string text1 { get; private set; }
    public string text2 { get; private set; }
    public Sprite art { get; private set; }

    public Card(int id, string deckID, string cardName, AudioClip voice1, AudioClip voice2, string text1, string text2, Sprite art)
    {
        this.id = id;
        this.deckID = deckID;
        this.cardName = cardName;
        this.voice1 = voice1;
        this.voice2 = voice2;
        this.text1 = text1;
        this.text2 = text2;
        this.art = art;
    }
}
