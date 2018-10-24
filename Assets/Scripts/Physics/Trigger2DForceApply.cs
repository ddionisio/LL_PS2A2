using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2DForceApply : MonoBehaviour {
    public float force;

    [M8.TagSelector]
    public string tagFilter;

    private const int collCapacity = 8;
    private M8.CacheList<Rigidbody2D> mBodies = new M8.CacheList<Rigidbody2D>(collCapacity);

    void OnTriggerEnter2D(Collider2D collision) {
        if(!string.IsNullOrEmpty(tagFilter) && !collision.CompareTag(tagFilter))
            return;

        bool exists = false;
        for(int i = 0; i < mBodies.Count; i++) {
            if(mBodies[i] && mBodies[i].gameObject == collision.gameObject) {
                exists = true;
                break;
            }
        }

        if(!exists) {
            var body = collision.GetComponent<Rigidbody2D>();
            if(body)
                mBodies.Add(body);
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if(!string.IsNullOrEmpty(tagFilter) && !collision.CompareTag(tagFilter))
            return;

        for(int i = 0; i < mBodies.Count; i++) {
            if(mBodies[i] && mBodies[i].gameObject == collision.gameObject) {
                mBodies.RemoveAt(i);
                break;
            }
        }
    }

    void FixedUpdate() {
        if(mBodies.Count == 0)
            return;

        var forceVector = (Vector2)transform.up * force;

        for(int i = mBodies.Count - 1; i >= 0; i--) {
            var body = mBodies[i];
            if(!body || !body.gameObject.activeInHierarchy) { //fail-safe
                mBodies.RemoveAt(i);
                continue;
            }

            body.AddForce(forceVector);
        }
    }
}
