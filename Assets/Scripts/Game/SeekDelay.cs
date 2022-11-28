using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekDelay : MonoBehaviour {
    public float delay;
    public Rigidbody2D body;
    [M8.TagSelector]
    public string tagFilter;
    public LayerMask layerFilter;
    public float radius;    
    public float force;
    public float forceDelay;
    public float speedLimit;
    public GameObject activeGO;

    private Coroutine mRout;
    private WaitForSeconds mStartWait;
    private WaitForSeconds mForceWait;

    private const int mCollsCapacity = 16;
    private Collider2D[] mColls = new Collider2D[mCollsCapacity];

    void OnDisable() {
        if(mRout != null) {
            StopCoroutine(mRout);
            mRout = null;
        }
    }

    void OnEnable() {
        if(activeGO)
            activeGO.SetActive(false);

        mRout = StartCoroutine(DoSeek());
    }

    void Awake() {
        mStartWait = new WaitForSeconds(delay);
        mForceWait = new WaitForSeconds(forceDelay);
    }

    IEnumerator DoSeek() {
        yield return mStartWait;

        if(activeGO)
            activeGO.SetActive(true);

        //grab target
        Transform targetT = null;
        while(!targetT) {
            var targetCount = Physics2D.OverlapCircleNonAlloc(transform.position, radius, mColls, layerFilter);
            for(int i = 0; i < targetCount; i++) {
                var coll = mColls[i];
                if(coll.CompareTag(tagFilter)) {
                    targetT = coll.transform;
                    break;
                }
            }

            yield return mForceWait;
        }

        var t = transform;

        while(body.simulated && targetT && targetT.gameObject.activeSelf) {
            var vel = body.velocity;
            var spd = vel.magnitude;
            if(spd > speedLimit) {
                var bodyDir = vel / spd;
                body.velocity = bodyDir * speedLimit;
            }

            Vector2 dpos = targetT.position - t.position;
            Vector2 dir = dpos.normalized;

            body.AddForce(dir * force);

            yield return mForceWait;
        }

        if(activeGO)
            activeGO.SetActive(false);

        mRout = null;
    }

    void OnDrawGizmos() {
        if(radius > 0f) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
