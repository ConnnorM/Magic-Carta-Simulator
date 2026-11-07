using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UIHitTester
{
    private static readonly List<RaycastResult> hits = new List<RaycastResult>();

    // Returns the topmost UI object under the point that has a component of type T, or null
    public static T GetAt<T>(Vector2 screenPoint) where T : Component
    {
        if (EventSystem.current == null) return null;
        var data = new PointerEventData(EventSystem.current) { position = screenPoint };
        EventSystem.current.RaycastAll(data, hits);
        foreach (var hit in hits)
        {
            T found = hit.gameObject.GetComponentInParent<T>();
            if (found != null) return found;
        }
        return null;
    }
}