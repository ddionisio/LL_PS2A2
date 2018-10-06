using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCursorGround : DragCursor {

    [Header("Ground Data")]
    public LayerMask groundLayerMask;

    [Header("Ground Display")]
    public SpriteRenderer groundTargetSprite;
    public SpriteRenderer groundToPointerSprite; //line display between ground and pointer, ensure anchor is bottom
    public int groundToPointerBottomOfsPixelY; //offset relative to ground target (pixel)

    public RaycastHit2D groundRaycastHit { get; private set; }

    private Vector2 mIconOfsDefault;
    private float mGroundToPointerUnitHeightRatio;

    protected override void Awake() {
        base.Awake();

        mIconOfsDefault = iconSpriteRender.transform.localPosition;

        mGroundToPointerUnitHeightRatio = groundToPointerSprite.sprite.pixelsPerUnit / groundToPointerSprite.sprite.rect.height;
    }

    protected override void UpdatePosition(PointerEventData eventData) {
        base.UpdatePosition(eventData);

        //get ground position
        var gameCamera = GameCamera.main;

        //get ground hit
        Vector2 checkPoint;
        float checkLength;

        //try to set check point to the top of the camera
        if(gameCamera) {
            checkPoint = new Vector2(worldPoint.x, gameCamera.position.y + gameCamera.cameraViewExtents.y);
            checkLength = gameCamera.cameraViewRect.height;
        }
        else {
            checkPoint = worldPoint;
            checkLength = float.MaxValue;
        }

        var checkDir = Vector2.down;

        groundRaycastHit = Physics2D.Raycast(checkPoint, checkDir, checkLength, groundLayerMask);

        //get ground point
        Vector2 groundPos;

        if(groundRaycastHit.collider)
            groundPos = groundRaycastHit.point;
        else
            groundPos = worldPoint;

        //determine icon position (if above ground, set relative to ground; pointer otherwise)
        var iconTrans = iconSpriteRender.transform;

        if(groundPos.y >= worldPoint.y)
            iconTrans.position = groundPos + mIconOfsDefault;
        else
            iconTrans.localPosition = mIconOfsDefault;

        //set ground pointer display
        groundTargetSprite.transform.position = groundPos;

        //set line display
        float groundToPointerScaleY = worldPoint.y - groundPos.y;
        if(groundToPointerScaleY > 0f) {
            groundToPointerSprite.gameObject.SetActive(true);

            var t = groundToPointerSprite.transform;

            float groundTargetHeightUnit = groundTargetSprite.sprite.rect.height / groundTargetSprite.sprite.pixelsPerUnit;
            float ofsY = groundTargetHeightUnit - (groundTargetSprite.sprite.pivot.y / groundTargetSprite.sprite.pixelsPerUnit) + (groundToPointerBottomOfsPixelY / groundTargetSprite.sprite.pixelsPerUnit);

            ofsY *= groundTargetSprite.transform.localScale.y;

            var groundToPointerStart = groundTargetSprite.transform.position;
            groundToPointerStart.y += ofsY;

            t.position = groundToPointerStart;

            if(groundToPointerSprite.drawMode == SpriteDrawMode.Simple) {
                var s = t.localScale;
                s.y = (groundToPointerScaleY - ofsY) * mGroundToPointerUnitHeightRatio;

                t.localScale = s;
            }
            else {
                var s = groundToPointerSprite.size;
                s.y = groundToPointerScaleY - ofsY;

                groundToPointerSprite.size = s;
            }
        }
        else {
            groundToPointerSprite.gameObject.SetActive(false);
        }
    }

    protected override bool IsDropValid(PointerEventData eventData) {
        if(!base.IsDropValid(eventData))
            return false;

        //check if ground is available
        return groundRaycastHit.collider != null;
    }
}
