using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTrajectoryDisplayControl : MonoBehaviour {
    public PhysicsTrajectoryDisplay display;
    public Rigidbody2D bodyTarget;
    public Vector2 dir = Vector2.right;
    public bool isClockwise;
    public float duration;
    public float radiusPadding = 0.05f;

    public bool show {
        get { return mShow; }
        set {
            if(mShow != value) {
                mShow = value;
                if(mShow)
                    ApplyCurrent();
                else
                    display.Clear();
            }
        }
    }

    public float angle {
        get { return mAngle; }
        set {
            if(mAngle != value) {
                mAngle = value;

                if(isClockwise)
                    mDir = M8.MathUtil.RotateAngle(dir, angle);
                else
                    mDir = M8.MathUtil.RotateAngle(dir, -angle);

                //update display if shown
                ApplyCurrent();
            }
        }
    }

    public float force {
        get { return mForce; }
        set {
            if(mForce != value) {
                mForce = value;

                //update display if shown
                ApplyCurrent();
            }
        }
    }

    public float forceDuration {
        get { return mForceDuration; }
        set {
            if(mForceDuration != value) {
                mForceDuration = value;

                //update display if shown
                ApplyCurrent();
            }
        }
    }
        
    private bool mShow;
    private float mForce;
    private float mForceDuration;
    private float mRadius;
    private float mAngle = 0f;
    private Vector2 mDir = Vector2.right;
    private float mMass;

    public void ApplyCurrent() {
        if(mShow) {
            display.Evaluate(transform.position, mMass, mDir * mForce, mRadius, mForceDuration, duration);
        }
    }

    void Awake() {
        if(!display)
            display = GetComponent<PhysicsTrajectoryDisplay>();

        if(bodyTarget) {
            mMass = bodyTarget.mass;

            //TODO: assumes circle collider for now
            var circleColl = bodyTarget.GetComponent<CircleCollider2D>();
            if(circleColl)
                mRadius = circleColl.radius + radiusPadding;
            else
                mRadius = radiusPadding;

            var unitStateForceApply = bodyTarget.GetComponent<UnitStateForceApply>();
            if(unitStateForceApply)
                mForceDuration = unitStateForceApply.defaultDuration;
        }
        else {            
            mMass = 1f;
            mRadius = radiusPadding;
        }
    }
}
