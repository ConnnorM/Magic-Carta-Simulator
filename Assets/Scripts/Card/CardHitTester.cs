using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class CardHitTester
{
    // Reused every call so we don't allocate a new list every frame
    private static readonly List<RaycastResult> hits = new List<RaycastResult>();
    
    public static CardView GetCardAt(Vector2 screenPoint)
    {
        // Make sure we have an EventSystem
        if (EventSystem.current == null) return null;
        
        var data = new PointerEventData(EventSystem.current) { position = screenPoint };
        // Returns results sorted from top-most to bottom-most. First card is the one drawn on top
        EventSystem.current.RaycastAll(data, hits);
        // return the first hit whose GetComponentInParent<CardView>() isn't null, else null
        foreach (var hit in hits)
        {
            CardView view = hit.gameObject.GetComponentInParent<CardView>();
            if (view != null) return view;
        }

        return null;
    }
}