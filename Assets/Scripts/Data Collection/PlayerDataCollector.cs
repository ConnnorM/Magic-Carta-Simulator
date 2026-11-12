using UnityEngine;

public class PlayerDataCollector
{
    // Player
    private readonly Player player;
    // Time
    private double q1StartTime;
    private double gotTargetTime;
    // Position
    private Vector2 startPosition;
    private Vector2 gotTargetPosition;
    // Round-Specific
    private int round;
    private Card target;
    private bool foundTarget;

    public PlayerDataCollector(Player player)
    {
        this.player = player;
    }

    public void RoundStart(Vector2 cursorPosition, int roundNumber, Card targetCard)
    {
        this.q1StartTime = Time.realtimeSinceStartupAsDouble;
        this.round = roundNumber;
        this.target = targetCard;
        this.startPosition = cursorPosition;
    }

    public void OnFoundTarget(Vector2 cursorPosition)
    {
        this.gotTargetPosition = cursorPosition;
        this.foundTarget = true;
        this.gotTargetTime = Time.realtimeSinceStartupAsDouble;
        this.gotTargetPosition = cursorPosition;
    }
    
    // Send everything to DataLogger which writes it to the CSV
    public void WriteRow()
    {
        if (target == null) return;

        // Click-based fields stay blank if this player never found the target
        string tCorrect = "", dist = "";
        if (foundTarget)
        {
            tCorrect = DataLogger.Num(gotTargetTime - q1StartTime);
            dist = DataLogger.Num(Vector2.Distance(startPosition, gotTargetPosition));
        }

        DataLogger.AppendRow(new[]
        {
            DataLogger.SessionId,
            player.id.ToString(),
            round.ToString(),
            target.id.ToString(),
            target.cardName,
            foundTarget ? "1" : "0",
            tCorrect,
            dist,
            Application.isEditor ? "1" : "0"
        });
    }
}