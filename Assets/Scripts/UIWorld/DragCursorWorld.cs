using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IDragCursorWorldDropValid {
    bool Check(PointerEventData eventData);
}

/// <summary>
/// Used by DragWidget to visualize its position in the world
/// </summary>
public class DragCursorWorld : MonoBehaviour {
    [Header("Display")]
    public SpriteRenderer iconSpriteRender;
    public M8.SpriteColorGroup iconSpriteColorGroup;
    public Color iconSpriteColorInvalid = Color.red;

    [Header("Drop")]
    public GameObject dropValidGO; //can drop
    public GameObject dropInvalidGO; //can't drop
    public LayerMask dropFilterLayerMask;
    public bool dropFilterEnabled; //if true, check drop filter layer mask from eventData for validity

    public bool isDropValid { get { return mIsDropValid; } }

    public Vector2 worldPoint { get; protected set; }

    public virtual Vector2 spawnPoint { get { return worldPoint; } }
    public virtual Vector2 spawnUp { get { return Vector2.up; } }

    private bool mIsDropValid = false;
    private List<IDragCursorWorldDropValid> mDropValidChecks = new List<IDragCursorWorldDropValid>();

    public void RegisterDropValidCheck(IDragCursorWorldDropValid dropValid) {
        if(!mDropValidChecks.Contains(dropValid))
            mDropValidChecks.Add(dropValid);
    }

    public void UnregisterDropValidCheck(IDragCursorWorldDropValid dropValid) {
        mDropValidChecks.Remove(dropValid);
    }

    public void ApplyIcon(Sprite sprite) {
        if(iconSpriteRender) {
            iconSpriteRender.sprite = sprite;
        }
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

            if((dropFilterLayerMask & (1 << eventData.pointerCurrentRaycast.gameObject.layer)) == 0)
                return false;
        }

        //other checks
        for(int i = 0; i < mDropValidChecks.Count; i++) {
            if(!mDropValidChecks[i].Check(eventData))
                return false;
        }

        return true;
    }

    protected virtual void ApplyDropValid() {
        if(dropValidGO) dropValidGO.SetActive(mIsDropValid);
        if(dropInvalidGO) dropInvalidGO.SetActive(!mIsDropValid);

        if(iconSpriteColorGroup) {
            if(mIsDropValid)
                iconSpriteColorGroup.Revert();
            else
                iconSpriteColorGroup.ApplyColor(iconSpriteColorInvalid);
        }
    }
}
