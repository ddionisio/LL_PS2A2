using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerRigidbody2D : MonoBehaviour {
    public struct PointData {
        public Vector2 position;
        public Vector2 velocity;
    }

    public GameObject template;
    public string poolGroup = "tracer";
    public int capacity = 100;
    public float timeInterval = 0.1f;

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

    public bool isRecording { get { return mRecordRout != null; } }

    private Rigidbody2D mBody;

    private M8.PoolController mPool;

    private Coroutine mRecordRout;
    
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
        mPool.ReleaseAllByType(template.name);
        points.Clear();
    }

    void OnDisable() {
        Stop();
    }

    void Awake() {
        mPool = M8.PoolController.CreatePool(poolGroup);
        mPool.AddType(template, capacity, capacity);

        points = new M8.CacheList<PointData>(capacity);
    }

    IEnumerator DoRecording() {
        var waitFixed = new WaitForFixedUpdate();

        while(!points.IsFull && mBody.simulated && mBody.gameObject.activeInHierarchy) {
            var pt = mBody.position;
            var vel = mBody.velocity;

            mPool.Spawn(template.name, points.Count.ToString(), null, pt, null);

            points.Add(new PointData() { position=pt, velocity=vel });

            float curTime = 0f;
            while(curTime < timeInterval) {
                yield return waitFixed;
                curTime += Time.fixedDeltaTime;
            }
        }

        mRecordRout = null;
    }
}
