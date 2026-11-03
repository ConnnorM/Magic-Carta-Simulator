using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCursor : MonoBehaviour
{
    [SerializeField] private Image handImage;
    [SerializeField] private TMP_Text playerNumText;
    private RectTransform rt;
    // Holds the particular cursor area that this cursor lives on
    private RectTransform area;
    // Camera used for screen-to-local coordinates conversion
    private Camera cam;
    private Player player;
    // A public getter for the player
    public Player Player => player;
    
    public void Awake()
    {
        rt = (RectTransform)transform;
    }

    public void Init(Player player, RectTransform area)
    {
        this.player = player;
        this.area = area;

        // AI nonsense tbh :(
        // Find the canvas that contains the cursor area and pick null or its camera
        // Make the hand follow the mouse
        Canvas canvas = area.GetComponentInParent<Canvas>();
        cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        
        // Set playerNumText
        this.playerNumText.text = player.id + "P";
    }

    public void MoveToScreenPoint(Vector2 screenPoint)
    {
        // Tell it which camera renders the Canvas since screen pixels map to  canvas positions depending on render mode
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(area, screenPoint, cam,
                out Vector2 localPoint))
        {
            rt.anchoredPosition = localPoint;
        }
    }
    
}