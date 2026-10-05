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
    [SerializeField, Range (0.5f, 3.0f)] private float c2cSpacingScalar = 1.75f;
    [SerializeField, Range(0.5f, 1.0f)] private float xBoundScalar = 0.7f;
    [SerializeField, Range(0.5f, 1.0f)] private float yBoundScalar = 0.65f;
    
    // Audio Stuff
    [SerializeField] private AudioManager audioManager;
    
    private List<Vector2> cardPositions;
    private string mode = "Easy";
    private int cardsOnBoard = 25;
    private int round = 0;
    private Card targetCard = null;

    private CardView targetView = null;
    // Keeps track of when players can start clicking and when targetCard has been found
    private bool roundInProgress = false;
    
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
        UI.HideResultCG();
        UI.HideGameOverCG();
    }
    
    public void OpenSelectMode()
    {
        UI.HideMainMenuCG();
        UI.ShowSelectModeCG();
        UI.HideGameplayCG();
        UI.HideResultCG();
        UI.HideGameOverCG();
    }
    
    public void OpenGameplay()
    {
        UI.HideMainMenuCG();
        UI.HideSelectModeCG();
        UI.ShowGameplayCG();
        UI.HideResultCG();
        UI.HideGameOverCG();

        roundInProgress = false;
        
        // Load and start background music
        audioManager.PlayRandomMusic();
        
        // Create a deck
        Deck deck = new Deck(DeckLoader.LoadDeck(deckDefinitions[0]));
        // Shuffle the deck
        deck.Shuffle();
        // Get the cards for the board
        List<Card> board = deck.SelectBoard(cardsOnBoard);
        // Generate card positions and spawn cards
        ShowBoard(board);
        
        // Pick a card, show text1 and play voice1, wait for correct choice, show line 2 and play voice2
        StartCoroutine(StartGame(board));
    }

    private IEnumerator StartGame(List<Card> board)
    {
        // On the first round, do the Ready Go countdown
        yield return StartCoroutine(ShowReadyGo());
        
        // MAIN GAMEPLAY LOOP CONTROL: play rounds until condition is met
        while (round < 5)
        {
            targetCard = board[round];
            // Waits until a round has finished
            yield return StartCoroutine(PlayRound());
            round++;
        }
        // Full game is over!
        UI.ShowGameOverCG();
        audioManager.PlaySoundEffect(Sfx.YouWin);
        
    }

    private IEnumerator PlayRound()
    {
        // Show the first quote as text on screen
        UI.ShowQuote(targetCard.text1);
        // Load the first quote and play it
        audioManager.PlayVoice(targetCard.voice1);
        // Officially start the round
        roundInProgress = true;
        
        // Don't progress the round until the round has finished (targetCard was selected)
        yield return new WaitUntil(() => !roundInProgress);
        
        // Wait till the Correct card sound effect has stopped
        yield return null;
        yield return new WaitWhile(() => audioManager.IsSfxPlaying);
        
        // Remove current target from the screen
        yield return new WaitForSeconds(0.5f);
        Destroy(targetView.gameObject);
        
        // Fade in the full result: card image, text1 + text2, play voice2 and don't move on till voice2 finishes
        UI.ResultCG.alpha = 0f;
        UI.ShowResultCG(targetCard);
        yield return StartCoroutine(UI.FadeInCG(0.2f, UI.ResultCG));
        
        audioManager.PlayVoice(targetCard.voice2);
        // Wait one frame to give time for everything to load
        yield return null;
        yield return new WaitWhile(() => audioManager.IsVoicePlaying);
        
        // Wait for a bit after voice2 ends before removing everything off the screen
        yield return new WaitForSeconds(2.5f);
        
        // Remove result and quote1
        UI.HideResultCG();
        UI.ShowQuote("");
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator ShowReadyGo()
    {
        //Wait a bit
        yield return new WaitForSeconds(1.0f);
        
        // Ready fades in
        UI.ReadyGoCG.alpha = 0f;
        UI.ShowReadyGo("READY...");
        yield return StartCoroutine(UI.FadeInCG(0.3f, UI.ReadyGoCG));
        
        // Wait a bit
        yield return new WaitForSeconds(2.0f);
        // Go!
        UI.ShowReadyGo("GO!");
        // Wait a bit
        yield return new WaitForSeconds(0.75f);
        // Erase text
        UI.ShowReadyGo("");
    }

    private void ShowBoard(List<Card> board)
    {
        // Generate all the positions for cards to be placed
        cardPositions = GenerateAllCardPositions();
        
        // For each Card on the board, Instantiate, place, and initialize it
        for (int i = 0; i < board.Count; i++)
        {
            CardView view = Instantiate(cardPrefab, cardParent);
            // Register Game's OnCardClicked method as a subscriber to CardView view's OnCardClicked event, Clicked
            view.Clicked += OnCardClicked;
            view.Place(cardPositions[i], Random.Range(0, 360));
            // Call init to set the cards's values and spawn it
            view.Init(board[i]);
        }
    }

    private void OnCardClicked(CardView clickedView)
    {
        if (!roundInProgress)
        {
            return;
        }
        if (clickedView.Card == targetCard)
        {
            audioManager.PlaySoundEffect(Sfx.SelectCorrectCard);
            targetView = clickedView;
            roundInProgress = false;
        }
        else
        {
            audioManager.PlaySoundEffect(Sfx.SelectIncorrectCard);
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
        // This way just keeps all the cards inside the screen
        // float xBound = boardHalfWidth - (Mathf.Sqrt((cardHalfWidth * cardHalfWidth) + (cardHalfHeight * cardHalfHeight)));
        // float yBound = boardHalfHeight - (Mathf.Sqrt((cardHalfWidth * cardHalfWidth) + (cardHalfHeight * cardHalfHeight)));
        // This way centers them more. Looks better when using the correct aspect ratio
        float xBound = boardHalfWidth * xBoundScalar;
        float yBound = boardHalfHeight * yBoundScalar;
        
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
