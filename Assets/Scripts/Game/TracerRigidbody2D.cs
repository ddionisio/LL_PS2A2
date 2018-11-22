using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerRigidbody2D : MonoBehaviour {
    public struct PointData {
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 accelApprox;
    }

    public GameObject template;
    public string poolGroup = "tracer";
    public int capacity = 100;
    public float timeInterval = 0.1f;
    public bool noContactUseGravityAccelInfo = true;

    public Rigidbody2D body {
        get {
            return mBody;
        }
        set {
            if(mBody != value) {
                mBody = value;

                Clear();
                Stop();
            }
        }
    }

    public M8.CacheList<PointData> points { get; private set; }

    public Vector2 minVelocity { get { return mMinVelocity; } }
    public Vector2 maxVelocity { get { return mMaxVelocity; } }
    public Vector2 minPosition { get { return mMinPosition; } }
    public Vector2 maxPosition { get { return mMaxPosition; } }
    public Vector2 minAccelApprox { get { return mMinAccelApprox; } }
    public Vector2 maxAccelApprox { get { return mMaxAccelApprox; } }

    private Vector2 mMinVelocity = Vector2.zero;
    private Vector2 mMaxVelocity = Vector2.zero;
    private Vector2 mMinPosition = Vector2.zero;
    private Vector2 mMaxPosition = Vector2.zero;
    private Vector2 mMinAccelApprox = Vector2.zero;
    private Vector2 mMaxAccelApprox = Vector2.zero;

    public bool isRecording { get { return mRecordRout != null; } }

    private Rigidbody2D mBody;

    private M8.PoolController mPool;

    private Coroutine mRecordRout;

    private ContactPoint2D[] mBodyContacts = new ContactPoint2D[16];
    private int mBodyContactCount;

    private bool mIsInit;
    
    public void Record() {
        Clear();
        Stop();

        if(!mBody)
            return;

        mRecordRout = StartCoroutine(DoRecording());
    }

    public void Stop() {
        if(mRecordRout != null) {
            StopCoroutine(mRecordRout);
            mRecordRout = null;
        }
    }

    public void Clear() {
        Init();

        mPool.ReleaseAllByType(template.name);
        points.Clear();
    }

    void OnDisable() {
        Stop();
    }

    private void Init() {
        if(!mIsInit) {
            mPool = M8.PoolController.CreatePool(poolGroup);
            mPool.AddType(template, capacity, capacity);

            points = new M8.CacheList<PointData>(capacity);

            mIsInit = true;
        }
    }

    IEnumerator DoRecording() {
        var waitFixed = new WaitForFixedUpdate();

        while(!points.IsFull && mBody.simulated && mBody.gameObject.activeInHierarchy) {
            var pt = mBody.position;

            //var vel = mBody.velocity;
            Vector2 vel;
            if(points.Count == 0)
                vel = Vector2.zero;
            else
                vel = (pt - points[points.Count - 1].position) / timeInterval;
            
            Vector2 accel;

            if(points.Count == 0)
                accel = Vector2.zero;
            else {
                accel = (vel - points[points.Count - 1].velocity) / timeInterval;
            }

            //cheat
            if(noContactUseGravityAccelInfo) {
                mBodyContactCount = mBody.GetContacts(mBodyContacts);
                if(mBodyContactCount == 0)
                    accel.y = (mBody.gravityScale * Physics2D.gravity).y;
            }

            mPool.Spawn(template.name, points.Count.ToString(), null, pt, null);

            if(points.Count == 0) {
                mMinPosition = mMaxPosition = pt;
                mMinVelocity = mMaxVelocity = vel;
                mMinAccelApprox = mMaxAccelApprox = accel;
            }
            else {
                if(pt.x < mMinPosition.x)
                    mMinPosition.x = pt.x;
                if(pt.x > mMaxPosition.x)
                    mMaxPosition.x = pt.x;

                if(pt.y < mMinPosition.y)
                    mMinPosition.y = pt.y;
                if(pt.y > mMaxPosition.y)
                    mMaxPosition.y = pt.y;

                if(vel.x < mMinVelocity.x)
                    mMinVelocity.x = vel.x;
                if(vel.x > mMaxVelocity.x)
                    mMaxVelocity.x = vel.x;

                if(vel.y < mMinVelocity.y)
                    mMinVelocity.y = vel.y;
                if(vel.y > mMaxVelocity.y)
                    mMaxVelocity.y = vel.y;

                if(accel.x < mMinAccelApprox.x)
                    mMinAccelApprox.x = accel.x;
                if(accel.x > mMaxAccelApprox.x)
                    mMaxAccelApprox.x = accel.x;

                if(accel.y < mMinAccelApprox.y)
                    mMinAccelApprox.y = accel.y;
                if(accel.y > mMaxAccelApprox.y)
                    mMaxAccelApprox.y = accel.y;
            }

            points.Add(new PointData() { position=pt, velocity=vel, accelApprox=accel });
                        
            float curTime = 0f;
            while(curTime < timeInterval) {
                yield return waitFixed;
                curTime += Time.fixedDeltaTime;
            }
        }

        mRecordRout = null;
    }
}
