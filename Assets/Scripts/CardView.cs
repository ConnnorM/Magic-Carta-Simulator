using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerClickHandler
{
    // [SerializeField] private Text cardNameText;
    // [SerializeField] private AudioSource voice1AS;
    // [SerializeField] private AudioSource voice2AS;
    // [SerializeField] private Text text1;
    // [SerializeField] private Text text2;
    [SerializeField] private Image artImage;
    private RectTransform rt;
    private Card card;
    // Create a public getter for this.card
    public Card Card => this.card;

    // Create an event called Clicked for each CardView
    // Declaring an empty list of callbacks
    public event System.Action<CardView> Clicked;

    // When this GameObject is clicked on, announce to any subscribers to the event which card was clicked. Returns itself (the CardView)
    public void OnPointerClick(PointerEventData eventData)
    {
        // When the CardView is clicked, call every subscriber and give them this (the CardView)
        // ? just handles if nobody is subscribing to Clicked (event w/o subscribers is null)
        Clicked?.Invoke(this);
    }

    private void Awake()
    {
        // Create a pointer to the transform so we can change its position
        rt = (RectTransform)transform;
    }

    public void Init(Card card)
    {
        // this.cardNameText.text = card.cardName;
        // this.voice1AS.clip = card.voice1;
        // this.voice2AS.clip = card.voice2;
        // this.text1.text = card.text1;
        // this.text2.text = card.text2;
        this.artImage.sprite = card.art;
        this.card = card;
    }

    public void Place(Vector2 position, float angle)
    {
        rt.anchoredPosition = position;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
    }
}