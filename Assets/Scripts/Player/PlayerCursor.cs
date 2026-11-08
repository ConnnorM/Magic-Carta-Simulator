using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCursor : MonoBehaviour
{
    // Image and display stuff
    [SerializeField] private Image handImage;
    [SerializeField] private TMP_Text playerNumText;
    [SerializeField] private CanvasGroup cg;
    // A public getter for cg
    public CanvasGroup CG => cg;
    private RectTransform rt;

    private bool isFrozen = false;
    // Holds the particular cursor area that this cursor lives on
    private RectTransform area;
    // Camera used for screen-to-local coordinates conversion
    private Camera cam;
    private Player player;
    // A public getter for the player
    public Player Player => player;
    // Speed
    [SerializeField] private float speed = 800f;
    public float Speed => speed;
    // Allows different user input devices
    private ICursorInput input;

    public bool SelectPressedThisFrame => !isPenalized && input != null && input.SelectPressedThisFrame();
    // Readonly property to get cursor's position in screen pixels
    public Vector2 ScreenPosition => RectTransformUtility.WorldToScreenPoint(cam, rt.position);
    
    // Wrong answer time-out
    [SerializeField] public float jiggleDistance = 32f;
    [SerializeField] public float jiggleFPS = 10f; // STEPS PER SECOND. Higher = faster
    private bool isPenalized = false;
    public bool IsPenalized => isPenalized;
    
    private void Awake()
    {
        rt = (RectTransform)transform;
    }

    public void Init(Player player, RectTransform area, ICursorInput input)
    {
        this.player = player;
        this.area = area;
        this.input = input;
        
        // Find the canvas that contains the cursor area and pick null or its camera
        // Pick the camera needed to convert screen positions into canvas positions for this Canvas' render mode
        Canvas canvas = area.GetComponentInParent<Canvas>().rootCanvas;
        cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        
        // Set playerNumText
        this.playerNumText.text = player.id + "P";
    }

    public void Update()
    {
        if (input == null)
        {
            return;
        }
        input.Drive(this, Time.deltaTime);
    }

    public void MoveToScreenPoint(Vector2 screenPoint)
    {
        // If we've frozen the cursor, don't move it
        if (isFrozen || isPenalized)
        {
            return;
        }
        // Tell it which camera renders the Canvas since screen pixels map to  canvas positions depending on render mode
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(area, screenPoint, cam,
                out Vector2 localPoint))
        {
            rt.anchoredPosition = localPoint;
        }
    }

    public void SetFrozen(bool value)
    {
        isFrozen = value;
    }

    // moves the cursor by a set distance, clamped by the cursor area
    public void MoveBy(Vector2 delta)
    {
        if (isFrozen || isPenalized)
        {
            return;
        }

        // Move the position of the cursor by the given amount
        Vector2 pos = rt.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x + delta.x, area.rect.xMin, area.rect.xMax);
        pos.y = Mathf.Clamp(pos.y + delta.y, area.rect.yMin, area.rect.yMax);
        rt.anchoredPosition = pos;
    }

    public void StartPenalty(float duration)
    {
        // Do nothing if they're already penalized
        if (isPenalized) return;

        // Start a coroutine to penalize the incorrect guess
        StartCoroutine(PenaltyRoutine(duration));
    }

    private IEnumerator PenaltyRoutine(float duration)
    {
        isPenalized = true;
        // Get the locations to jump to
        Vector2 center = rt.anchoredPosition;
        Vector2 downLeft = center + new Vector2(-jiggleDistance, -jiggleDistance);
        Vector2 upLeft = center + new Vector2(-jiggleDistance, jiggleDistance);
        Vector2 downRight = center + new Vector2(jiggleDistance, -jiggleDistance);
        Vector2 upRight = center + new Vector2(jiggleDistance, jiggleDistance);

        // Order of the steps in the jiggle loop
        Vector2[] steps = { downLeft, upLeft, downRight, upRight };
        
        // Set up the 1 step per frame variables
        float stepTime = 1f / jiggleFPS; // Get the interval of jiggles
        //float animationLength = steps.Length * stepTime; // How long is each individual cycle of steps
        float timeElapsed = 0f; // How much time has passed since starting
        
        while (timeElapsed < duration)
        {
            // Determine which step in the animation to be at based on elapsed time
            // % wraps index back to 0
            int index = (int)(timeElapsed / stepTime) % steps.Length;
            // Update position
            rt.anchoredPosition = steps[index];
            timeElapsed += Time.deltaTime;
            // Pauses execution until the next frame
            yield return null;
        }
        // Ensure we finish at the center position
        rt.anchoredPosition = center;

        // Turn off the penalty
        isPenalized = false;

    }

    public void GoToStartPosition(Vector2 startPosition)
    {
        rt.anchoredPosition = startPosition;
    }
    
}