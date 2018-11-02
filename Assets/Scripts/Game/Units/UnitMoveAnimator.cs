using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveAnimator : MonoBehaviour {
    [Header("Data")]
    public UnitEntity unit;
    [M8.TagSelector]
    public string tagIgnoreSide; //tag to ignore side animation on contact
    public M8.EntityState[] states; //which states this is associated with

    [Header("Sprite")]
    public SpriteRenderer spriteRender;

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeAir; //while not on ground
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeGroundIdle;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeGroundMove;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeGroundPush;

    private bool mIsActive = false;

    private int mTakeAirInd;
    private int mTakeGroundIdleInd;
    private int mTakeGroundMoveInd;
    private int mTakeGroundPushInd;

    void OnDestroy() {
        if(unit)
            unit.setStateCallback -= OnUnitChangeState;
    }

    void Awake() {
        if(!unit)
            unit = GetComponent<UnitEntity>();

        unit.setStateCallback += OnUnitChangeState;

        mTakeAirInd = animator.GetTakeIndex(takeAir);
        mTakeGroundIdleInd = animator.GetTakeIndex(takeGroundIdle);
        mTakeGroundMoveInd = animator.GetTakeIndex(takeGroundMove);
        mTakeGroundPushInd = animator.GetTakeIndex(takeGroundPush);
    }

    void Update() {
        if(mIsActive) {
            var bodyMoveCtrl = unit.bodyMoveCtrl;
            float moveXSign = Mathf.Sign(bodyMoveCtrl.moveHorizontal);

            if(bodyMoveCtrl.isGrounded) {
                if(moveXSign == 0f) {
                    animator.Play(mTakeGroundIdleInd);
                }
                else {
                    spriteRender.flipX = moveXSign < 0f;

                    bool isSide = false;

                    var collDat = bodyMoveCtrl.collisionData;
                    for(int i = 0; i < collDat.Count; i++) {
                        if((collDat[i].flag & CollisionFlags.Sides) != CollisionFlags.None) {
                            if(Mathf.Sign(collDat[i].normal.x) == moveXSign)
                                continue;

                            if(string.IsNullOrEmpty(tagIgnoreSide) || !collDat[i].collider.CompareTag(tagIgnoreSide))
                                isSide = true;

                            break;
                        }
                    }

                    if(isSide) {
                        animator.Play(mTakeGroundPushInd);
                    }
                    else {
                        animator.Play(mTakeGroundMoveInd);
                    }
                }
            }
            else {
                spriteRender.flipX = moveXSign < 0f;

                animator.Play(mTakeAirInd);
            }
        }
    }
    
    void OnUnitChangeState(M8.EntityBase ent) {
        mIsActive = false;

        for(int i = 0; i < states.Length; i++) {
            if(ent.state == states[i]) {
                mIsActive = true;
                break;
            }
        }
    }
}
