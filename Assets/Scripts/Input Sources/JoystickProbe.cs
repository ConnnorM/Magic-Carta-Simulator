using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// Temporary: logs every button/axis that changes on any Joystick, so you can see what each physical input is called
// L JOYCON MAPPING when holding sideways:
    // trigger: down
    // button2: right
    // button3: left
    // button4: up
public class JoystickProbe : MonoBehaviour
{
    private readonly Dictionary<string, float> last = new Dictionary<string, float>();

    void Update()
    {
        foreach (Joystick js in Joystick.all)
        {
            foreach (InputControl control in js.allControls)
            {
                if (control.noisy) continue;
                // ButtonControl derives from AxisControl, so this covers buttons, axes and hat directions
                if (!(control is AxisControl axis)) continue;

                float value = axis.ReadValue();
                last.TryGetValue(control.path, out float previous);
                if (Mathf.Abs(value - previous) > 0.2f)
                {
                    Debug.Log($"{js.displayName}: {control.name} ({control.path}) = {value:F2}");
                    last[control.path] = value;
                }
            }
        }
    }
}