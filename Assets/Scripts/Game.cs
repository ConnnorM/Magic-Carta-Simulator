using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public UI UI;
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private Transform cardParent;
    public AudioClip stahnV1;
    public AudioClip stahnV2;
    public Sprite stahnArt;
    public AudioClip ruteeV1;
    public AudioClip ruteeV2;
    public Sprite ruteeArt;
    
    // Start is called before the first frame update
    void Start()
    {
        // OpenMainMenu();
        OpenGameplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMainMenu()
    {
        UI.ShowMainMenuCG();
        UI.HideSelectModeCG();
        UI.HideGameplayCG();
    }
    
    public void OpenSelectMode()
    {
        UI.HideMainMenuCG();
        UI.ShowSelectModeCG();
        UI.HideGameplayCG();
    }
    
    public void OpenGameplay()
    {
        UI.HideMainMenuCG();
        UI.HideSelectModeCG();
        UI.ShowGameplayCG();
        
        Card cardStahn = new Card(1, "Stahn", stahnV1, stahnV2, "Th-The sword spoke!",
            "How in the world can you speak?", stahnArt);
        Card cardRutee = new Card(2, "Rutee", ruteeV1, ruteeV2, "Hey Atwight? I have something to ask you...", "It's about Leon.", ruteeArt);
        
        CardView view = Instantiate(cardPrefab, cardParent);
        
        // rtStahn is a direct reference to view's transform. Yay pointers
        RectTransform rt = (RectTransform)view.transform;
        // So this line actually changes view's position
        rt.anchoredPosition = new Vector2(-150f, 0);
        // Render the card
        view.Init(cardStahn);
        
        // Setting up a new card from the prefabs and original location
        view = Instantiate(cardPrefab, cardParent);
        // Need to update rt to point at (reference) the new prefab
        rt = (RectTransform)view.transform;
        rt.anchoredPosition = new Vector2(150f, 0);
        view.Init(cardRutee);
    }
}
