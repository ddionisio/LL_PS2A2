using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMoveAnimator : MonoBehaviour {
    [Header("Data")]
    public UnitEntity unit;
    [M8.TagSelector]
    public string tagIgnoreSide; //tag to ignore side animation on contact
    public M8.EntityState[] states; //which states this is associated with
    public float idleDelayMin;
    public float idleDelayMax;

    [Header("Sprite")]
    public SpriteRenderer spriteRender;

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeAir; //while not on ground
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeGroundIdle;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeGroundIdleDelay; //do a simple idle animation after a delay while on idle
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeGroundMove;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeGroundPush;

    private bool mIsActive = false;

    private int mTakeAirInd;
    private int mTakeGroundIdleInd;
    private int mTakeGroundIdleDelayInd;
    private int mTakeGroundMoveInd;
    private int mTakeGroundPushInd;

    private Coroutine mIdleRout;

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
        mTakeGroundIdleDelayInd = animator.GetTakeIndex(takeGroundIdleDelay);
        mTakeGroundMoveInd = animator.GetTakeIndex(takeGroundMove);
        mTakeGroundPushInd = animator.GetTakeIndex(takeGroundPush);
    }

    void OnDisable() {
        if(mIdleRout != null) {
            StopCoroutine(mIdleRout);
            mIdleRout = null;
        }
    }

    void Update() {
        if(mIsActive) {
            var bodyMoveCtrl = unit.bodyMoveCtrl;
            float moveX = bodyMoveCtrl.moveHorizontal;

            if(bodyMoveCtrl.isGrounded) {
                if(moveX == 0f) {
                    if(mTakeGroundIdleInd != -1) {
                        if(animator.currentPlayingTakeIndex != mTakeGroundIdleInd) {
                            if(mTakeGroundIdleDelayInd != -1 && (idleDelayMin > 0f || idleDelayMax > 0f)) {
                                if(mIdleRout == null)
                                    mIdleRout = StartCoroutine(DoIdleDelay());
                            }
                            else
                                animator.Play(mTakeGroundIdleInd);
                        }
                    }
                }
                else {
                    if(moveX != 0f)
                        spriteRender.flipX = moveX < 0f;

                    bool isSide = false;

                    if(mTakeGroundPushInd != -1) {
                        var collDat = bodyMoveCtrl.collisionData;
                        for(int i = 0; i < collDat.Count; i++) {
                            if((collDat[i].flag & CollisionFlags.Sides) != CollisionFlags.None) {
                                if(Mathf.Sign(collDat[i].normal.x) == moveX)
                                    continue;

                                if(string.IsNullOrEmpty(tagIgnoreSide) || !collDat[i].collider.CompareTag(tagIgnoreSide))
                                    isSide = true;

                                break;
                            }
                        }
                    }

                    if(isSide) {
                        StopIdleDelay();
                        animator.Play(mTakeGroundPushInd);
                    }
                    else if(mTakeGroundMoveInd != -1) {
                        StopIdleDelay();
                        animator.Play(mTakeGroundMoveInd);
                    }
                }
            }
            else {
                if(moveX != 0f)
                    spriteRender.flipX = moveX < 0f;

                if(mTakeAirInd != -1) {
                    StopIdleDelay();
                    animator.Play(mTakeAirInd);
                }
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

        if(!mIsActive) {
            StopIdleDelay();
        }
    }

    void StopIdleDelay() {
        if(mIdleRout != null) {
            StopCoroutine(mIdleRout);
            mIdleRout = null;
        }
    }

    IEnumerator DoIdleDelay() {
        while(true) {
            animator.Play(mTakeGroundIdleInd);

            var delay = Random.Range(idleDelayMin, idleDelayMax);
            yield return new WaitForSeconds(delay);

            animator.Play(mTakeGroundIdleDelayInd);
            while(animator.isPlaying)
                yield return null;
        }
    }
}
