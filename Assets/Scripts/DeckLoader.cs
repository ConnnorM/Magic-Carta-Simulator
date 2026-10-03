using System.Collections.Generic;
using UnityEngine;

public static class DeckLoader
{
    private static int numFields = 4;
    public static List<Card> LoadDeck(DeckDefinition deckDefinition)
    {
        // Create empty list to populate and return
        List<Card> cardList = new List<Card>();
        if (deckDefinition == null || deckDefinition.CardsCSV == null)
        {
            Debug.LogWarning("Deck Definition or CSV is empty");
            return cardList;
        }
        
        // 1. Build the asset dictionaries
        // For each dictionary, fill in the values from the deckDefinition's lists
        // Sprite Dictionary
        Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in deckDefinition.ArtList)
        {
            if (sprite == null)
            {
                Debug.LogWarning("Null sprite");
                continue;
            }
            if (!spriteDict.TryAdd(sprite.name, sprite))
            {
                Debug.LogWarning($"Duplicate sprite: {sprite.name}");
            }
        }
        
        // Voice 1 Dictionary
        Dictionary<string, AudioClip> voice1Dict = new Dictionary<string, AudioClip>();
        foreach (AudioClip voice1 in deckDefinition.Voice1List)
        {
            if (voice1 == null)
            {
                Debug.LogWarning("Null voice 1 file");
                continue;
            }
            if (!voice1Dict.TryAdd(voice1.name, voice1))
            {
                Debug.LogWarning($"Duplicate voice1: {voice1.name}");
            }
        }
        
        // Voice 2 Dictionary
        Dictionary<string, AudioClip> voice2Dict = new Dictionary<string, AudioClip>();
        foreach (AudioClip voice2 in deckDefinition.Voice2List)
        {
            if (voice2 == null)
            {
                Debug.LogWarning("Null voice 2 file");
                continue;
            }
            if (!voice2Dict.TryAdd(voice2.name, voice2))
            {
                Debug.LogWarning($"Duplicate voice2: {voice2.name}");
            }
        }
        
        // 2. Get text and split into lines
        // First, get each line of the CSV (\r\n is what windows sometimes uses, so check for that too)
        string[] csvLines = deckDefinition.CardsCSV.text.Split(new[] {"\r\n", "\n"}, System.StringSplitOptions.RemoveEmptyEntries);
        
        // Loop through each line
        for (int i = 1; i < csvLines.Length; i++)
        {
            // Split up the current line on tabs (CSV was exported with tab separation)
            // Don't remove empty entries here or everything gets filled into the wrong spot if we remove empty entries
            string[] curLine = csvLines[i].Split(new[] { "\t" }, System.StringSplitOptions.None);
            
            // Make sure we got the correct number of fields
            if (curLine.Length != numFields)
            {
                Debug.LogWarning($"Number of fields does not equal {numFields}: " + curLine[0] + " " + curLine[1]);
                for (int j = 0; j < curLine.Length; j++)
                {
                    Debug.LogWarning($"Field {j}: {curLine[j]}");
                }
                continue;
            }

            // 3. Parse out the ID
            if (!int.TryParse(curLine[0], out int id))
            {
                Debug.LogWarning($"Could not parse ID: {curLine[0]}");
                continue;
            }
            
            // 4. Clean fields
            string name = curLine[1].Trim();
            string text1 = curLine[2].Trim();
            string text2 = curLine[3].Trim();
            
            // 5. Find the assets that match the name of the card we're building from this line in the CSV
            // Creates a new object that stores the asset/value in the dictionary if the key/name is found
            if (!spriteDict.TryGetValue(name, out Sprite sprite))
            {
                // Skip if anything is missing
                Debug.LogWarning($"Row {i + 1}: no sprite named '{name}'");
                continue;
            }
            if (!voice1Dict.TryGetValue(name + "V1", out AudioClip voice1))
            {
                Debug.LogWarning($"Row {i + 1}: no audio clip named '{name}'");
                continue;
            }
            if (!voice2Dict.TryGetValue(name + "V2", out AudioClip voice2))
            {
                Debug.LogWarning($"Row {i + 1}: no audio clip named '{name}'");
                continue;
            }
            
            // 6. Create the card and add it to the list of cards
            Card curCard = new Card(id, deckDefinition.DeckTag, name, voice1, voice2, text1, text2, sprite);
            cardList.Add(curCard);
        }

        // 7. Return the list of cards
        return cardList;
    }
}
