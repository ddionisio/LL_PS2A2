using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Used by DragWidget to visualize its position in UI space
/// </summary>
public class DragCursorWidget : MonoBehaviour {
    [Header("Display")]
    public Image icon;
    public bool iconApplyNativeSize;

    //TODO: always drop invalid for now
    public bool isDropValid { get { return false; } }

    public void ApplyIcon(Sprite sprite) {
        if(icon) {
            icon.sprite = sprite;
            if(iconApplyNativeSize)
                icon.SetNativeSize();
        }
    }

    public void UpdateState(PointerEventData eventData) {
        transform.position = eventData.position;
    }
}
