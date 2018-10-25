using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalPlayAnimationToNext : MonoBehaviour {
    public Rigidbody2D body; //turn off simulator

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeReceive; //take to play on receive
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeNext; //take to play after sending next signal

    [Header("Signals")]
    public M8.Signal signalReceive; //signal to listen to
    public M8.Signal signalNext; //signal after animation

    void OnDestroy() {
        signalReceive.callback -= OnSignalReceive;
    }

    void Awake() {
        signalReceive.callback += OnSignalReceive;
    }

    void OnSignalReceive() {
        StartCoroutine(DoReceive());
    }

    IEnumerator DoReceive() {
        if(body) body.simulated = false;

        if(animator && !string.IsNullOrEmpty(takeReceive)) {
            animator.Play(takeReceive);
            while(animator.isPlaying)
                yield return null;
        }

        if(signalNext != null) signalNext.Invoke();

        if(animator && !string.IsNullOrEmpty(takeNext))
            animator.Play(takeNext);
    }
}
