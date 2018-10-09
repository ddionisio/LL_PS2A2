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

    public Sprite dragSpriteIcon;    
    public GameObject dragActiveGO; //activated during drag
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

        SetDragging(true);

        _dragCursor.UpdateState(eventData);
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        _dragCursor.UpdateState(eventData);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

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

    private void SetDragging(bool dragging) {
        if(mIsDragging != dragging) {
            mIsDragging = dragging;
            ApplyDragState();
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
            if(dragDisplayRoot) dragDisplayRoot.gameObject.SetActive(true);
        }
        else {
            if(_dragCursor) {
                _dragCursor.gameObject.SetActive(false);
                _dragCursor.UnregisterDropValidCheck(this);
            }

            if(dragActiveGO) dragActiveGO.SetActive(false);
            if(dragDisplayRoot) dragDisplayRoot.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(transform.position + (Vector3)checkArea.position, new Vector3(checkArea.width, checkArea.height, 0f));
    }
}
