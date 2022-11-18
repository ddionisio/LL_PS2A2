using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorPlayInterval : MonoBehaviour {
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string take;
    public float delayMin;
    public float delayMax;
    public float animScaleMin = 1f;
    public float animScaleMax = 1f;

    void OnEnable() {
        StartCoroutine(DoPlay());
    }

    IEnumerator DoPlay() {
        while(true) {
            var delay = Random.Range(delayMin, delayMax);
            yield return new WaitForSeconds(delay);

            animator.animScale = Random.Range(animScaleMin, animScaleMax);

            animator.Play(take);
            while(animator.isPlaying)
                yield return null;
        }
    }
}
