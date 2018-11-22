using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2DGotoPlayAnimator : MonoBehaviour {
    [Header("Data")]
    public Rigidbody2D body;
    public Collider2D coll;
    [M8.TagSelector]
    public string tagFilter;

    [Header("Move")]
    public float moveDelay;
    public DG.Tweening.Ease moveEase;

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takePlay;

    void OnTriggerEnter2D(Collider2D collision) {
        if(!string.IsNullOrEmpty(tagFilter) && !collision.CompareTag(tagFilter))
            return;

        StartCoroutine(DoPlay(collision.transform.position));
    }

    IEnumerator DoPlay(Vector2 toPos) {
        if(body) body.simulated = false;
        if(coll) coll.enabled = false;

        var trans = transform;
        Vector2 fromPos = trans.position;
        Vector2 fromUp = trans.up;

        //move to destination
        var easeFunc = DG.Tweening.Core.Easing.EaseManager.ToEaseFunction(moveEase);
        float curTime = 0f;
        while(curTime < moveDelay) {
            yield return null;

            curTime += Time.deltaTime;

            float t = easeFunc(curTime, moveDelay, 0f, 0f);

            trans.position = Vector2.Lerp(fromPos, toPos, t);
            trans.up = Vector2.Lerp(fromUp, Vector2.up, t);
        }

        //play animation
        if(animator && !string.IsNullOrEmpty(takePlay)) {
            animator.Play(takePlay);
            while(animator.isPlaying)
                yield return null;
        }
    }
}
