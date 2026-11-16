using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickInput : ICursorInput
{
    
    // readonly means it can only be set in the constructor
    private readonly Joystick joystick;
    // Device remembers which controllers belong to a player
    public Joystick Device => joystick;
    // Stores Input type for retrieval
    public CursorInputType InputType => CursorInputType.Joystick;
    private readonly string deviceName;
    public string DeviceName => deviceName;
    
    public JoystickInput(Joystick joystick)
    {
        this.joystick = joystick;
        this.deviceName = this.joystick.displayName;
    }

    public void Drive(PlayerCursor cursor, float deltaTime)
    {
        if (joystick == null) return;
        Vector2 stick = joystick.stick.ReadValue();
        // Limit the combined length of the vectors to 1 so pressing both won't exceed full speed
        Vector2 dir = Vector2.ClampMagnitude(stick, 1f);
        // Uses direction and speed to figure out how far to move each frame
        cursor.MoveBy(dir * cursor.Speed * deltaTime);
    }

    public bool SelectPressedThisFrame()
    {
        return joystick != null && joystick.trigger.wasPressedThisFrame;
    }
}