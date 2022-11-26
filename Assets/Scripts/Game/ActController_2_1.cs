using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LoLExt;

public class ActController_2_1 : ActCannonController {
    [System.Serializable]
    public struct WheelInfo {
        public float mass;
        public Sprite sprite;
    }

    [Header("Cannon Control")]
    public M8.Animator.Animate cannonAnimator;
    [M8.Animator.TakeSelector(animatorField = "cannonAnimator")]
    public string cannonTakeEnter;

    public Transform knightRoot;
    
    public SpriteRenderer knightSpriteRender;
    public Color knightSpriteColorOverlay = Color.white;
    public string knightSpriteColorOverlayVar = "_ColorOverlay";
    public float knightSpriteColorOverlayPulse = 0.5f;
    public float knightSpriteColorOverlayAlphaMax = 0.5f;

    public SpriteRenderer knightWheelSpriteRender;
    public ParticleSystem knightFX;
    public int knightFXMaxParticle = 32;
    public Text knightWheelMassText;
    public M8.Animator.Animate knightAnimator;
    public float knightAnimatePushScaleMin = 0.5f;
    public float knightAnimatePushSpeedRef = 2.0f; //divide on current wheel to speed up/down animation
    [M8.Animator.TakeSelector(animatorField = "knightAnimator")]
    public string knightTakePush;
    [M8.Animator.TakeSelector(animatorField = "knightAnimator")]
    public string knightTakeMove;
    public float knightReturnDelay;
    public Transform knightReturnPoint;

    [Header("Sequence")]
    public AnimatorEnterExit seqIllustrateFormula;
    public ModalDialogController seqDlgFormula;
    public AnimatorEnterExit seqIllustrateAxis;
    public ModalDialogController seqDlgAxis;
    public ModalDialogController seqDlgKnightEnter;
    public AnimatorEnterExit seqPressLaunch;

    public AnimatorEnterExit seqPressGraph;
    public ModalDialogController seqDlgGraph;
    public ModalDialogController seqDlgPlay;
    public AnimatorEnterExit seqForceSlider;

    [System.Serializable]
    public struct WheelTimeDialogData {
        [M8.Localize]
        public string textRef;
        public float timeMin;
        public float timeMax;
        public GameObject goActive;
    }

    [Header("Wheel Time")]
    public AnimatorEnterExit wheelTimeAnim;
    public Slider wheelTimeSlider;
    [M8.Localize]
    public string wheelTimeSecondFormatTextRef;
    public Text wheelTimeThumbLabel;
    public GameObject wheelTimeGhost;
    public Transform wheelTimeGhostDisplayRoot;
    [M8.Localize]
    public string wheelTimeVelocityTextRef;
    public Text wheelTimeVelocityLabel;
    [M8.Localize]
    public string wheelTimeForceBalancedTextRef;
    [M8.Localize]
    public string wheelTimeForceUnbalancedTextRef;
    public Text wheelTimeForceStateLabel;
    public GameObject wheelTimeDialogGO;
    public Text wheelTimeDialogLabel;
    public WheelTimeDialogData[] wheelTimeDialogs;

    private bool mIsKnightLocked;
    private bool mIsGraphWait;

    private int mCurWheelTracerInd = 0;
    private int mCurWheelTracerMaxInd;
    private int mCurWheelDialogInd;
    private bool mIsWheelTimeWaitNext;

    private Material mKnightSpriteMat;
    private int mKnightSpriteColorOverlayID;
    private float mKnightSpriteColorOverlayCurAlphaMax = 0f;

    protected override void OnInstanceDeinit() {
        //
        if(mKnightSpriteMat)
            DestroyImmediate(mKnightSpriteMat);

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        seqIllustrateFormula.gameObject.SetActive(false);
        seqIllustrateAxis.gameObject.SetActive(false);
        seqPressLaunch.gameObject.SetActive(false);
        seqPressGraph.gameObject.SetActive(false);
        seqForceSlider.gameObject.SetActive(false);

        cannonAnimator.ResetTake(cannonTakeEnter);

        //
        wheelTimeSlider.onValueChanged.AddListener(OnWheelTimeSliderChanged);

        wheelTimeAnim.gameObject.SetActive(false);
        wheelTimeGhost.SetActive(false);

        for(int i = 0; i < wheelTimeDialogs.Length; i++) {
            if(wheelTimeDialogs[i].goActive)
                wheelTimeDialogs[i].goActive.SetActive(false);
        }

        if(knightFX)
            knightFX.gameObject.SetActive(false);

        mKnightSpriteMat = knightSpriteRender.material;
        mKnightSpriteColorOverlayID = Shader.PropertyToID(knightSpriteColorOverlayVar);
    }

