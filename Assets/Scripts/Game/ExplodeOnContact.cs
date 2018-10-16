using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M8;

public class ExplodeOnContact : MonoBehaviour {
    [TagSelector]
    public string tagFilter;

    [Header("Explode Data")]
    public float explodeRadius;
    [TagSelector]
    public string explodeTagFilter;
    public LayerMask explodeLayerMask;
    public float explodeForce;
    public ForceMode2D explodeForceMode;
    public float explodeUpwardsModifier;
    public bool explodeApplyWearOff;

    private const int collCapacity = 8;
    private Collider2D[] mColls = new Collider2D[collCapacity];

    private void OnCollisionEnter2D(Collision2D collision) {
        if(!string.IsNullOrEmpty(tagFilter) && !collision.collider.CompareTag(tagFilter))
            return;

        Vector2 pos = transform.position;

        var collsCount = Physics2D.OverlapCircleNonAlloc(pos, explodeRadius, mColls, explodeLayerMask);
        for(int i = 0; i < collsCount; i++) {
            var coll = mColls[i];

            if(!string.IsNullOrEmpty(explodeTagFilter) && !coll.CompareTag(explodeTagFilter))
                continue;

            var body = coll.GetComponent<Rigidbody2D>();
            if(!body)
                continue;

            if(explodeUpwardsModifier != 0f)
                body.AddExplosionForce(explodeForce, pos, explodeRadius, explodeUpwardsModifier, explodeApplyWearOff, explodeForceMode);
            else
                body.AddExplosionForce(explodeForce, pos, explodeRadius, explodeApplyWearOff, explodeForceMode);
        }
    }

    private void OnDrawGizmos() {
        if(explodeRadius > 0f) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explodeRadius);
        }
    }
}
