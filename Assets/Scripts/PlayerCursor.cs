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
    [SerializeField] private float speed = 900f;
    public float Speed => speed;
    // Allows different user input devices
    private ICursorInput input;

    public bool SelectPressedThisFrame => input != null && input.SelectPressedThisFrame();
    // Readonly property to get cursor's position in screen pixels
    public Vector2 ScreenPosition => RectTransformUtility.WorldToScreenPoint(cam, rt.position);
    
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
        Canvas canvas = area.GetComponentInParent<Canvas>();
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
        if (isFrozen)
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
        if (isFrozen)
        {
            return;
        }

        // Move the position of the cursor by the given amount
        Vector2 pos = rt.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x + delta.x, area.rect.xMin, area.rect.xMax);
        pos.y = Mathf.Clamp(pos.y + delta.y, area.rect.yMin, area.rect.yMax);
        rt.anchoredPosition = pos;
    }
    
}