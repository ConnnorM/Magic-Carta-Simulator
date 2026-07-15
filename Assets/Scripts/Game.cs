using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public UI UI;

    public AudioClip stahnV1;
    public AudioClip stahnV2;
    public Sprite stahnArt;
    public AudioClip ruteeV1;
    public AudioClip ruteeV2;
    public Sprite ruteeArt;
    
    // Start is called before the first frame update
    void Start()
    {
        OpenMainMenu();
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
        
        Card card1 = ScriptableObject.CreateInstance<Card>();
        card1.Init(1, "Stahn", stahnV1, stahnV2, "Th-The sword spoke!", "How in the world can you speak?", stahnArt);
        Card card2 = ScriptableObject.CreateInstance<Card>();
        card2.Init(2, "Rutee", ruteeV1, ruteeV2, "Hey Atwight? I have something to ask you...", "It's about Leon.", ruteeArt);

        Instantiate(card1);
    }
}