    protected override IEnumerator Start() {
        yield return base.Start();

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

        ApplyCurrentWheelInfoDisplay();
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
        do { yield return null; } while(graphControl.tracer.isRecording || graphControl.tracer.points == null);


        //wheel time
        mCurWheelTracerMaxInd = graphControl.tracer.points.Count - 2;

        wheelTimeSlider.minValue = 0f;
        wheelTimeSlider.maxValue = mCurWheelTracerMaxInd;

        mCurWheelDialogInd = -1;
        ApplyWheelTime(0);

        wheelTimeGhost.SetActive(true);

        wheelTimeAnim.gameObject.SetActive(true);
        yield return wheelTimeAnim.PlayEnterWait();
                
        mIsWheelTimeWaitNext = true; //wait for slider to get to end where next is show, wait for it to be pressed.
        while(mIsWheelTimeWaitNext)
            yield return null;

        wheelTimeGhost.SetActive(false);

        yield return wheelTimeAnim.PlayExitWait();
        wheelTimeAnim.gameObject.SetActive(false);

        nextGO.SetActive(false);
        //


        //graph stuff
        graphButton.interactable = true;

        seqPressGraph.gameObject.SetActive(true);
        seqPressGraph.PlayEnter();

        mIsGraphWait = true;
        while(mIsGraphWait)
            yield return null;

        //dialog about graph
        yield return new WaitForSeconds(0.34f);

        seqDlgGraph.Play();
        while(seqDlgGraph.isPlaying)
            yield return null;

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
                
        ApplyCurrentWheelInfoDisplay();
        cannonAnimator.Play(cannonTakeEnter);
        while(cannonAnimator.isPlaying)
            yield return null;

        cannonLaunch.interactable = true;
        forceSlider.interactable = true;

        seqForceSlider.gameObject.SetActive(true);
        seqForceSlider.PlayEnter();

        StartCoroutine(DoKnightGlow());
    }

    protected override void OnNext() {
        if(mIsWheelTimeWaitNext) {
            mIsWheelTimeWaitNext = false;
            return;
        }

        base.OnNext();
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

        var ent = SpawnCannonball();
        StartCoroutine(DoKnightPush(ent));
    }

    protected override void OnForceValueChanged(float val) {
        if(seqForceSlider.gameObject.activeSelf && !seqForceSlider.animator.isPlaying)
            StartCoroutine(DoExitSeqAnimator(seqForceSlider));

        var sliderVal = forceSlider.normalizedValue;

        //update knight glow

        //update FX
        if(knightFX) {
            if(sliderVal > 0f) {
                knightFX.gameObject.SetActive(true);

                var fxDat = knightFX.main;
                fxDat.maxParticles = Mathf.RoundToInt(sliderVal * knightFXMaxParticle);
            }
            else
                knightFX.gameObject.SetActive(false);
        }

        mKnightSpriteColorOverlayCurAlphaMax = sliderVal * knightSpriteColorOverlayAlphaMax;
    }

    private void ApplyCurrentWheelInfoDisplay() {
        var cannonballSpawner = cannonballSpawnerCurrent;

        var cannonballSpriteRender = cannonballSpawner.template.GetComponentInChildren<SpriteRenderer>();

        //apply sprite
        if(cannonballSpriteRender)
            knightWheelSpriteRender.sprite = cannonballSpriteRender.sprite;

        var unitBody = cannonballSpawner.template.GetComponent<Rigidbody2D>();

        //apply mass info
        knightWheelMassText.text = string.Format("{0:G0}kg", unitBody.mass);

        knightWheelSpriteRender.gameObject.SetActive(true);
    }

