using UnityEngine;
using UnityEngine.UI;
public class HoverOverlay : MonoBehaviour
{
    [SerializeField] private Image ghostImage;
    private RectTransform rt;
    
    // Cache the transform and immediately hide the effect (deactivate it)
    private void Awake()
    {
        rt = (RectTransform)transform;
        Hide();
    }

    // Whatever gameObject has this script (the Hover prefab) will be turned off
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // Turn on the effect for a given card
    public void Show(CardView view)
    {
        // Get the transform from the card
        RectTransform cardRT = (RectTransform)view.transform;
        // Have the effect's transform/sizing match the card
        rt.position = cardRT.position;
        rt.rotation = cardRT.rotation;
        rt.sizeDelta = cardRT.rect.size;
        rt.localScale = cardRT.localScale;
        // Get the same image
        ghostImage.sprite = view.Card.art;
        // Turn on the instantiated prefab of the hover effect
        gameObject.SetActive(true);
    }
}