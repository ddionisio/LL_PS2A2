using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActCannonController : GameModeController<ActCannonController> {
    [Header("Display")]
    public PhysicsTrajectoryDisplayControl trajectoryDisplayControl;

    [Header("Force")]
    public Slider forceSlider;
    public float forceStart = 16f;
    public float forceMin = 10f;
    public float forceMax = 50f;

    [Header("Angle")]
    public SliderRadial angleSlider;
    public float angleStart;
    public float angleMin = 15f;
    public float angleMax = 80f;

    [Header("General Interface")]
    public GameObject cannonInterfaceGO;
    public Button cannonLaunch;
    public Button graphButton;
    
    [Header("Data")]
    public int cannonballCount = 10;
    public ProjectileEntitySpawner cannonballSpawner;

    [Header("Targets")]
    [M8.TagSelector]
    public string targetTag;
    public M8.EntityState targetStateSpawn;
    public M8.EntityState targetStateDespawn;
    public float targetDelayMin;
    public float targetDelayMax;

    [Header("Graph")]
    public TracerGraphControl graphControl;

    [Header("Next")]
    public GameObject nextGO;
    public M8.Signal nextSignal;
    public string nextModal;

    public int cannonballLaunced { get { return mCannonballLaunched; } }

    public event System.Action cannonballLaunchedCallback;
        
    protected bool mIsCannonballFree = false; //set to true to not decrement cannonball count on launch

    protected bool mIsLaunchWait = false;

    private int mCannonballLaunched = 0;

    private M8.CacheList<UnitEntity> mActiveTargets;
    private int mTargetCount;

    private int mGraphCurStartIndex;
    private int mGraphTimeCount;

    public void ShowTargets() {
        StartCoroutine(DoTargetsShow());
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        if(forceSlider) {
            forceSlider.minValue = forceMin;
            forceSlider.maxValue = forceMax;
            forceSlider.value = forceStart;
            forceSlider.onValueChanged.Invoke(forceStart);

            forceSlider.onValueChanged.AddListener(OnForceValueChanged);
        }

        if(angleSlider) {
            angleSlider.minValue = angleMin;
            angleSlider.maxValue = angleMax;
            angleSlider.startAngle = -angleMin;
            angleSlider.endAngle = -angleMax;
            angleSlider.value = angleStart;
            angleSlider.onValueChanged.Invoke(angleStart);

            angleSlider.onValueChanged.AddListener(OnAngleChanged);
        }

        var targetGOs = GameObject.FindGameObjectsWithTag(targetTag);

        var entList = new List<UnitEntity>();
        for(int i = 0; i < targetGOs.Length; i++) {
            var ent = targetGOs[i].GetComponent<UnitEntity>();
            if(ent)
                entList.Add(ent);
        }

        mActiveTargets = new M8.CacheList<UnitEntity>(entList.Count);
        for(int i = 0; i < entList.Count; i++) {
            var ent = entList[i];

            ent.setStateCallback += OnTargetChangeState;
            ent.releaseCallback += OnTargetReleased;

            ent.gameObject.SetActive(false);

            mActiveTargets.Add(ent);
        }

        mTargetCount = mActiveTargets.Count;

        cannonLaunch.onClick.AddListener(OnLaunched);

        graphButton.onClick.AddListener(OnShowGraph);
        graphButton.interactable = false;

        //start with interfaces hidden and disabled
        cannonInterfaceGO.SetActive(false);

        SetInteractiveEnable(false);

        nextGO.SetActive(false);

        nextSignal.callback += OnNext;
    }

    protected override void OnInstanceDeinit() {
        nextSignal.callback -= OnNext;

        if(mActiveTargets != null) {
            for(int i = 0; i < mActiveTargets.Count; i++) {
                if(mActiveTargets[i]) {
                    mActiveTargets[i].setStateCallback -= OnTargetChangeState;
                    mActiveTargets[i].releaseCallback -= OnTargetReleased;
                }
            }
            mActiveTargets.Clear();
        }

        base.OnInstanceDeinit();
    }

    protected virtual void OnFinish() {
        nextGO.SetActive(true);
    }

    protected virtual void OnNext() { //called when next is pressed (activated during OnFinish)
        if(!string.IsNullOrEmpty(nextModal)) {
            M8.UIModal.Manager.instance.ModalCloseAll();

            var modalParms = new M8.GenericParams();
            modalParms[TargetCrossoutCounterWidget.parmTargetCount] = mTargetCount;
            modalParms[TargetCrossoutCounterWidget.parmTargetCrossCount] = mTargetCount - mActiveTargets.Count;

            M8.UIModal.Manager.instance.ModalOpen(nextModal, modalParms);
        }
    }

    protected virtual void OnLaunched() {
        graphButton.interactable = false;

        mIsLaunchWait = false;

        if(!mIsCannonballFree) {
            if(mCannonballLaunched < cannonballCount) {
                mCannonballLaunched++;
                if(mCannonballLaunched == cannonballCount)
                    OnFinish();
            }
        }
        else
            mIsCannonballFree = false;

        if(cannonballLaunchedCallback != null)
            cannonballLaunchedCallback();
    }

    protected virtual void OnShowGraph() {
        graphControl.ShowGraph();
    }

    protected virtual void OnForceValueChanged(float val) {

    }

    protected virtual void OnAngleChanged(float val) {

    }

    protected void GraphPopulate(bool enableGraphButton) {
        if(graphControl.tracer.points.Count == 0) {
            graphButton.interactable = false;
            return;
        }

        graphControl.GraphPopulate();

        if(enableGraphButton)
            graphButton.interactable = true;
    }
    
    protected void SetInteractiveEnable(bool interact) {
        if(forceSlider) forceSlider.interactable = interact;
        if(angleSlider) angleSlider.interactable = interact;
        if(cannonLaunch) cannonLaunch.interactable = interact;
    }

    void OnTargetChangeState(M8.EntityBase ent) {
        //targetStateDespawn
        if(ent.state == targetStateDespawn) {
            RemoveTarget((UnitEntity)ent);
            if(mActiveTargets.Count == 0)
                OnFinish();
        }
    }

    void OnTargetReleased(M8.EntityBase ent) {
        //fail safe if not despawned by released somehow
        RemoveTarget((UnitEntity)ent);
        if(mActiveTargets.Count == 0)
            OnFinish();
    }

    private void RemoveTarget(UnitEntity ent) {
        if(mActiveTargets.Remove(ent)) {
            ent.setStateCallback -= OnTargetChangeState;
            ent.releaseCallback -= OnTargetReleased;
        }
    }

    IEnumerator DoTargetsShow() {
        for(int i = 0; i < mActiveTargets.Count; i++) {
            var ent = mActiveTargets[i];
            if(!ent.gameObject.activeSelf) {
                ent.gameObject.SetActive(true);

                ent.state = targetStateSpawn;

                yield return new WaitForSeconds(Random.Range(targetDelayMin, targetDelayMax));
            }
        }

        for(int i = 0; i < mActiveTargets.Count; i++) {
            mActiveTargets[i].physicsEnabled = true;
        }
    }
}
