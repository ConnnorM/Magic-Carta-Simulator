using TMPro;
using UnityEngine;

public class PlayerBubble : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text streakText;
    private Player player;

    // Match up a player to their bubble
    public void Bind(Player player)
    {
        this.player = player;
        Refresh();
    }

    public void Refresh()
    {
        if (player == null) return;
        nameText.text = player.name;
        pointsText.text = player.points + " pts";
        if (player.streak >= 2)
        {
            streakText.text = player.streak + "-card combo!";
        }
        else
        {
            streakText.text = "";
        }
    }
}
