using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSurfaceSnap : MonoBehaviour, M8.IPoolSpawn, M8.IPoolDespawn, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [Header("Drag Info")]

    [SerializeField]
    bool _dragEnabled = true;

    public Sprite dragSpriteIcon;
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

    public event System.Action dragBeginCallback;
    public event System.Action dragEndCallback;

    private bool mIsDragging;
    
    private DragCursorWorldSurfaceSnap mDragWorldSurfaceSnap;

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
            mDragWorldSurfaceSnap = dragCursorObj as DragCursorWorldSurfaceSnap;
        }
    }

    void M8.IPoolDespawn.OnDespawned() {
        SetDragging(false);

        if(mDragWorldSurfaceSnap) {
            mDragWorldSurfaceSnap = null;
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        UpdatePointerEventData(eventData);

        SetDragging(true);

        mDragWorldSurfaceSnap.UpdateState(eventData);
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        UpdatePointerEventData(eventData);

        mDragWorldSurfaceSnap.UpdateState(eventData);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        UpdatePointerEventData(eventData);

        mDragWorldSurfaceSnap.UpdateState(eventData);

        if(mDragWorldSurfaceSnap.isDropValid) {
            transform.position = mDragWorldSurfaceSnap.surfacePoint;
            transform.up = mDragWorldSurfaceSnap.surfaceNormal;
        }

        SetDragging(false);
    }

    private void UpdatePointerEventData(PointerEventData eventData) {
        
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
            if(mDragWorldSurfaceSnap) {
                mDragWorldSurfaceSnap.gameObject.SetActive(true);
                mDragWorldSurfaceSnap.ApplyIcon(dragSpriteIcon);
            }

            if(dragActiveGO) dragActiveGO.SetActive(true);
            if(dragInactiveGO) dragInactiveGO.SetActive(false);
        }
        else {
            if(mDragWorldSurfaceSnap) {
                mDragWorldSurfaceSnap.gameObject.SetActive(false);
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
