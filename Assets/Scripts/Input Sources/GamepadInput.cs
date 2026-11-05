using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadInput : ICursorInput
{
    // readonly means it can only be set in the constructor
    private readonly Gamepad pad;
    // Device rememebers which controllers belong to a player
    public Gamepad Device => pad;
    
    public GamepadInput(Gamepad pad)
    {
        this.pad = pad;
    }

    public void Drive(PlayerCursor cursor, float deltaTime)
    {
        Vector2 stick = pad.leftStick.ReadValue();
        Vector2 dpad = pad.dpad.ReadValue();
        // Limit the combined length of the vectors to 1 so pressing both won't exceed full speed
        Vector2 dir = Vector2.ClampMagnitude(stick + dpad, 1f);
        // Uses direction and speed to figure out how far to move each frame
        cursor.MoveBy(dir * cursor.Speed * deltaTime);
    }

    public bool SelectPressedThisFrame()
    {
        return pad.buttonEast.wasPressedThisFrame;
    }
}
