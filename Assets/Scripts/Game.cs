using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    public UI UI;
    [SerializeField] private CardView cardPrefab;
    // Card parent is the Game Board, or the GameObject where card prefabs live
    [SerializeField] private Transform cardParent;
    [SerializeField] private DeckDefinition[] deckDefinitions;
    [SerializeField, Range (0.1f, 1.5f)] private float c2cSpacingScalar = 0.8f;
    [SerializeField] private AudioSource voiceSource;
    private List<Vector2> cardPositions;
    private string mode = "Easy";
    private int cardsOnBoard = 25;
    private int round = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        // OpenMainMenu();
        OpenGameplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMainMenu()
    {
        UI.ShowMainMenuCG();
        UI.HideSelectModeCG();
        UI.HideGameplayCG();
    }
    
    public void OpenSelectMode()
    {
        UI.HideMainMenuCG();
        UI.ShowSelectModeCG();
        UI.HideGameplayCG();
    }
    
    public void OpenGameplay()
    {
        UI.HideMainMenuCG();
        UI.HideSelectModeCG();
        UI.ShowGameplayCG();
        
        // Create a deck
        Deck deck = new Deck(DeckLoader.LoadDeck(deckDefinitions[0]));
        // Shuffle the deck
        deck.Shuffle();
        // Get the cards for the board
        List<Card> board = deck.SelectBoard(cardsOnBoard);
        // Generate card positions and spawn cards
        ShowBoard(board);
        // Pick a card, show text1 and play voice1, wait for correct choice, show line 2 and play voice2
        PlayRound(board);
    }

    private void PlayRound(List<Card> board)
    {
        Card target = board[round];
        // Show the first quote as text on screen
        UI.ShowQuote(target.text1);
        // Load the first quote and play it
        voiceSource.clip = target.voice1;
        voiceSource.Play();
    }

    private void ShowBoard(List<Card> board)
    {
        // Generate all the positions for cards to be placed
        cardPositions = GenerateAllCardPositions();
        
        // For each Card on the board, Instantiate, place, and initialize it
        for (int i = 0; i < board.Count; i++)
        {
            CardView view = Instantiate(cardPrefab, cardParent);
            view.Place(cardPositions[i], Random.Range(0, 360));
            // Call init to set the cards's values and spawn it
            view.Init(board[i]);
        }
    }

    private List<Vector2> GenerateAllCardPositions()
    {
        // Define list of Vector2s to return
        List<Vector2> positionsList = new List<Vector2>();
        
        // Get the bounds of the Game board
        RectTransform gameBoardArea = (RectTransform)cardParent;
        // (0,0) is the center of our canvas, so the board stretches in each direction by width/2
        float boardHalfWidth = gameBoardArea.rect.width / 2;
        float boardHalfHeight = gameBoardArea.rect.height / 2;
        
        // Get the size of the cards
        RectTransform cardArea = (RectTransform)cardPrefab.transform;
        float cardHalfWidth = cardArea.rect.width / 2;
        float cardHalfHeight = cardArea.rect.height / 2;
        
        // Define the X and Y boundaries for spawn locations
        float xBound = boardHalfWidth - (Mathf.Sqrt((cardHalfWidth * cardHalfWidth) + (cardHalfHeight * cardHalfHeight)));
        float yBound = boardHalfHeight - (Mathf.Sqrt((cardHalfWidth * cardHalfWidth) + (cardHalfHeight * cardHalfHeight)));
        
        // Generate a random position for each card
        for (int i = 0; i < cardsOnBoard; i++)
        {
            // Keep track of how many attempts we've made so we can move on if we can't find a solution easily
            int attempts = 1;
            // Create X and Y positions
            float xPos = Random.Range(-xBound, xBound);
            float yPos = Random.Range(-yBound, yBound);
            
            // Keep generating random positions until we have a valid one
            while (CheckCardPosition(xPos, yPos, c2cSpacingScalar * cardHalfWidth, positionsList) == false)
            {
                xPos = Random.Range(-xBound, xBound);
                yPos = Random.Range(-yBound, yBound);
                attempts += 1;
                if (attempts > 200)
                {
                    Debug.LogWarning($"Exceeded 200 attempts on card {i}");
                    break;
                }
            }
            
            // Add the valid position to the list
            positionsList.Add(new Vector2(xPos, yPos));
        }
        
        // Return the list of valid positions
        return positionsList;
    }

    // Keep cards a minimum center-to-center distance away from one another
    private bool CheckCardPosition(float xPos, float yPos, float minC2Cdistance, List<Vector2> finalPositions)
    {
        // Save our proposed position as a vector so we can use sqrMagnitude to find the distance
        Vector2 newPosition = new Vector2(xPos, yPos);
        // Check the newly proposed position against all other acceptable positions
        foreach (Vector2 existingPosition in finalPositions)
        {
            // Check the magnitude of the squared distances between the centers of the cards
            if ((newPosition - existingPosition).sqrMagnitude < (minC2Cdistance * minC2Cdistance))
            {
                return false;
            }
        }
        return true;
    }
    
}
