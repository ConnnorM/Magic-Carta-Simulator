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

    public void Init(Card card)
    {
        this.cardNameText.text = card.cardName;
        this.voice1AS.clip = card.voice1;
        this.voice2AS.clip = card.voice2;
        this.text1.text = card.text1;
        this.text2.text = card.text2;
        this.artImage.sprite = card.art;

    }
}