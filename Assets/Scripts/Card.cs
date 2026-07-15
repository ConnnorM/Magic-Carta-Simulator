using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Magic Carta/Card")]
public class Card : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private string cardName;
    [SerializeField] private AudioClip voice1;
    [SerializeField] private AudioClip voice2;
    [SerializeField] private string text1;
    [SerializeField] private string text2;
    [SerializeField] private Sprite art;

    public int Id => id;
    public string CardName => cardName;
    public AudioClip Voice1 => voice1;
    public AudioClip Voice2 => voice2;
    public string Text1 => text1;
    public string Text2 => text2;
    public Sprite Art => art;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
