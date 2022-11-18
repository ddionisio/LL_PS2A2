using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchContact : MonoBehaviour {
    public Transform target;
    public Transform endTarget;
    public bool endTargetApplyUp = true;
    public float targetPixelHeight;
    public float targetPixelPerUnit = 32f;
    public float collisionStartOfs; //based on up dir
    public float collisionMaxDistance = 100f;
    public LayerMask collisionLayerMask;

    public bool alwaysUpdate;

    void OnEnable() {
        StartCoroutine(DoSetup());        
    }

    IEnumerator DoSetup() {
        target.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        target.gameObject.SetActive(true);

        while(true) {
            var tgtScale = target.localScale;

            var dir = (Vector2)transform.up;
            var pt = (Vector2)transform.position + dir * collisionStartOfs;

            Vector2 dest;
            float dist;

            /*
            //grab the longest of hit between sides
            var extX = tgtScale.x * 0.5f;

            var dirL = new Vector2(dir.y, -dir.x);
            var dirR = new Vector2(-dir.y, dir.x);

            var hitL = Physics2D.Raycast(pt + extX * dirL, dir, collisionMaxDistance, collisionLayerMask);
            var hitR = Physics2D.Raycast(pt + extX * dirR, dir, collisionMaxDistance, collisionLayerMask);

            RaycastHit2D hit;
            if(hitL.collider && hitL.distance > hitR.distance) {
                hit = hitL;
            }
            else {
                hit = hitR;
            }*/

            var hit = Physics2D.Raycast(pt, dir, collisionMaxDistance, collisionLayerMask);

            if(hit.collider) {
                dest = pt + dir * hit.distance;
                dist = hit.distance;
            }
            else {
                dest = pt + dir * collisionMaxDistance;
                dist = collisionMaxDistance;
            }

            //set scale y to distance
            if(targetPixelHeight <= 0f || targetPixelPerUnit <= 0f)
                tgtScale.y = dist;
            else
                tgtScale.y = dist * (targetPixelPerUnit / targetPixelHeight);

            target.localScale = tgtScale;

            //apply position, assume pivot is center
            var toPt = Vector2.Lerp(pt, dest, 0.5f);
            target.position = new Vector3(toPt.x, toPt.y, target.position.z);
            target.rotation = Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, dir), Vector3.forward);

            if(endTarget) {
                endTarget.position = dest;
                if(endTargetApplyUp)
                    endTarget.rotation = target.rotation;
            }

            if(!alwaysUpdate)
                break;

            //wait for position/rotation change
            var lastPos = transform.position;
            var lastRot = transform.rotation;
            while(transform.position == lastPos && transform.rotation == lastRot)
                yield return null;
        }
    }
}
