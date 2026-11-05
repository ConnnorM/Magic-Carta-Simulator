using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInput : ICursorInput
{
    // readonly means it can only be set in the constructor
    private readonly Keyboard board;
    // Device rememebers which input device belong to a player
    public Keyboard Device => board;
    
    public KeyboardInput(Keyboard board)
    {
        this.board = board;
    }

    public void Drive(PlayerCursor cursor, float deltaTime)
    {
        if (board == null)
        {
            return;
        }
        Vector2 dir = new Vector2(0, 0);
        // If directional key is pressed, set the corresponding direction
        // If opposing keys are pressed, cancel them out
        dir.x = (board.dKey.isPressed ? 1f : 0f) - (board.aKey.isPressed ? 1f : 0f); 
        dir.y = (board.wKey.isPressed ? 1f : 0f) - (board.sKey.isPressed ? 1f : 0f);
        dir = Vector2.ClampMagnitude(dir, 1f);
        
        // Uses direction and speed to figure out how far to move each frame
        cursor.MoveBy(dir * cursor.Speed * deltaTime);
    }

    public bool SelectPressedThisFrame()
    {
        if (board == null)
        {
            return false;
        }

        return board.spaceKey.wasPressedThisFrame;
    }
}