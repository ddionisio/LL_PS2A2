using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragRigidbody2D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDragCursorWorldDropValid {
    [Header("Data")]    
    public Rigidbody2D body;
    public Rect checkArea; //relative to drag position, check with overlap to ensure nothing is touching
    public LayerMask checkMask; //layer mask to determine which overlap should not touch to drop

    [Header("Drag Info")]
    [SerializeField]
    DragCursorWorld _dragCursor;
    [SerializeField]
    bool _dragEnabled = true;
    public bool dragLockX; //lock X to body's x position
    public bool dragLockY; //lock Y to body's y position

    public Sprite dragSpriteIcon;
    public M8.SpriteColorGroup dragColorGroup;
    public GameObject dragActiveGO; //activated during drag
    public Color dragActiveColor = Color.white; //color to apply to dragColorGroup while dragging
    public GameObject dragInactiveGO; //deactivate during drag
    public Transform dragDisplayRoot; //this is the one that gets moved as drag happens

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
            return body && body.simulated ? body.position : (Vector2)transform.position;
        }

        set {
            if(body && body.simulated)
                body.position = value;
            else
                transform.position = value;
        }
    }

    public event System.Action dragBeginCallback;
    public event System.Action dragEndCallback;

    private bool mIsDragging;

    private const int overlapResultCapacity = 8;
    private Collider2D[] mOverlapResults = new Collider2D[overlapResultCapacity];

    public void SetDragCursor(DragCursorWorld cursor) {
        if(_dragCursor != cursor) {
            if(mIsDragging && _dragCursor)
                _dragCursor.UnregisterDropValidCheck(this);

            _dragCursor = cursor;

            if(mIsDragging && _dragCursor)
                _dragCursor.RegisterDropValidCheck(this);
        }
    }

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

    void Update() {
        if(mIsDragging) {
            if(dragDisplayRoot) {
                dragDisplayRoot.position = _dragCursor.worldPoint;
                dragDisplayRoot.rotation = Quaternion.identity;
            }
        }
    }
    
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        if(!_dragCursor)
            return;

        UpdatePointerEventData(eventData);

        SetDragging(true);

        _dragCursor.UpdateState(eventData);
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        UpdatePointerEventData(eventData);

        _dragCursor.UpdateState(eventData);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        UpdatePointerEventData(eventData);

        _dragCursor.UpdateState(eventData);

        if(_dragCursor.isDropValid) {
            if(body) {
                if(body.simulated) {
                    body.position = _dragCursor.worldPoint;
                    body.rotation = 0f;
                }
                else {
                    body.transform.position = _dragCursor.worldPoint;
                    body.transform.rotation = Quaternion.identity;
                }

                body.velocity = Vector2.zero;
                body.angularVelocity = 0f;
            }
            else {
                transform.position = _dragCursor.worldPoint;
                transform.rotation = Quaternion.identity;
            }
        }

        SetDragging(false);
    }

    bool IDragCursorWorldDropValid.Check(PointerEventData eventData) {
        int overlapCount = Physics2D.OverlapBoxNonAlloc(_dragCursor.worldPoint + checkArea.position, checkArea.size, 0f, mOverlapResults, checkMask);
        if(overlapCount == 0)
            return true;

        if(overlapCount == 1) {
            //check that it's our own
            return body.gameObject == mOverlapResults[0].gameObject;
        }

        return false;
    }

    private void UpdatePointerEventData(PointerEventData eventData) {
        if(dragLockX || dragLockY) {
            //modify position to stay on body's x or y
            //TODO: for now use main camera
            var cam = Camera.main;
            var screenPos = cam.WorldToScreenPoint(position);

            var pointerPos = eventData.position;

            if(dragLockX)
                pointerPos.x = screenPos.x;
            if(dragLockY)
                pointerPos.y = screenPos.y;

            eventData.position = pointerPos;
        }
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
            //dragCursor
            if(_dragCursor) {
                _dragCursor.gameObject.SetActive(true);
                _dragCursor.ApplyIcon(dragSpriteIcon);
                _dragCursor.RegisterDropValidCheck(this);
            }

            if(dragActiveGO) dragActiveGO.SetActive(true);
            if(dragColorGroup) dragColorGroup.ApplyColor(dragActiveColor);
            if(dragInactiveGO) dragInactiveGO.SetActive(false);
            if(dragDisplayRoot) dragDisplayRoot.gameObject.SetActive(true);
        }
        else {
            if(_dragCursor) {
                _dragCursor.gameObject.SetActive(false);
                _dragCursor.UnregisterDropValidCheck(this);
            }

            if(dragActiveGO) dragActiveGO.SetActive(false);
            if(dragColorGroup) dragColorGroup.Revert();
            if(dragInactiveGO) dragInactiveGO.SetActive(true);
            if(dragDisplayRoot) dragDisplayRoot.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(transform.position + (Vector3)checkArea.position, new Vector3(checkArea.width, checkArea.height, 0f));
    }
}
