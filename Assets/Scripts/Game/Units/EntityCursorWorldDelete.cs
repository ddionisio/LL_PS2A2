using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this along with entity and drag
/// </summary>
public class EntityCursorWorldDelete : MonoBehaviour {
    public M8.EntityBase entity;
    public DragSurfaceSnap drag; //TODO: refactor Drag if needed

    
    void OnDestroy() {
        if(drag)
            drag.dragEndCallback -= OnDragEnd;
    }

    void Awake() {
        if(!drag)
            drag = GetComponent<DragSurfaceSnap>();

        if(!entity)
            entity = GetComponent<M8.EntityBase>();

        drag.dragEndCallback += OnDragEnd;
    }

    void OnDragEnd() {
        if(drag.isDelete)
            entity.Release();
    }
}
