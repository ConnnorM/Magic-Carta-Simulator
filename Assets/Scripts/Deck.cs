using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> cardsList;
    public List<Card> CardsList => cardsList;

    public Deck(List<Card> cardsList)
    {
        this.cardsList = new List<Card>(cardsList);
    }

    // Fisher-Yates shuffling algorithm. Exactly what Python's random.shuffle does
    // Guarantees every element is involved in at least 1 swap
    public void Shuffle()
    {
        // Counting up/down doesn't actually matter
        for (int i = cardsList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            
            // Standard swapping with temporary variables
            //Card temp = cardsList[randomIndex];
            //cardsList[randomIndex] = cardsList[i];
            //cardsList[i] = temp;
            
            // A nicer way to swap using tuples
            (cardsList[i], cardsList[randomIndex]) = (cardsList[randomIndex], cardsList[i]);
        }
    }

    // Returns the subset of cards for the board
    public List<Card> SelectBoard(int cardsOnBoard)
    {
        return cardsList.GetRange(0, cardsOnBoard);
    }
}