using UnityEngine;
public class Player
{
    // Links a player to their cursor
    public int id { get; }
    // Links a player to their stats
    public string name { get; }
    // Initialize the player's points
    public int points { get; private set; }

    public Player(int playerId, string playerName)
    {
        this.id = playerId;
        this.name = playerName;
        points = 0;
    }

    public void AddPoints(int newPoints)
    {
        this.points += newPoints;
    }
}