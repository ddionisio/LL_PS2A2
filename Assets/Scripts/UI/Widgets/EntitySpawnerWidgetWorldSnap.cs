using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawnerWidgetWorldSnap : EntitySpawnerWidget {

    private DragSurfaceSnap mEntityTemplateDragSurfaceSnap;

    public override void Init() {
        base.Init();

        if(!mEntityTemplateDragSurfaceSnap)
            mEntityTemplateDragSurfaceSnap = entityTemplate.GetComponent<DragSurfaceSnap>();
    }

    protected override void SetupCursorWorld() {
        base.SetupCursorWorld();

        if(cursorWorld is DragCursorWorldSurfaceSnap) {
            var cursorWorldSurfaceSnap = (DragCursorWorldSurfaceSnap)cursorWorld;

            if(mEntityTemplateDragSurfaceSnap) {
                cursorWorldSurfaceSnap.SetBeamType(mEntityTemplateDragSurfaceSnap.beamType);
            }
        }
    }
}
