using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class JoyConInput : ICursorInput
{
    private readonly Joystick joystick;
    private readonly Vector2Control hat;
    private readonly List<ButtonControl> selectButtons = new List<ButtonControl>();
    private readonly float rotationDegrees;
    public Joystick Device => joystick;

    public JoyConInput(Joystick joystick, string[] selectButtonNames, float rotationDegrees)
    {
        this.joystick = joystick;
        this.rotationDegrees = rotationDegrees;

        // The Joy-Con reports its stick as a hat switch (digital, 8 directions)
        hat = joystick.TryGetChildControl<Vector2Control>("hat");
        if (hat == null) Debug.LogWarning($"JoyConInput: no 'hat' control on {joystick.displayName}, falling back to 'stick'");

        foreach (string buttonName in selectButtonNames)
        {
            ButtonControl button = joystick.TryGetChildControl<ButtonControl>(buttonName);
            if (button != null) selectButtons.Add(button);
            else Debug.LogWarning($"JoyConInput: no button named '{buttonName}' on {joystick.displayName}");
        }
    }

    public void Drive(PlayerCursor cursor, float deltaTime)
    {
        if (joystick == null) return;

        Vector2 dir = hat != null ? hat.ReadValue() : joystick.stick.ReadValue();
        // Turn the direction to match how the controller is held
        dir = (Vector2)(Quaternion.Euler(0f, 0f, rotationDegrees) * (Vector3)dir);
        dir = Vector2.ClampMagnitude(dir, 1f);

        cursor.MoveBy(dir * cursor.Speed * deltaTime);
    }

    public bool SelectPressedThisFrame()
    {
        if (joystick == null) return false;
        foreach (ButtonControl button in selectButtons)
        {
            if (button.wasPressedThisFrame) return true;
        }
        return false;
    }
}