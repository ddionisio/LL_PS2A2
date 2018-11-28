using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActController_3_end : GameModeController<ActController_3_end> {

    [Header("Sequence")]
    public float startDelay = 1f;

    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takePlay;

    public float endDelay = 0.5f;

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        animator.ResetTake(takePlay);
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        yield return new WaitForSeconds(startDelay);

        animator.Play(takePlay);
        while(animator.isPlaying)
            yield return null;

        yield return new WaitForSeconds(endDelay);

        GameData.instance.Progress();
    }
}