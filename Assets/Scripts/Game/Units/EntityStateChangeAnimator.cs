using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Play animation based on 'stateFrom', then change to 'stateTo'
/// </summary>
public class EntityStateChangeAnimator : MonoBehaviour {    
    public M8.EntityBase entity;
    public M8.EntityState stateFrom;
    public M8.EntityState stateTo;
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string take;
    public bool waitForAnimationEnd; //wait for animation to finish?
    public float waitDelay; //delay before entering to stateTo

    void OnDestroy() {
        if(entity)
            entity.setStateCallback -= OnEntityStateChanged;
    }

    void Awake() {
        if(!entity)
            entity = GetComponent<M8.EntityBase>();

        entity.setStateCallback += OnEntityStateChanged;
    }

    void OnEntityStateChanged(M8.EntityBase ent) {
        if(ent.state == stateFrom) {
            StartCoroutine(DoStateChange());
        }
    }

    IEnumerator DoStateChange() {
        if(animator && !string.IsNullOrEmpty(take)) {
            animator.Play(take);
            if(waitForAnimationEnd) {
                while(animator.isPlaying)
                    yield return null;
            }
        }

        if(waitDelay > 0f)
            yield return new WaitForSeconds(waitDelay);

        entity.state = stateTo;
    }
}
