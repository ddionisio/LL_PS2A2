using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Drag snap to a surface
/// </summary>
public class DragCursorWorldSurfaceSnap : DragCursorWorld, IComparer<RaycastHit2D> {
    [Header("Data")]
    public float checkRadius;
    public LayerMask checkLayerMask;
    public Vector2 checkBoxSize;
    public LayerMask checkBoxOverlapLayerMask;
    public float checkSurfaceOfs = 0.01f;
    [M8.TagSelector]
    public string[] checkTagInvalids; //which tags are invalid placements

    [Header("Display")]
    public Transform ghostRoot;
    public SpriteRenderer ghostSpriteRenderer;
    public M8.RendererAutoTileScale ghostAutoTileScale;

    public Vector2 surfacePoint { get; private set; }
    public Vector2 surfaceNormal { get; private set; }

    public override Vector2 spawnPoint { get { return surfacePoint; } }
    public override Vector2 spawnUp { get { return surfaceNormal; } }

    private const int surfaceOverlapCapacity = 8;
    private RaycastHit2D[] mSurfaceOverlaps = new RaycastHit2D[surfaceOverlapCapacity];
    private int mSurfaceOverlapCount;

    private Vector2 mIconLocalPos;
    private float mIconOfs;

    private bool mIsBoxValid;

    public void ApplyGhostSprite(Sprite sprite) {
        if(ghostSpriteRenderer)
            ghostSpriteRenderer.sprite = sprite;
    }

    protected override void Awake() {
        base.Awake();
        
        if(iconSpriteRender) {
            mIconLocalPos = iconSpriteRender.transform.localPosition;
            mIconOfs = mIconLocalPos.magnitude;
        }
    }

    protected override void UpdatePosition(PointerEventData eventData) {
        base.UpdatePosition(eventData);

        //check surfaces
        mSurfaceOverlapCount = Physics2D.CircleCastNonAlloc(worldPoint, checkRadius, Vector2.zero, mSurfaceOverlaps, 0f, checkLayerMask);
        System.Array.Sort(mSurfaceOverlaps, 0, mSurfaceOverlapCount, this);

        mIsBoxValid = false;

        //go through surfaces and check validity
        float surfaceOfs = checkBoxSize.y * 0.5f + checkSurfaceOfs;

        for(int i = 0; i < mSurfaceOverlapCount; i++) {
            var hit = mSurfaceOverlaps[i];

            //check tag invalids
            bool isInvalid = false;
            for(int tagInd = 0; tagInd < checkTagInvalids.Length; tagInd++) {
                var tag = checkTagInvalids[tagInd];
                if(!string.IsNullOrEmpty(tag) && hit.collider.CompareTag(tag)) {
                    isInvalid = true;
                    break;
                }
            }

            if(isInvalid)
                continue;

            //grab point on surface
            var dCast = hit.point - worldPoint;
            var dCastDist = dCast.magnitude;
            if(dCastDist <= 0f)
                continue;

            var dCastDir = dCast / dCastDist;

            var dCastHit = Physics2D.Raycast(worldPoint, dCastDir, dCastDist, checkLayerMask);
            //

            surfacePoint = dCastHit.point;
            surfaceNormal = hit.normal;

            var checkPt = dCastHit.point + hit.normal * surfaceOfs;
                        
            var angle = Vector2.SignedAngle(Vector2.up, hit.normal);

            var overlapColl = Physics2D.OverlapBox(checkPt, checkBoxSize, angle, checkBoxOverlapLayerMask);
            if(!overlapColl) {
                mIsBoxValid = true;                
                break;
            }
            //else
                //Debug.Log("Overlap: " + overlapColl.name);
        }

        //update icon/ghost position/orientation
        if(mIsBoxValid) {
            if(ghostRoot) {
                ghostRoot.position = surfacePoint;
                ghostRoot.up = surfaceNormal;
            }

            if(iconSpriteRender) {
                var t = iconSpriteRender.transform;
                t.position = transform.position + (Vector3)(surfaceNormal * mIconOfs);
                t.up = surfaceNormal;
            }
        }
        else {
            if(iconSpriteRender) {
                var t = iconSpriteRender.transform;
                t.localPosition = mIconLocalPos;
                t.localRotation = Quaternion.identity;
            }
        }
    }

    protected override bool IsDropValid(PointerEventData eventData) {
        if(!base.IsDropValid(eventData))
            return false;

        return mIsBoxValid;
    }

    protected override void ApplyDropValid() {
        base.ApplyDropValid();

        if(ghostRoot) ghostRoot.gameObject.SetActive(isDropValid);
    }

    int IComparer<RaycastHit2D>.Compare(RaycastHit2D h1, RaycastHit2D h2) {
        if(h1.distance < h2.distance)
            return -1;
        else if(h1.distance > h2.distance)
            return 1;

        return 0;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        Gizmos.color = Color.cyan;
        if(Application.isPlaying) {
            Gizmos.DrawWireCube(surfacePoint, checkBoxSize);
        }
        else {
            Gizmos.DrawWireCube(transform.position, checkBoxSize);
        }
    }
}
