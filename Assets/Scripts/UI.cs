using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    // CG -> CanvasGroup
    public CanvasGroup MainMenuCG;
    public CanvasGroup SelectModeCG;
    public CanvasGroup GameplayCG;
    public CanvasGroup ReadyGoCG;
    [SerializeField] private TMP_Text QuoteText;
    [SerializeField] private Image readyImage;
    [SerializeField] private Image goImage;
    public CanvasGroup ResultCG;
    [SerializeField] private TMP_Text targetNameText;
    [SerializeField] private TMP_Text targetQuote1Text;
    [SerializeField] private TMP_Text targetQuote2Text;
    [SerializeField] private Image targetImage;
    public CanvasGroup GameOverCG;

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

    // Enable/disable the ready/go images
    public void ShowReady()
    {
        goImage.gameObject.SetActive(false);
        readyImage.gameObject.SetActive(true);
        
    }
    public void ShowGo()
    {
        readyImage.gameObject.SetActive(false);
        goImage.gameObject.SetActive(true);
    }
    
    public void HideReadyGo()
    {
        readyImage.gameObject.SetActive(false);
        goImage.gameObject.SetActive(false);
    }

    public void ShowResultCG(Card targetCard)
    {
        targetNameText.text = targetCard.cardName;
        targetQuote1Text.text = targetCard.text1;
        targetQuote2Text.text = targetCard.text2;
        targetImage.sprite = targetCard.art;
        ResultCG.gameObject.SetActive(true);
        CanvasGroupDisplayer.Show(ResultCG);
    }

    public void HideResultCG()
    {
        targetNameText.text = "";
        targetQuote1Text.text = "";
        targetQuote2Text.text = "";
        CanvasGroupDisplayer.Hide(ResultCG);
        ResultCG.gameObject.SetActive(false);
    }

    // Changes alpha evenly over the course of duration to fade in a Canvas Group
    public IEnumerator FadeInCG(float duration, CanvasGroup cg)
    {
        // Create a timer from 0 to duration
        float curTime = 0f;
        while (curTime < duration)
        {
            curTime += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(curTime / duration);
            yield return null;
        }

        cg.alpha = 1f;
    }

    public void ShowGameOverCG()
    {
        GameOverCG.gameObject.SetActive(true);
        
        CanvasGroupDisplayer.Show(GameOverCG);
    }

    public void HideGameOverCG()
    {
        CanvasGroupDisplayer.Hide(GameOverCG);
        GameOverCG.gameObject.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
     
    }
    
}
