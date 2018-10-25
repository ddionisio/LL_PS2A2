using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchContact : MonoBehaviour {
    public Transform target;
    public float targetPixelHeight;
    public float targetPixelPerUnit = 32f;
    public float collisionStartOfs; //based on up dir
    public float collisionMaxDistance = 100f;
    public LayerMask collisionLayerMask;

    void OnEnable() {
        StartCoroutine(DoSetup());        
    }

    IEnumerator DoSetup() {
        target.gameObject.SetActive(false);

        yield return null;

        target.gameObject.SetActive(true);

        var dir = (Vector2)transform.up;
        var pt = (Vector2)transform.position + dir * collisionStartOfs;

        Vector2 dest;
        float dist;

        var hit = Physics2D.Raycast(pt, dir, collisionMaxDistance, collisionLayerMask);
        if(hit.collider) {
            dest = hit.point;
            dist = hit.distance;
        }
        else {
            dest = pt + dir * collisionMaxDistance;
            dist = collisionMaxDistance;
        }

        //set scale y to distance
        var tgtScale = target.localScale;

        if(targetPixelHeight <= 0f || targetPixelPerUnit <= 0f)
            tgtScale.y = dist;
        else
            tgtScale.y = dist * (targetPixelPerUnit / targetPixelHeight);

        target.localScale = tgtScale;

        //apply position, assume pivot is center
        target.position = Vector2.Lerp(pt, dest, 0.5f);
        target.rotation = Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, dir), Vector3.forward);
    }
}
