using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public UI UI;
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
    }
}
