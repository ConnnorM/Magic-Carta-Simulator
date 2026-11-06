using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// These are interfaces. IPointerEnterHandler, for example, requires you to have a method in the class with this
// definition: public void OnPointerEnter(PointerEventData eventData)
public class CardView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // [SerializeField] private Text cardNameText;
    // [SerializeField] private AudioSource voice1AS;
    // [SerializeField] private AudioSource voice2AS;
    // [SerializeField] private Text text1;
    // [SerializeField] private Text text2;
    [SerializeField] private Image artImage;
    private RectTransform rt;
    [SerializeField] private Image shadowImage;
    [SerializeField] private Vector2 shadowOffset = new Vector2(10, -10);
    private RectTransform shadowRT;
    private Card card;
    // Create a public getter for this.card
    public Card Card => this.card;

    // Create an event called Clicked for each CardView: returns the CardView and point data (position)
    // Declaring an empty list of callbacks
    public event System.Action<CardView, PointerEventData> Clicked;
    // Add events for enter/exit hover. Returns the cardview that was hovered/unhovered
    public event System.Action<CardView> HoverEntered;
    public event System.Action<CardView> HoverExited;

    // When this GameObject is clicked on, announce to any subscribers to the event which card was clicked. Returns itself (the CardView)
    public void OnPointerClick(PointerEventData eventData)
    {
        // When the CardView is clicked, call every subscriber and give them this (the CardView and pointer data)
        // ? just handles if nobody is subscribing to Clicked (event w/o subscribers is null)
        Clicked?.Invoke(this, eventData);
    }

    private void Awake()
    {
        // Create a pointer to the transform so we can change its position
        rt = (RectTransform)transform;
        // Get a reference to the transform so we can access it quickly
        shadowRT = (RectTransform)shadowImage.transform;
    }

    public void Init(Card card)
    {
        // this.cardNameText.text = card.cardName;
        // this.voice1AS.clip = card.voice1;
        // this.voice2AS.clip = card.voice2;
        // this.text1.text = card.text1;
        // this.text2.text = card.text2;
        this.artImage.sprite = card.art;
        // Set shadowImage's sprite to just be the card's art so we get the right shape/size
        this.shadowImage.sprite = card.art;
        this.card = card;
    }

    public void Place(Vector2 position, float angle)
    {
        // Place the actual card
        rt.anchoredPosition = position;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
        
        // Place the shadow
        // We feed in a vector rotated the opposite direction of the card's rotation so that when the parent
        // applies the actual rotation, they cancel out and we get down/right offset
        shadowRT.anchoredPosition = Quaternion.Inverse(rt.localRotation) * (Vector3)shadowOffset;
    }

    // When a pointer enters a cardview, turn hover on
    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverEntered?.Invoke(this);
    }

    // When a pointer exits a cardview, turn hover off
    public void OnPointerExit(PointerEventData eventData)
    {
        HoverExited?.Invoke(this);
    }
}