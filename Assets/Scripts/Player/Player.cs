using UnityEngine;
public class Player
{
    // Links a player to their cursor
    public int id { get; }
    // Links a player to their stats
    public string name { get; }
    // Initialize the player's points
    public int points { get; private set; }
    public int streak { get; private set; }
    
    // Data collector
    public PlayerDataCollector DataCollector { get; private set; }

    public Player(int playerId, string playerName)
    {
        this.id = playerId;
        this.name = playerName;
        points = 0;
        streak = 0;
        // Create a new data collector and give it this player
        DataCollector = new PlayerDataCollector(this);
    }

    public void AddPoints(int newPoints)
    {
        this.points += newPoints;
    }

    public void IncreaseStreak()
    {
        streak += 1;
    }

    public void ResetStreak()
    {
        streak = 0;
    }

    public void ResetForNewGame()
    {
        points = 0;
        streak = 0;
    }
}