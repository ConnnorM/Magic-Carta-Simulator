using UnityEngine;
using System.Collections.Generic;

public class PlayerDataCollector
{
    // Player
    private readonly Player player;
    // Cursor type
    private CursorInputType inputType = CursorInputType.Unknown;

    private string inputDeviceName;
    // Time
    private double q1StartTime;
    private double firstMoveTime;
    private double gotTargetTime;
    private bool isTiming;
    private bool hasMoved;
    // Position
    private Vector2 startPosition;
    private Vector2 gotTargetPosition;
    private const float MoveThreshold = 5f;
    // Round-Specific
    private int round;
    private Card target;
    private bool foundTarget;
    // Guesses
    private readonly List<int> wrongCardIds = new List<int>();

    public PlayerDataCollector(Player player)
    {
        this.player = player;
    }

    public void RoundStart(Vector2 cursorPosition, int roundNumber, Card targetCard, CursorInputType inputType, string inputDeviceName)
    {
        this.q1StartTime = Time.realtimeSinceStartupAsDouble;
        this.round = roundNumber;
        this.target = targetCard;
        this.startPosition = cursorPosition;
        this.inputType = inputType;
        this.inputDeviceName = inputDeviceName;
        isTiming = true;
        hasMoved = false;
        // Reset the list of wrong guesses
        wrongCardIds.Clear();
    }

    public void ObservePosition(Vector2 cursorPosition)
    {
        // Ignore if we aren't timing right now or we've already moved
        if (!isTiming || hasMoved) return;

        if (Vector2.Distance(cursorPosition, startPosition) > MoveThreshold)
        {
            hasMoved = true;
            firstMoveTime = Time.realtimeSinceStartupAsDouble;
        }
    }

    public void OnFoundTarget(Vector2 cursorPosition)
    {
        this.gotTargetPosition = cursorPosition;
        this.foundTarget = true;
        this.isTiming = false;
        this.gotTargetTime = Time.realtimeSinceStartupAsDouble;
        this.gotTargetPosition = cursorPosition;
    }

    public void RecordWrongGuess(Card guessed, Vector2 cursorPosition)
    {
        wrongCardIds.Add(guessed.id);

        // Written immediately, so a crash or quit mid-round doesn't lose the guess
        DataLogger.AppendRow(DataLogger.GuessFileName, DataLogger.GuessesHeader, new[]
        {
            DataLogger.SessionId,
            player.id.ToString(),
            round.ToString(),
            target.id.ToString(),
            wrongCardIds.Count.ToString(),   // guess number within this round
            guessed.id.ToString(),
            guessed.cardName,
            DataLogger.Num(Time.realtimeSinceStartupAsDouble - q1StartTime),
            DataLogger.Num(cursorPosition.x),
            DataLogger.Num(cursorPosition.y)
        });
    }
    
    // Send everything to DataLogger which writes it to the CSV
    public void WriteRow()
    {
        if (target == null) return;

        // Click-based fields stay blank if this player never found the target
        string tCorrect = "", dist = "", fMoveDelay = "";
        if (hasMoved)
        {
            fMoveDelay = DataLogger.Num(firstMoveTime - q1StartTime);
        }
        // Update fields if player was winner
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
            fMoveDelay,
            wrongCardIds.Count.ToString(),
            string.Join(";", wrongCardIds),
            inputType.ToString(),
            inputDeviceName,
            Application.isEditor ? "1" : "0"
        });
    }
}