    IEnumerator DoKnightPush(M8.EntityBase cannonEnt) {
        //apply wheel info on ent

        //wait for push
        var unitForceApply = cannonEnt.GetComponent<UnitStateForceApply>();

        while(!unitForceApply.unit.physicsEnabled)
            yield return null;

        graphControl.tracer.body = unitForceApply.unit.body;
        graphControl.tracer.Record();

        //push
        knightAnimator.Play(knightTakePush);

        knightWheelSpriteRender.gameObject.SetActive(false);        

        Vector2 wheelOfs = knightRoot.position - knightWheelSpriteRender.transform.position;
                
        while(unitForceApply.isPlaying) {
            knightRoot.position = unitForceApply.unit.position + wheelOfs;

            var animScale = Mathf.Max(knightAnimatePushScaleMin, Mathf.Abs(unitForceApply.unit.body.velocity.x / knightAnimatePushSpeedRef));
            knightAnimator.animScale = animScale;

            yield return null;
        }

        knightAnimator.animScale = 1.0f;
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
            ApplyCurrentWheelInfoDisplay();
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

    IEnumerator DoKnightGlow() {
        bool isActive = mKnightSpriteColorOverlayCurAlphaMax > 0f;
        float curPulseTime = 0f;
        float lastTime = Time.time;

        while(true) {
            if(isActive) {
                float time = Time.time;
                float delta = time - lastTime;
                lastTime = time;

                curPulseTime += delta;

                float t = Mathf.Sin(Mathf.PI * curPulseTime * knightSpriteColorOverlayPulse);
                t *= t;

                var clr = knightSpriteColorOverlay;
                clr.a = Mathf.Lerp(0f, mKnightSpriteColorOverlayCurAlphaMax, t);
                mKnightSpriteMat.SetColor(mKnightSpriteColorOverlayID, clr);

                if(mKnightSpriteColorOverlayCurAlphaMax <= 0f)
                    isActive = false;
            }
            else if(mKnightSpriteColorOverlayCurAlphaMax > 0f) {
                curPulseTime = 0f;
                lastTime = Time.time;
                isActive = true;
            }

            yield return null;
        }
    }

    void OnWheelTimeSliderChanged(float val) {
        int traceIndex = (int)val;
        if(mCurWheelTracerInd != traceIndex)
            ApplyWheelTime(traceIndex);
    }

    void ApplyWheelTime(int traceIndex) {
        mCurWheelTracerInd = traceIndex;

        var trace = graphControl.tracer.points[traceIndex];

        float time = graphControl.tracer.timeInterval * traceIndex;

        //update slider time
        wheelTimeThumbLabel.text = string.Format(M8.Localize.Get(wheelTimeSecondFormatTextRef), time);

        //update ghost position
        wheelTimeGhost.transform.position = trace.position;
        wheelTimeGhostDisplayRoot.localEulerAngles = new Vector3(0f, 0f, trace.rotate);

        //update stats
        wheelTimeVelocityLabel.text = string.Format("{0}\nX = {1:0.00} m/s\nY = {2:0.00} m/s", M8.Localize.Get(wheelTimeVelocityTextRef), trace.velocity.x, trace.velocity.y);

        bool isForceBalanced = trace.accelApprox == Vector2.zero;
        if(isForceBalanced)
            wheelTimeForceStateLabel.text = M8.Localize.Get(wheelTimeForceBalancedTextRef);
        else
            wheelTimeForceStateLabel.text = M8.Localize.Get(wheelTimeForceUnbalancedTextRef);

        //check if dialog needs to be played
        int dlgIndex = -1;
        GameObject goActive = null;
        string textRef = null;
        for(int i = 0; i < wheelTimeDialogs.Length; i++) {
            var dlg = wheelTimeDialogs[i];
            if(time >= dlg.timeMin && time <= dlg.timeMax) {
                dlgIndex = i;

                textRef = dlg.textRef;
                goActive = dlg.goActive;

                //update and play dialog?
                if(mCurWheelDialogInd != dlgIndex) {                    
                    wheelTimeDialogLabel.text = M8.Localize.Get(textRef);
                    LoLManager.instance.SpeakText(textRef);

                    if(goActive)
                        goActive.SetActive(true);
                }
                break;
            }
        }

        if(mCurWheelDialogInd != -1 && mCurWheelDialogInd != dlgIndex) {
            var dlg = wheelTimeDialogs[mCurWheelDialogInd];
            if(dlg.goActive && dlg.goActive != goActive)
                dlg.goActive.SetActive(false);
        }

        mCurWheelDialogInd = dlgIndex;

        wheelTimeDialogGO.SetActive(dlgIndex != -1 && !string.IsNullOrEmpty(textRef));

        if(mCurWheelTracerInd == mCurWheelTracerMaxInd) {
            nextGO.SetActive(true);
        }
    }
}