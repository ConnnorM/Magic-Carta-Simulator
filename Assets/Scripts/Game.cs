using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    private int totalCardsInRound = 1;
    private int round = 0;
    private Card targetCard = null;

    private CardView targetView = null;
    // Keeps track of when players can start clicking and when targetCard has been found
    private bool roundInProgress = false;
    
    // Create containers for the X mark to appear if a wrong answer is made
    // Need container to determine draw order, use coordinate space easily, and all X's can be removed at once
    [SerializeField] private RectTransform xMarkParent;
    // Prefabs are templates. We could in theory have multiple X's on screen at once, so we can't just
    // make one object like we can with other things
    [SerializeField] private RectTransform xMarkPrefab;
    
    // Hover stuff
    [SerializeField] private HoverOverlay hoverOverlayPrefab;
    [SerializeField] private RectTransform hoverOverlayParent;
    private List<HoverOverlay> playerOverlays = new List<HoverOverlay>();
    private bool hoverEnabled = false;
    
    // Cursor Stuff
    [SerializeField] private PlayerCursor cursorPrefab;
    [SerializeField] private RectTransform cursorParent;
    private List<Player> players = new List<Player>();
    private List<PlayerCursor> cursors = new List<PlayerCursor>();
    private List<CardView> hoveredViews = new List<CardView>();
    [SerializeField] private float penaltySeconds = 3f;
    [SerializeField] private List<PlayerBubble> playerBubbles = new List<PlayerBubble>();

    [SerializeField] private Vector2[] cursorStartPositions =
    {
        new Vector2(-150f, 50f),
        new Vector2(150f, 50f),
        new Vector2(-150f, -50f),
        new Vector2(150f, -50f),

    };
    
    // Camera stuff
    private Camera uiCamera;
    
    // Points/winning stuff
    private Player winner = null;
    
    // Quit menu stuff
    [SerializeField] private MenuOption yesOption;
    [SerializeField] private MenuOption noOption;
    private bool menuOpen = false;
    private bool isRestarting = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to quit menu events
        yesOption.Chosen += OnQuitYes;
        noOption.Chosen += OnQuitNo;
        // OpenMainMenu();
        OpenGameplay();
    }

    // Update is called once per frame
    void Update()
    {
        // If a cursor exists and we have a mouse, AND the mouse moved in the last frame, move the cursor
        if (cursors.Count > 0 && Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0f)
        {
            cursors[0].MoveToScreenPoint(Mouse.current.position.ReadValue());
        }

        // If the menu is open, don't look at cards. Just handle the menu
        if (menuOpen)
        {
            UpdateMenu();
            return;
        }
        
        for (int i = 0; i < cursors.Count; i++)
        {
            // Do not update the hoveredView if the cursor is currently penalized
            if (cursors[i].IsPenalized) continue;
            
            CardView under = UIHitTester.GetAt<CardView>(cursors[i].ScreenPosition);
            if (under != hoveredViews[i]) SetHover(i, under);
            if (cursors[i].SelectPressedThisFrame && under != null)
                HandleCardSelected(players[i], under, cursors[i].ScreenPosition);
        }
    }

    public void OpenMainMenu()
    {
        Cursor.visible = true;
        UI.ShowMainMenuCG();
        UI.HideSelectModeCG();
        UI.HideGameplayCG();
        UI.HideResultCG();
        UI.HideGameOverCG();
    }
    
    public void OpenSelectMode()
    {
        Cursor.visible = true;
        UI.HideMainMenuCG();
        UI.ShowSelectModeCG();
        UI.HideGameplayCG();
        UI.HideResultCG();
        UI.HideGameOverCG();
    }
    
    public void OpenGameplay()
    {
        // Hide the operating system's mouse icon so only the hand represents the cursor
        Cursor.visible = false;
        UI.HideMainMenuCG();
        UI.HideSelectModeCG();
        UI.ShowGameplayCG();
        UI.HideResultCG();
        UI.HideGameOverCG();
        menuOpen = false;
        UI.HideQuitMenuCG();

        // Set up the round
        roundInProgress = false;
        hoverEnabled = false;
        foreach (HoverOverlay playerOverlay in playerOverlays) playerOverlay.Hide();
        
        // Set up players
        if (players.Count == 0)
        {
            // Make a new player and add to the list of players
            Player p1 = new Player(1, "Pascal");
            players.Add(p1);
            // Set up the player bubble
            if (playerBubbles.Count > 0)
            {
                // Connect the player to a bubble
                playerBubbles[0].Bind(p1);
            }

            // Make sure each player starts a new game at default values
            foreach (Player p in players)
            {
                p.ResetForNewGame();
            }
            RefreshPlayerBubbles();
            
            // Add a null entry in HoveredViews since the player isn't hovering anything
            hoveredViews.Add(null);
            // Add a hover overlay to our list per player
            playerOverlays.Add(Instantiate(hoverOverlayPrefab, hoverOverlayParent));
            
            // Instantiate a new cursor
            PlayerCursor cursor = Instantiate(cursorPrefab, cursorParent);
            // Give the cursor its player
            cursor.Init(p1, cursorParent, new KeyboardInput(Keyboard.current));
            // Add the cursor to the list of cursors
            cursors.Add(cursor);
        }
        
        // Reset all the cursors to starting position
        for (int i = 0; i < cursors.Count; i++)
        {
            // Make the cursors invisible, they fade in when GO! Fades out
            cursors[i].CG.alpha = 0f;
            cursors[i].GoToStartPosition(GetStartPosition(i));
        }
        // Don't allow cursors to move until they've appeared
        SetFrozenCursors(true);
        
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
        
        // Create the canvas and set up the camera
        Canvas canvas = xMarkParent.GetComponentInParent<Canvas>();
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        
        // Pick a card, show text1 and play voice1, wait for correct choice, show line 2 and play voice2
        StartCoroutine(StartGame(board));
    }

    private IEnumerator StartGame(List<Card> board)
    {
        // On the first round, do the Ready Go countdown
        yield return StartCoroutine(ShowReadyGo());
        
        // MAIN GAMEPLAY LOOP CONTROL: play rounds until condition is met
        while (round < totalCardsInRound)
        {
            targetCard = board[round];
            // Waits until a round has finished
            yield return StartCoroutine(PlayRound());
            round++;
        }
        // Full game is over!
        UI.GameOverCG.alpha = 0f;
        UI.ShowGameOverCG();
        yield return StartCoroutine(UI.FadeCG(0.2f, UI.GameOverCG, 0, 1));
        audioManager.PlaySoundEffect(Sfx.YouWin);
        yield return new WaitWhile(() => audioManager.IsSfxPlaying);
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(UI.FadeCG(0.2f, UI.GameOverCG, 1, 0));
        UI.HideGameOverCG();
        
        // Show the quit menu
        UI.QuitMenuCG.alpha = 0f;
        UI.ShowQuitMenuCG();
        yield return StartCoroutine(UI.FadeCG(0.2f, UI.QuitMenuCG, 0, 1));
        yield return new WaitForSeconds(0.5f);
        menuOpen = true;
        hoverEnabled = false;


    }

    private IEnumerator PlayRound()
    {
        // Show the points for this round
        UI.ShowNextPointsCG(round + 1);
        // Show the first quote as text on screen
        UI.ShowQuote(targetCard.text1);
        // Load the first quote and play it
        audioManager.PlayVoice(targetCard.voice1);
        // Officially start the round
        roundInProgress = true;
        hoverEnabled = true;
        for (int i = 0; i < playerOverlays.Count; i++)
        {
            if (hoveredViews[i] != null)
            {
                playerOverlays[i].Show(hoveredViews[i]);
            }
        }

        // Don't progress the round until the round has finished (targetCard was selected)
        yield return new WaitUntil(() => !roundInProgress);
        
        // Wait till the Correct card sound effect has stopped
        yield return null;
        yield return new WaitWhile(() => audioManager.IsSfxPlaying);
        
        // Remove current target from the screen
        yield return new WaitForSeconds(0.5f);
        Destroy(targetView.gameObject);
        for (int i = 0; i < hoveredViews.Count; i++)
        {
            hoveredViews[i] = null;
        }
        // Remove any X markers left on the screen immediately
        foreach (Transform xMarker in xMarkParent) Destroy(xMarker.gameObject);
        
        // Fade in the full result: card image, text1 + text2, play voice2 and don't move on till voice2 finishes
        UI.ResultCG.alpha = 0f;
        UI.ShowResultCG(targetCard, ScoreManager.GetBasePoints(round + 1), ScoreManager.GetStreakPoints(round + 1, winner.streak));
        // Start the cursor fade-out first and disable them
        SetFrozenCursors(true);
        StartCoroutine(FadeCursors(1, 0, 0.2f));
        yield return StartCoroutine(UI.FadeCG(0.2f, UI.ResultCG, 0, 1));
        
        
        audioManager.PlayVoice(targetCard.voice2);
        // Wait one frame to give time for everything to load
        yield return null;
        yield return new WaitWhile(() => audioManager.IsVoicePlaying);
        
        // Wait for a bit after voice2 ends before removing everything off the screen
        yield return new WaitForSeconds(2.5f);
        
        // Remove quote1
        UI.ShowQuote("");
        // Fade the result CG back out
        Coroutine resultFade = StartCoroutine(UI.FadeCG(0.2f, UI.ResultCG, 1, 0));
        // Fade cursors back in and reenable them
        Coroutine cursorFade = StartCoroutine(FadeCursors(0, 1, 0.2f));
        StartCoroutine(UI.FadeCG(0.2f, UI.NextPointsCG, 1, 0));
        // Wait until the resultFade is over to hide the result CG
        yield return resultFade;
        UI.HideResultCG();
        // If cursorFade isn't done, wait for that to be done too
        yield return cursorFade;
        SetFrozenCursors(false);
        yield return new WaitForSeconds(1.2f);
    }

    private IEnumerator ShowReadyGo()
    {
        //Wait a bit
        yield return new WaitForSeconds(1.0f);
        
        // Ready fades in
        UI.ReadyGoCG.alpha = 0f;
        UI.ShowReady();
        yield return StartCoroutine(UI.FadeCG(0.3f, UI.ReadyGoCG, 0, 1));
        
        // Wait a bit
        yield return new WaitForSeconds(2.0f);
        // Go!
        UI.ShowGo();
        // Wait a bit
        yield return new WaitForSeconds(0.75f);
        // Fade out text and fade in cursors
        Coroutine fadeGO = StartCoroutine(UI.FadeCG(0.3f, UI.ReadyGoCG, 1, 0));
        Coroutine fadeCurorsIn = StartCoroutine(FadeCursors(0, 1, 0.3f));
        yield return fadeGO;
        UI.HideReadyGo();
        yield return fadeCurorsIn;
        SetFrozenCursors(false);
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
            view.Clicked += HandleCardClicked;
            view.Place(cardPositions[i], Random.Range(0, 360));
            // Call init to set the cards's values and spawn it
            view.Init(board[i]);
            
        }
    }

    // OnCardClicked, but for all input devices
    private void HandleCardSelected(Player player, CardView view, Vector2 screenPoint)
    {
        // A round is not in progress, so ignore card clicks
        if (!roundInProgress)
        {
            return;
        }
        // Pointer selected the correct card
        if (view.Card == targetCard)
        {
            audioManager.PlaySoundEffect(Sfx.SelectCorrectCard);
            targetView = view;
            foreach (HoverOverlay playerOverlay in playerOverlays) playerOverlay.Hide();
            roundInProgress = false;
            hoverEnabled = false;
            // Set the round winner and handle scoring
            winner = player;
            // reset other's streaks
            winner.IncreaseStreak();
            foreach (Player p in players)
            {
                if (p != player)
                {
                    p.ResetStreak();
                }
            }
            // Add 1 here since round starts at 0. Everything in ScoreManager assumes correct round number
            int points = ScoreManager.GetTotalPoints(round + 1, winner.streak);
            winner.AddPoints(points);
            // Update points on the bubble and streak right away
            RefreshPlayerBubbles();

        }
        // Pointer selected the incorrect card
        else
        {
            // Incorrect sound effect
            audioManager.PlaySoundEffect(Sfx.SelectIncorrectCard);
            
            // Place the X marker now
            // Convert the pointer location to Canvas units (what the X marker uses)
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(xMarkParent, screenPoint,
                    uiCamera, out Vector2 localPoint))
            {
                // Instantiate the X Marker prefab
                RectTransform xMark = Instantiate(xMarkPrefab, xMarkParent);
                // Local point is what the conversion above returns
                xMark.anchoredPosition = localPoint;
                
                // Remove the X Mark after 3 seconds if it's not already gone
                Destroy(xMark.gameObject, penaltySeconds);
            }
            // Freeze the cursor for some time and animate the jiggle
            cursors[players.IndexOf(player)].StartPenalty(penaltySeconds);
        }
    }
    
    // HandleCardSelected but for mouse clicks
    private void HandleCardClicked(CardView clickedView, PointerEventData eventData)
    {
        HandleCardSelected(players[0], clickedView, eventData.position);
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

    private void SetHover(int playerIndex, CardView view)
    {
        hoveredViews[playerIndex] = view;
        if (!hoverEnabled || view == null)
        {
            playerOverlays[playerIndex].Hide();
            return;
        }
        playerOverlays[playerIndex].Show(view);
    }

    // Freezes/unfreezes all cursors
    private void SetFrozenCursors(bool input)
    {
        foreach (PlayerCursor curCursor in cursors)
        {
            curCursor.SetFrozen(input);
        }
    }

    // Coroutine for fading all the cursors out
    private IEnumerator FadeCursors(float from, float to, float duration)
    {
        foreach (PlayerCursor curCursor in cursors)
        {
            StartCoroutine(UI.FadeCG(duration, curCursor.CG, from, to));
        }

        yield return new WaitForSeconds(duration);
    }

    // Gives each player their own unique starting position
    private Vector2 GetStartPosition(int playerIndex)
    {
        if (cursorStartPositions == null || cursorStartPositions.Length == 0)
        {
            return Vector2.zero;
        }
        return cursorStartPositions[playerIndex];
    }

    private void RefreshPlayerBubbles()
    {
        foreach (PlayerBubble bubble in playerBubbles) bubble.Refresh();
    }

    public void UpdateMenu()
    {
        // Update menu is called each frame in update. By default yes/noHovers are false if someone unHovers
        bool yesHover = false;
        bool noHover = false;

        for (int i = 0; i < cursors.Count; i++)
        {
            // if the penalty is somehow still running, don't let them move. Shouldn't be possible though
            if (cursors[i].IsPenalized)
            {
                continue;
            }

            // If a menuoption is hovered over, store it in under
            MenuOption under = UIHitTester.GetAt<MenuOption>(cursors[i].ScreenPosition);

            if (under == yesOption) yesHover = true;
            if (under == noOption) noHover = true;

            // Whoever pressed select first while over an option selects it
            if (under != null && cursors[i].SelectPressedThisFrame)
            {
                under.Choose();
                return;
            }
        }
        yesOption.SetHighlighted(yesHover);
        noOption.SetHighlighted(noHover);
    }

    private void OnQuitYes(MenuOption option)
    {
        menuOpen = false;
        Cursor.visible = true;
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnQuitNo(MenuOption option)
    {
        if (isRestarting) return;
        menuOpen = false;
        yesOption.SetHighlighted(false);
        noOption.SetHighlighted(false);
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        isRestarting = true;
        SetFrozenCursors(true);
        
        // Fade screen to black
        yield return StartCoroutine(UI.FadeCG(0.5f, UI.BlackoutCG, 0, 1));
        
        // Reset the board
        UI.HideQuitMenuCG();
        foreach(Transform card in cardParent) Destroy(card.gameObject);
        foreach(Transform xMark in xMarkParent) Destroy(xMark.gameObject);
        for (int i = 0; i < hoveredViews.Count; i++)
        {
            hoveredViews[i] = null;
        }

        // Reset all variables
        round = 0;
        targetCard = null;
        targetView = null;
        winner = null;
        
        OpenGameplay();

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(UI.FadeCG(0.5f, UI.BlackoutCG, 1, 0));
        isRestarting = false;
    }
    
}
