using UnityEngine;

public class MenuOption : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    public event System.Action<MenuOption> Chosen;

    public void SetHighlighted(bool value)
    {
        highlight.SetActive(value);
    }
    
    public void Choose()
    {
        Chosen?.Invoke(this);
    }
}
