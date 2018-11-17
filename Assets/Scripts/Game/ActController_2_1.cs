using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActController_2_1 : ActCannonController {

    [Header("Cannon Control")]
    public M8.Animator.Animate cannonAnimator;
    [M8.Animator.TakeSelector(animatorField = "cannonAnimator")]
    public string cannonTakeEnter;
        
    public Transform knightRoot;
    public SpriteRenderer knightSpriteRender;
    public GameObject knightWheelGO;    
    public M8.Animator.Animate knightAnimator;
    [M8.Animator.TakeSelector(animatorField = "knightAnimator")]
    public string knightTakePush;
    [M8.Animator.TakeSelector(animatorField = "knightAnimator")]
    public string knightTakeMove;
    public float knightReturnDelay;
    public Transform knightReturnPoint;

    [Header("Sequence")]
    public AnimatorEnterExit seqTitle;
    public ModalDialogController seqDlgIntro;
    public AnimatorEnterExit seqIllustrateFormula;
    public ModalDialogController seqDlgFormula;

    protected override void OnInstanceDeinit() {
        //

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        cannonAnimator.ResetTake(cannonTakeEnter);

        //
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro part

        cannonInterfaceGO.SetActive(true);
                
        cannonAnimator.Play(cannonTakeEnter);
        while(cannonAnimator.isPlaying)
            yield return null;

        //some other stuff?

        //enable cannon launch
        cannonLaunch.interactable = true;

        //wait for launch
        mIsCannonballFree = true; //free shot
        mIsLaunchWait = true;
        while(mIsLaunchWait)
            yield return null;

        //explanations and such

        //show targets
        ShowTargets();

        //enable force input
        forceSlider.interactable = true;
    }

    protected override void OnFinish() {
        //victory
    }

    protected override void OnLaunched() {
        base.OnLaunched();

        cannonLaunch.interactable = false;

        var ent = cannonballSpawner.Spawn();
        StartCoroutine(DoKnightPush(ent));
    }

    IEnumerator DoKnightPush(M8.EntityBase cannonEnt) {
        //wait for push
        var unitForceApply = cannonEnt.GetComponent<UnitStateForceApply>();

        while(!unitForceApply.unit.physicsEnabled)
            yield return null;

        graphControl.tracer.body = unitForceApply.unit.body;
        graphControl.tracer.Record();

        //push
        knightAnimator.Play(knightTakePush);

        knightWheelGO.SetActive(false);        

        Vector2 wheelOfs = knightRoot.position - knightWheelGO.transform.position;
                
        while(unitForceApply.isPlaying) {
            knightRoot.position = unitForceApply.unit.position + wheelOfs;
            yield return null;
        }
        //

        //victory thing

        //move back
        knightAnimator.Play(knightTakeMove);
        knightSpriteRender.flipX = true;

        Vector2 startPos = knightRoot.position;
        Vector2 endPos = knightReturnPoint.position;

        float curTime = 0f;
        while(curTime < knightReturnDelay) {
            curTime += Time.deltaTime;

            float t = Mathf.Clamp01(curTime / knightReturnDelay);

            knightRoot.position = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }
        //

        knightSpriteRender.flipX = false;

        knightWheelGO.SetActive(true);

        cannonAnimator.Play(cannonTakeEnter);
        while(cannonAnimator.isPlaying)
            yield return null;

        //wait for tracer to finish
        while(graphControl.tracer.isRecording)
            yield return null;

        cannonLaunch.interactable = true;

        //populate graph, enable graph button
        GraphPopulate();
    }
}