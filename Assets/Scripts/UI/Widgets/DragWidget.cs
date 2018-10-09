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
    public DragCursorWorld cursorWorld;

    [Header("Signals")]
    public SignalDragWidget signalDragBegin;
    public SignalDragWidget signalDragEnd;
    
    public virtual bool active {
        get { return mActive; }
        set {
            if(mActive != value) {
                mActive = value;
                ApplyCurDragEnabled();
            }
        }
    }

    public bool isDropValid {
        get {
            switch(curSpace) {
                case DragWidgetSpace.UI:
                    return cursorUI ? cursorUI.isDropValid : false;
                case DragWidgetSpace.World:
                    return cursorWorld ? cursorWorld.isDropValid : false;
                default:
                    return false;
            }
        }
    }

    public DragWidgetSpace curSpace { get; private set; }

    public bool isDragging { get; private set; }

    private bool mActive;

    public virtual void Init() {
        if(icon) {
            icon.sprite = iconSpriteUI;

            if(iconApplyNativeSize)
                icon.SetNativeSize();
        }
    }

    protected virtual void DragEnd() {

    }

    protected virtual void OnEnable() {
        mActive = defaultDragEnabled;
        ApplyCurDragEnabled();

        if(initOnEnable)
            Init();
    }

    protected virtual void OnDisable() {
        StopDrag();
    }

    protected virtual void Awake() {
        ResetState();
    }

    protected void ApplyCurDragEnabled() {
        if(active) {
            if(disableColorGroup) disableColorGroup.Revert();
        }
        else {
            StopDrag();

            if(disableColorGroup) disableColorGroup.ApplyColor(disableColor);
        }
    }

    protected void StopDrag() {
        if(isDragging) {
            curSpace = DragWidgetSpace.None;

            if(signalDragEnd)
                signalDragEnd.Invoke(new DragWidgetSignalInfo() { dragWidget = this, dragWidgetSpace = curSpace, pointerData = null });

            ResetState();
        }
    }

    protected void ResetState() {
        if(cursorUI) cursorUI.gameObject.SetActive(false);
        if(cursorWorld) cursorWorld.gameObject.SetActive(false);

        curSpace = DragWidgetSpace.None;
        isDragging = false;
    }
        
    void OnApplicationFocus(bool isFocus) {
        if(!isFocus) {
            StopDrag();
        }
    }
        
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        if(!active)
            return;

        isDragging = true;

        //setup cursors
        SetupCursorUI();
        SetupCursorWorld();

        //apply space
        curSpace = DragWidgetSpace.None;
        UpdateState(eventData);

        if(signalDragBegin)
            signalDragBegin.Invoke(new DragWidgetSignalInfo() { dragWidget = this, dragWidgetSpace = curSpace, pointerData = eventData });
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if(!isDragging)
            return;

        UpdateState(eventData);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if(!isDragging)
            return;

        UpdateState(eventData);

        DragEnd();

        if(signalDragEnd)
            signalDragEnd.Invoke(new DragWidgetSignalInfo() { dragWidget = this, dragWidgetSpace = curSpace, pointerData = eventData });

        ResetState();
    }
        
    private void UpdateState(PointerEventData eventData) {
        var uiAreaLocalPos = uiArea.InverseTransformPoint(eventData.position);

        var space = uiArea.rect.Contains(uiAreaLocalPos) ? DragWidgetSpace.UI : DragWidgetSpace.World;

        if(curSpace != space) {
            curSpace = space;
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
        if(cursorUI) cursorUI.gameObject.SetActive(curSpace == DragWidgetSpace.UI);
        if(cursorWorld) cursorWorld.gameObject.SetActive(curSpace == DragWidgetSpace.World);
    }
        
    private void ApplyPosition(PointerEventData eventData) {
        switch(curSpace) {
            case DragWidgetSpace.UI:
                if(cursorUI) cursorUI.UpdateState(eventData);
                break;

            case DragWidgetSpace.World:
                if(cursorWorld) cursorWorld.UpdateState(eventData);
                break;
        }
    }
}
