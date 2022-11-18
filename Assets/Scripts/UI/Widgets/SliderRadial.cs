using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderRadial : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public enum Dir {
        Left,
        Right,
        Up,
        Down
    }

    [Header("Config")]
    public Dir dir;
    public float startAngle = 0f;
    public float endAngle = 360f;
    public float radius = 1f;    
    public bool isScreenSpace;

    [Header("Display")]
    public Transform handle;
    public M8.SpriteColorGroup spriteColorGroup; //for displays in world
    public M8.UI.Graphics.ColorGroup uiColorGroup; //for displays in UI
    public Color disableColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("Value")]
    public float minValue = 0f;
    public float maxValue = 100f;
    public bool isValueRounded;
    [SerializeField]
    float _value = 0f;
    [SerializeField]
    Slider.SliderEvent _onValueChanged;
    
    public float value {
        get { return _value; }
        set {
            var v = Mathf.Clamp(isValueRounded ? Mathf.Round(value) : value, minValue, maxValue);
            if(_value != v) {
                _value = v;
                UpdateCurDirFromValue();
                UpdateHandle();

                _onValueChanged.Invoke(_value);
            }
        }
    }

    public float valueScalar {
        get {
            var delta = maxValue - minValue;
            return delta > 0f ? Mathf.Clamp01((value - minValue) / delta) : 0f;
        }

        set {
            this.value = Mathf.Lerp(minValue, maxValue, Mathf.Clamp01(value));
        }
    }

    public bool interactable {
        get { return mInteractable; }
        set {
            if(mInteractable != value) {
                mInteractable = value;
                ApplyInteractable();
            }
        }
    }

    public Slider.SliderEvent onValueChanged { get { return _onValueChanged; } }

    private Vector2 mStartDir;
    private Vector2 mEndDir;
    private Vector2 mCurDir;

    private bool mInteractable = true;
    private bool mIsDragging;

    private Collider2D[] mColls;

    void OnApplicationFocus(bool focus) {
        if(!focus) {
            if(mIsDragging) {
                mIsDragging = false;
            }
        }
    }

    void Awake() {
        SetupDirs();
        UpdateCurDirFromValue();
        UpdateHandle();
    }
        
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        mIsDragging = true;
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        Vector2 pos = GetWorldPos(eventData);
        UpdateCurDirAndValueFromPosition(pos);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        mIsDragging = false;

        Vector2 pos = GetWorldPos(eventData);
        UpdateCurDirAndValueFromPosition(pos);
    }

    private Vector2 GetInitDir() {
        switch(dir) {
            case Dir.Left:
                return Vector2.left;
            case Dir.Right:
                return Vector2.right;
            case Dir.Down:
                return Vector2.down;
            default:
                return Vector2.up;
        }
    }
        
    private void SetupDirs() {
        Vector2 d = GetInitDir();

        mStartDir = M8.MathUtil.RotateAngle(d, startAngle);
        mEndDir = M8.MathUtil.RotateAngle(d, endAngle);
    }

    private Vector2 GetWorldPos(PointerEventData eventData) {
        Vector2 pos;
        if(isScreenSpace)
            pos = eventData.position;
        else {
            //TODO: use main camera for now
            var cam = Camera.main;
            pos = cam.ScreenToWorldPoint(eventData.position);
        }

        return pos;
    }

    private void UpdateCurDirAndValueFromPosition(Vector2 pos) {
        var delta = pos - (Vector2)transform.position;
        var norm = delta.normalized;

        var startAngleAbs = AngleAbs(startAngle);
        var endAngleAbs = AngleAbs(endAngle);

        var initDir = GetInitDir();

        var curAngle = Vector2.SignedAngle(norm, initDir);
        curAngle = AngleAbs(curAngle);
                
        //set value via scalar
        float deltaAngle = Mathf.Abs(endAngleAbs - startAngleAbs);
        if(deltaAngle > 0f) {
            float scalar;
            if(startAngleAbs < endAngleAbs)
                scalar = (curAngle - startAngleAbs) / deltaAngle;
            else
                scalar = (curAngle - endAngleAbs) / deltaAngle;

            valueScalar = scalar;
        }   
    }

    private float AngleAbs(float a) {
        float _a = a % 360f;
        if(_a < 0f)
            _a = 360f - _a;
        return _a;
    }

    private void UpdateCurDirFromValue() {
        mCurDir = Vector2.Lerp(mStartDir, mEndDir, valueScalar);
        mCurDir.Normalize();
    }

    private void UpdateHandle() {
        if(handle) {
            var pos = transform.TransformPoint(mCurDir * radius);
            handle.position = pos;
        }
    }

    private void ApplyInteractable() {
        if(mColls == null)
            mColls = GetComponentsInChildren<Collider2D>();

        if(mInteractable) {
            if(spriteColorGroup) spriteColorGroup.Revert();
            if(uiColorGroup) uiColorGroup.Revert();

            for(int i = 0; i < mColls.Length; i++)
                mColls[i].enabled = true;
        }
        else {
            if(spriteColorGroup) spriteColorGroup.ApplyColor(disableColor);
            if(uiColorGroup) uiColorGroup.ApplyColor(disableColor);

            for(int i = 0; i < mColls.Length; i++)
                mColls[i].enabled = false;
        }
    }

    void OnDrawGizmos() {
        if(radius > 0f) {
            SetupDirs();
            UpdateCurDirFromValue();
            UpdateHandle();

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.TransformPoint(mStartDir * radius));

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.TransformPoint(mEndDir * radius));
        }
    }
}
