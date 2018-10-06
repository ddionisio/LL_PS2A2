using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragWidget : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [Header("Info")]
    public Sprite iconSpriteUI; //icon displayed during drag in UI
    public Sprite iconSpriteWorld; //icon displayed during drag in world

    [Header("Display")]
    public Image icon;
    public bool iconApplyNativeSize = true;

    public M8.UI.Graphics.ColorGroup disableColorGroup;
    public Color disableColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("Data")]
    public RectTransform uiArea;
    public bool defaultDragEnabled = true;
    public bool initOnEnable = true;

    [Header("Drag Cursors")]
    public DragCursorWidget cursorUI;
    public DragCursor cursorWorld;

    [Header("Signals")]
    public SignalDragWidget signalDragBegin;
    public SignalDragWidget signalDragEnd;
    
    public bool isDragEnabled {
        get { return mIsDragEnabled; }
        set {
            if(mIsDragEnabled != value) {
                mIsDragEnabled = value;
                ApplyCurDragEnabled();
            }
        }
    }

    public bool isDropValid {
        get {
            switch(mCurSpace) {
                case DragWidgetSpace.UI:
                    return cursorUI ? cursorUI.isDropValid : false;
                case DragWidgetSpace.World:
                    return cursorWorld ? cursorWorld.isDropValid : false;
                default:
                    return false;
            }
        }
    }

    private DragWidgetSpace mCurSpace;
    private bool mIsDragging;
    private bool mIsDragEnabled;

    public void Init() {
        if(icon) {
            icon.sprite = iconSpriteUI;

            if(iconApplyNativeSize)
                icon.SetNativeSize();
        }
    }

    void OnEnable() {
        mIsDragEnabled = defaultDragEnabled;
        ApplyCurDragEnabled();

        if(initOnEnable)
            Init();
    }

    void OnDisable() {
        StopDrag();
    }

    void OnApplicationFocus(bool isFocus) {
        if(!isFocus) {
            StopDrag();
        }
    }

    void Awake() {
        ResetState();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        if(!mIsDragEnabled)
            return;

        mIsDragging = true;

        //setup cursors
        SetupCursorUI();
        SetupCursorWorld();

        //apply space
        mCurSpace = DragWidgetSpace.None;
        UpdateState(eventData);

        if(signalDragBegin)
            signalDragBegin.Invoke(new DragWidgetSignalInfo() { dragWidget = this, dragWidgetSpace = mCurSpace, pointerData = eventData });
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        UpdateState(eventData);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        UpdateState(eventData);

        if(signalDragEnd)
            signalDragEnd.Invoke(new DragWidgetSignalInfo() { dragWidget = this, dragWidgetSpace = mCurSpace, pointerData = eventData });

        ResetState();
    }

    private void UpdateState(PointerEventData eventData) {
        var uiAreaLocalPos = uiArea.InverseTransformPoint(eventData.position);

        var space = uiArea.rect.Contains(uiAreaLocalPos) ? DragWidgetSpace.UI : DragWidgetSpace.World;

        if(mCurSpace != space) {
            mCurSpace = space;
            ApplyCurSpace();
        }

        ApplyPosition(eventData);
    }

    private void SetupCursorUI() {
        if(cursorUI) cursorUI.ApplyIcon(iconSpriteUI);
    }

    private void SetupCursorWorld() {
        if(cursorWorld) cursorWorld.ApplyIcon(iconSpriteWorld);
    }

    private void ApplyCurSpace() {
        if(cursorUI) cursorUI.gameObject.SetActive(mCurSpace == DragWidgetSpace.UI);
        if(cursorWorld) cursorWorld.gameObject.SetActive(mCurSpace == DragWidgetSpace.World);
    }

    private void ApplyCurDragEnabled() {
        if(mIsDragEnabled) {
            if(disableColorGroup) disableColorGroup.Revert();
        }
        else {
            StopDrag();

            if(disableColorGroup) disableColorGroup.ApplyColor(disableColor);            
        }   
    }

    private void ApplyPosition(PointerEventData eventData) {
        switch(mCurSpace) {
            case DragWidgetSpace.UI:
                if(cursorUI) cursorUI.UpdateState(eventData);
                break;

            case DragWidgetSpace.World:
                if(cursorWorld) cursorWorld.UpdateState(eventData);
                break;
        }
    }
        
    private void StopDrag() {
        if(mIsDragging) {
            mCurSpace = DragWidgetSpace.None;

            if(signalDragEnd)
                signalDragEnd.Invoke(new DragWidgetSignalInfo() { dragWidget = this, dragWidgetSpace = mCurSpace, pointerData = null });

            ResetState();
        }
    }

    private void ResetState() {
        if(cursorUI) cursorUI.gameObject.SetActive(false);
        if(cursorWorld) cursorWorld.gameObject.SetActive(false);

        mCurSpace = DragWidgetSpace.None;
        mIsDragging = false;
    }
}
