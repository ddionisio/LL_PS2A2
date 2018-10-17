using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTrajectoryDisplay : MonoBehaviour {    
    [Header("Display")]
    public GameObject template;
    public Transform templateRoot; //where to place displays
    public int capacity = 50;
    public float timeInterval = 0.1f;

    [Header("Collision")]
    public LayerMask collisionLayerMask; //for grounds and wall
    public bool collisionEndOnContact; //end trajectory on any contact?
    [M8.TagSelector]
    public string collisionEndTagFilter; //for ending the trajectory

    private M8.CacheList<GameObject> mDisplayActives;
    private M8.CacheList<GameObject> mDisplayInactives;

    private const int castCapacity = 8;
    private RaycastHit2D[] mCasts = new RaycastHit2D[castCapacity];
    private int mCastHitCount;

    public void Clear() {
        while(mDisplayActives.Count > 0) {
            var go = mDisplayActives.RemoveLast();
            go.SetActive(false);
            mDisplayInactives.Add(go);
        }
    }

    public void Evaluate(Vector2 pos, float mass, Vector2 force, float collisionRadius, float forceDuration, float duration) {
        Clear();

        float prevTime = 0f;
        Vector2 curAccel = Vector2.zero;
        Vector2 curVel = Vector2.zero;
        Vector2 curPos = pos;
        Vector2 forceGravity = Physics2D.gravity * mass;
        //Vector2 dir = force.normalized;

        for(float curTime = 0f; curTime <= duration; curTime += timeInterval) {
            if(mDisplayInactives.Count == 0) //no more displays
                break;

            var newPos = curPos + curVel * timeInterval;
            var dpos = newPos - curPos;

            Vector2 dir;
            float dist;
                        
            if(curVel.x != 0f || curVel.y != 0f) {
                //move current pos
                dist = dpos.magnitude;
                dir = dpos / dist;
            }
            else {
                //just grab current area to see if we are in contact with anything
                dist = 0f;
                dir = Vector2.zero;
            }

            mCastHitCount = Physics2D.CircleCastNonAlloc(curPos, collisionRadius, dir, mCasts, dist, collisionLayerMask);

            if(mCastHitCount > 0 && collisionEndOnContact)
                break;

            var netForce = forceGravity;

            if(curTime <= forceDuration)
                netForce += force;

            Vector2 forceNormals = Vector2.zero;
            
            if(mCastHitCount > 0f) {
                bool isEnd = false;

                //newPos = curPos;
                //bool newPosIsApplied = dist == 0f;

                for(int i = 0; i < mCastHitCount; i++) {
                    var hit = mCasts[i];

                    //check to see if we need to end based on tag
                    if(!string.IsNullOrEmpty(collisionEndTagFilter) && hit.collider.CompareTag(collisionEndTagFilter)) {
                        isEnd = true;
                        break;
                    }

                    //TODO: only do the first hit to determine displacement
                    //if(!newPosIsApplied) {
                        //newPos = curPos + dir * hit.distance;
                        //newPosIsApplied = true;
                    //}

                    //TODO: proper math
                    forceNormals += new Vector2(hit.normal.x * Mathf.Abs(netForce.x), hit.normal.y * Mathf.Abs(netForce.y));
                }

                if(isEnd)
                    break;

                //dpos = newPos - curPos;
            }

            netForce += forceNormals;

            curAccel = netForce / mass;

            float deltaTime = curTime - prevTime;

            if(deltaTime == 0f) {
                if(forceDuration == 0f || forceDuration < Time.fixedDeltaTime)
                    curTime = deltaTime = Time.fixedDeltaTime;
                else if(forceDuration < timeInterval)
                    curTime = deltaTime = forceDuration;
            }

            if(curAccel != Vector2.zero) {
                curVel += curAccel * deltaTime;
            }
            else {
                curVel = dpos / deltaTime;
            }

            curPos = newPos;

            //add point
            if(dpos != Vector2.zero) {
                var go = mDisplayInactives.RemoveLast();
                go.SetActive(true);
                go.transform.position = curPos;
                mDisplayActives.Add(go);
            }

            prevTime = curTime;
        }
    }

    void OnDisable() {
        Clear();
    }

    void Awake() {
        mDisplayActives = new M8.CacheList<GameObject>(capacity);
        mDisplayInactives = new M8.CacheList<GameObject>(capacity);

        for(int i = 0; i < capacity; i++) {
            var newGO = Object.Instantiate<GameObject>(template);
            newGO.transform.SetParent(templateRoot);
            newGO.SetActive(false);

            mDisplayInactives.Add(newGO);
        }
    }
}
