using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Used by DragWidget to visualize its position in the world
/// </summary>
public class DragCursor : MonoBehaviour {
    [Header("Display")]
    public SpriteRenderer iconSpriteRender;

    [Header("Drop")]
    public GameObject dropValidGO; //can drop
    public GameObject dropInvalidGO; //can't drop
    public LayerMask dropFilterLayerMask;
    public bool dropFilterEnabled; //if true, check drop filter layer mask from eventData for validity

    public bool isDropValid { get { return mIsDropValid; } }

    public Vector2 worldPoint { get; protected set; }

    private bool mIsDropValid = false;

    public void ApplyIcon(Sprite sprite) {
        if(iconSpriteRender)
            iconSpriteRender.sprite = sprite;
    }

    public void UpdateState(PointerEventData eventData) {
        if(eventData != null)
            UpdatePosition(eventData);

        bool _isDropValid = IsDropValid(eventData);
        if(mIsDropValid != _isDropValid) {
            mIsDropValid = _isDropValid;
            ApplyDropValid();
        }
    }

    protected virtual void Awake() {
        mIsDropValid = false;
        ApplyDropValid();
    }

    protected virtual void UpdatePosition(PointerEventData eventData) {
        //convert to world space
        var gameCam = M8.Camera2D.main;
                
        worldPoint = gameCam.unityCamera.ScreenToWorldPoint(eventData.position);

        transform.position = worldPoint;
    }

    protected virtual bool IsDropValid(PointerEventData eventData) {
        if(eventData == null)
            return false;

        if(dropFilterEnabled) {
            if(!eventData.pointerCurrentRaycast.isValid)
                return false;

            return (dropFilterLayerMask & (1 << eventData.pointerCurrentRaycast.gameObject.layer)) != 0;
        }

        return true;
    }

    private void ApplyDropValid() {
        if(dropValidGO) dropValidGO.SetActive(mIsDropValid);
        if(dropInvalidGO) dropInvalidGO.SetActive(!mIsDropValid);
    }
}
