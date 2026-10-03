using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    // CG -> CanvasGroup
    public CanvasGroup MainMenuCG;
    public CanvasGroup SelectModeCG;
    public CanvasGroup GameplayCG;
    public Text QuoteText;

    public void ShowMainMenuCG()
    {
        CanvasGroupDisplayer.Show(MainMenuCG);
    }
    
    public void HideMainMenuCG()
    {
        CanvasGroupDisplayer.Hide(MainMenuCG);
    }
    
    public void ShowSelectModeCG()
    {
        CanvasGroupDisplayer.Show(SelectModeCG);
    }
    
    public void HideSelectModeCG()
    {
        CanvasGroupDisplayer.Hide(SelectModeCG);
    }
    
    public void ShowGameplayCG()
    {
        CanvasGroupDisplayer.Show(GameplayCG);
    }
    
    public void HideGameplayCG()
    {
        CanvasGroupDisplayer.Hide(GameplayCG);
    }

    public void ShowQuote(string text)
    {
        QuoteText.text = text;
    }
    
    // Update is called once per frame
    void Update()
    {
     
    }
    
}
