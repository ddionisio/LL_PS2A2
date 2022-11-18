using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSurfaceSnap : MonoBehaviour, M8.IPoolSpawn, M8.IPoolDespawn, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [Header("Info")]
    public UnitUIDisplayInfo displayInfo;
    public bool ghostAutoTileFlipY;

    [Header("Drag Info")]

    [SerializeField]
    bool _dragEnabled = true;

    public GameObject dragActiveGO; //activated during drag
    public GameObject dragInactiveGO; //deactivate during drag

    public bool isDragEnabled {
        get { return _dragEnabled; }
        set {
            if(_dragEnabled != value) {
                _dragEnabled = value;
                ApplyDragEnabled();
            }
        }
    }

    public Vector2 position {
        get {
            return (Vector2)transform.position;
        }

        set {
            transform.position = value;
        }
    }

    public DragCursorWorldSurfaceSnap dragWorldSurfaceSnap { get; private set; }

    public bool isDelete { get; private set; }

    public event System.Action dragBeginCallback;
    public event System.Action dragEndCallback;

    private bool mIsDragging;
        
    void OnApplicationFocus(bool focus) {
        if(!focus)
            SetDragging(false);
    }

    void OnDisable() {
        SetDragging(false);
    }

    void Awake() {
        mIsDragging = false;
        ApplyDragState();

        ApplyDragEnabled();
    }

    void M8.IPoolSpawn.OnSpawned(M8.GenericParams parms) {
        //grab drag cursor
        if(parms != null) {
            var dragCursorObj = parms.GetValue<object>(EntitySpawnerWidget.parmDragWorld);
            dragWorldSurfaceSnap = dragCursorObj as DragCursorWorldSurfaceSnap;
        }
    }

    void M8.IPoolDespawn.OnDespawned() {
        SetDragging(false);

        if(dragWorldSurfaceSnap) {
            dragWorldSurfaceSnap = null;
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        SetDragging(true);

        dragWorldSurfaceSnap.UpdateState(eventData);
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        dragWorldSurfaceSnap.UpdateState(eventData);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        dragWorldSurfaceSnap.UpdateState(eventData);

        if(dragWorldSurfaceSnap.isDropValid) {
            transform.position = dragWorldSurfaceSnap.surfacePoint;
            transform.rotation = Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, dragWorldSurfaceSnap.surfaceNormal), Vector3.forward);
            //transform.up = dragWorldSurfaceSnap.surfaceNormal;
        }

        isDelete = dragWorldSurfaceSnap.isDelete;

        SetDragging(false);
    }

    private void SetDragging(bool dragging) {
        if(mIsDragging != dragging) {
            mIsDragging = dragging;

            ApplyDragState();

            if(mIsDragging) {
                if(dragBeginCallback != null)
                    dragBeginCallback();
            }
            else {
                if(dragEndCallback != null)
                    dragEndCallback();
            }
        }
    }

    private void ApplyDragEnabled() {
        //TODO
    }

    private void ApplyDragState() {
        if(mIsDragging) {
            if(dragWorldSurfaceSnap) {
                dragWorldSurfaceSnap.gameObject.SetActive(true);
                dragWorldSurfaceSnap.ApplyIcon(displayInfo.uiWorldIcon);
                dragWorldSurfaceSnap.deleteEnabled = true;

                dragWorldSurfaceSnap.ghostAutoTileScale.flipY = ghostAutoTileFlipY;
            }

            if(dragActiveGO) dragActiveGO.SetActive(true);
            if(dragInactiveGO) dragInactiveGO.SetActive(false);
        }
        else {
            if(dragWorldSurfaceSnap) {
                dragWorldSurfaceSnap.gameObject.SetActive(false);
            }

            if(dragActiveGO) dragActiveGO.SetActive(false);
            if(dragInactiveGO) dragInactiveGO.SetActive(true);
        }   
    }

    void OnDrawGizmos() {
        //Gizmos.color = Color.yellow;

        //Gizmos.DrawWireCube(transform.position + (Vector3)checkArea.position, new Vector3(checkArea.width, checkArea.height, 0f));
    }
}
