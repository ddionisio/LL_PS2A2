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
    public AnimatorEnterExit seqIllustrateAxis;
    public ModalDialogController seqDlgAxis;
    public ModalDialogController seqDlgKnightEnter;
    public AnimatorEnterExit seqPressLaunch;
    public AnimatorEnterExit seqIllustrateTraces1;
    public ModalDialogController seqDlgTraces1;
    public AnimatorEnterExit seqIllustrateTraces2;
    public ModalDialogController seqDlgTraces2;
    public AnimatorEnterExit seqIllustrateTraces3;
    public ModalDialogController seqDlgTraces3;
    public AnimatorEnterExit seqPressGraph;
    public ModalDialogController seqDlgPlay;
    public AnimatorEnterExit seqForceSlider;

    private bool mIsKnightLocked;
    private bool mIsGraphWait;

    protected override void OnInstanceDeinit() {
        //

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        seqTitle.gameObject.SetActive(false);
        seqIllustrateFormula.gameObject.SetActive(false);
        seqIllustrateAxis.gameObject.SetActive(false);
        seqPressLaunch.gameObject.SetActive(false);
        seqIllustrateTraces1.gameObject.SetActive(false);
        seqIllustrateTraces2.gameObject.SetActive(false);
        seqIllustrateTraces3.gameObject.SetActive(false);
        seqPressGraph.gameObject.SetActive(false);
        seqForceSlider.gameObject.SetActive(false);

        cannonAnimator.ResetTake(cannonTakeEnter);

        //
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro part
        seqTitle.gameObject.SetActive(true);
        yield return seqTitle.PlayEnterWait();

        seqDlgIntro.Play();
        while(seqDlgIntro.isPlaying)
            yield return null;

        yield return seqTitle.PlayExitWait();
        seqTitle.gameObject.SetActive(false);

        seqIllustrateFormula.gameObject.SetActive(true);
        yield return seqIllustrateFormula.PlayEnterWait();

        seqDlgFormula.Play();
        while(seqDlgFormula.isPlaying)
            yield return null;
                
        yield return seqIllustrateFormula.PlayExitWait();
        seqIllustrateFormula.gameObject.SetActive(false);

        seqIllustrateAxis.gameObject.SetActive(true);
        yield return seqIllustrateAxis.PlayEnterWait();

        seqDlgAxis.Play();
        while(seqDlgAxis.isPlaying)
            yield return null;

        yield return seqIllustrateAxis.PlayExitWait();
        seqIllustrateAxis.gameObject.SetActive(false);

        cannonAnimator.Play(cannonTakeEnter);
        while(cannonAnimator.isPlaying)
            yield return null;

        seqDlgKnightEnter.Play();
        while(seqDlgKnightEnter.isPlaying)
            yield return null;

        cannonInterfaceGO.SetActive(true);

        yield return new WaitForSeconds(0.34f);

        //enable cannon launch
        mIsKnightLocked = true;

        cannonLaunch.interactable = true;

        seqPressLaunch.gameObject.SetActive(true);
        seqPressLaunch.PlayEnter();

        //wait for launch
        mIsCannonballFree = true; //free shot
        mIsLaunchWait = true;
        while(mIsLaunchWait)
            yield return null;

        //wait for tracer to finish        
        do { yield return null; } while(graphControl.tracer.isRecording);

        //explain traces 1
        seqIllustrateTraces1.gameObject.SetActive(true);
        yield return seqIllustrateTraces1.PlayEnterWait();

        yield return new WaitForSeconds(0.5f);

        seqDlgTraces1.Play();
        while(seqDlgTraces1.isPlaying)
            yield return null;

        yield return seqIllustrateTraces1.PlayExitWait();
        seqIllustrateTraces1.gameObject.SetActive(false);
        //

        //explain traces 2
        seqIllustrateTraces2.gameObject.SetActive(true);
        yield return seqIllustrateTraces2.PlayEnterWait();

        seqDlgTraces2.Play();
        while(seqDlgTraces2.isPlaying)
            yield return null;

        yield return seqIllustrateTraces2.PlayExitWait();
        seqIllustrateTraces2.gameObject.SetActive(false);
        //

        //explain traces 3
        seqIllustrateTraces3.gameObject.SetActive(true);
        yield return seqIllustrateTraces3.PlayEnterWait();

        seqDlgTraces3.Play();
        while(seqDlgTraces3.isPlaying)
            yield return null;

        yield return seqIllustrateTraces3.PlayExitWait();
        seqIllustrateTraces3.gameObject.SetActive(false);
        //

        //graph stuff
        graphButton.interactable = true;

        seqPressGraph.gameObject.SetActive(true);
        seqPressGraph.PlayEnter();

        mIsGraphWait = true;
        while(mIsGraphWait)
            yield return null;

        //dialog about graph

        //wait for graph to be closed
        while(graphControl.graphGO.activeSelf)
            yield return null;
        //
        
        seqPressGraph.gameObject.SetActive(false);

        //show targets
        ShowTargets();

        yield return new WaitForSeconds(1.5f);

        seqDlgPlay.Play();
        while(seqDlgPlay.isPlaying)
            yield return null;
        //

        //ready to play normally
        mIsKnightLocked = false;

        knightWheelGO.SetActive(true);
        cannonAnimator.Play(cannonTakeEnter);
        while(cannonAnimator.isPlaying)
            yield return null;

        cannonLaunch.interactable = true;
        forceSlider.interactable = true;

        seqForceSlider.gameObject.SetActive(true);
        seqForceSlider.PlayEnter();
    }

    protected override void OnFinish() {
        //victory
    }

    protected override void OnShowGraph() {
        base.OnShowGraph();

        if(seqPressGraph.gameObject.activeSelf)
            StartCoroutine(DoExitSeqAnimator(seqPressGraph));

        mIsGraphWait = false;
    }

    protected override void OnLaunched() {
        base.OnLaunched();

        if(seqPressLaunch.gameObject.activeSelf)
            StartCoroutine(DoExitSeqAnimator(seqPressLaunch));

        cannonLaunch.interactable = false;

        var ent = cannonballSpawner.Spawn();
        StartCoroutine(DoKnightPush(ent));
    }

    protected override void OnForceValueChanged(float val) {
        if(seqForceSlider.gameObject.activeSelf && !seqForceSlider.animator.isPlaying)
            StartCoroutine(DoExitSeqAnimator(seqForceSlider));
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
                                
        //wait for tracer to finish
        while(graphControl.tracer.isRecording)
            yield return null;

        GraphPopulate(!mIsKnightLocked);

        if(!mIsKnightLocked) {
            knightWheelGO.SetActive(true);
            cannonAnimator.Play(cannonTakeEnter);
            while(cannonAnimator.isPlaying)
                yield return null;

            cannonLaunch.interactable = true;
        }
    }

    IEnumerator DoExitSeqAnimator(AnimatorEnterExit seq) {
        seq.PlayExit();
        yield return seq.PlayExitWait();
        seq.gameObject.SetActive(false);
    }
}