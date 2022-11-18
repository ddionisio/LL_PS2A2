using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DragWidgetSpace {
    None,
    UI,
    World
}

public struct DragWidgetSignalInfo {
    public DragWidget dragWidget;
    public DragWidgetSpace dragWidgetSpace;
    public PointerEventData pointerData; //null if dragWidgetSpace == None
}
