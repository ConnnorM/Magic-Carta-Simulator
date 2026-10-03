using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Text cardNameText;
    [SerializeField] private AudioSource voice1AS;
    [SerializeField] private AudioSource voice2AS;
    [SerializeField] private Text text1;
    [SerializeField] private Text text2;
    [SerializeField] private Image artImage;
    private RectTransform rt;

    private void Awake()
    {
        // Create a pointer to the transform so we can change its position
        rt = (RectTransform)transform;
    }

    public void Init(Card card)
    {
        this.cardNameText.text = card.cardName;
        this.voice1AS.clip = card.voice1;
        this.voice2AS.clip = card.voice2;
        this.text1.text = card.text1;
        this.text2.text = card.text2;
        this.artImage.sprite = card.art;
    }

    public void Place(Vector2 position, float angle)
    {
        rt.anchoredPosition = position;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
    }
}