using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputDiagnostics : MonoBehaviour
{
    private Vector2 lastHat;
    private double lastChangeTime;

    void Update()
    {
        // Slow frames mean the game itself is stalling
        if (Time.unscaledDeltaTime > 0.04f)
            Debug.LogWarning($"Slow frame: {Time.unscaledDeltaTime * 1000f:F0} ms (frame {Time.frameCount})");

        // What the controller reports when you change direction
        foreach (Joystick js in Joystick.all)
        {
            Vector2Control hat = js.TryGetChildControl<Vector2Control>("hat");
            if (hat == null) continue;
            Vector2 v = hat.ReadValue();
            if (v != lastHat)
            {
                double now = Time.realtimeSinceStartupAsDouble;
                Debug.Log($"hat {lastHat} -> {v}  (+{(now - lastChangeTime) * 1000.0:F0} ms, frame {Time.frameCount})");
                lastHat = v;
                lastChangeTime = now;
            }
        }
    }
}