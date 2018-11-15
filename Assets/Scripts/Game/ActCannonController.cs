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
    public GameObject graphGO;
    public Slider graphTimeSlider;
    public Text graphTimeMaxLabel;    
    public float graphTimeStep = 0.1f;
    public GraphLineWidget graphXPosition;
    public GraphBarWidget graphXVelocity;
    public GraphBarWidget graphXAccel;

    public TracerRigidbody2D tracer;

    public int cannonballLaunced { get { return mCannonballLaunched; } }

    public event System.Action cannonballLaunchedCallback;
        
    protected bool mIsCannonballFree = false; //set to true to not decrement cannonball count on launch

    protected bool mIsLaunchWait = false;

    private int mCannonballLaunched = 0;

    private M8.CacheList<UnitEntity> mActiveTargets;

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
        }

        if(angleSlider) {
            angleSlider.minValue = angleMin;
            angleSlider.maxValue = angleMax;
            angleSlider.startAngle = -angleMin;
            angleSlider.endAngle = -angleMax;
            angleSlider.value = angleStart;
            angleSlider.onValueChanged.Invoke(angleStart);
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

        cannonLaunch.onClick.AddListener(OnLaunched);

        graphButton.onClick.AddListener(OnShowGraph);
        graphButton.interactable = false;

        graphTimeSlider.onValueChanged.AddListener(OnGraphTimeSlider);

        graphGO.SetActive(false);

        //start with interfaces hidden and disabled
        cannonInterfaceGO.SetActive(false);

        SetInteractiveEnable(false);
    }

    protected override void OnInstanceDeinit() {
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
        graphGO.SetActive(true);
    }

    protected void GraphPopulate() {
        if(tracer.points.Count == 0) {
            graphButton.interactable = false;
            return;
        }

        /*private int mGraphCurStartIndex;
    private int mGraphTimeCount;*/

        mGraphCurStartIndex = 0;

        UpdateGraph();

        //setup slider
        int count = graphXPosition.barHorizontalCount;
        float timeDuration = tracer.points.Count * graphTimeStep;

        if(tracer.points.Count > count) {
            graphTimeSlider.interactable = true;
            graphTimeSlider.minValue = 0f;
            graphTimeSlider.maxValue = tracer.points.Count - count;
            graphTimeMaxLabel.text = (graphTimeSlider.maxValue * graphTimeStep).ToString("0.0");
        }
        else {
            graphTimeSlider.interactable = false;
            graphTimeMaxLabel.text = timeDuration.ToString("0.0");
        }

        graphTimeSlider.value = 0f;

        graphButton.interactable = true;
    }

    private void UpdateGraph() {
        if(tracer.points.Count == 0)
            return;

        int count = graphXPosition.barHorizontalCount;
        float timeDuration = count * graphTimeStep;
        float graphXStep = timeDuration / (count - 1);

        float timeMin = mGraphCurStartIndex * graphTimeStep;
        float timeMax = timeMin + timeDuration;

        //populate graphs
        int pointCount = Mathf.Min(count, tracer.points.Count - mGraphCurStartIndex);

        var startPt = tracer.points[0].position;

        //get min/max
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        for(int i = 0; i < pointCount; i++) {
            var pt = tracer.points[mGraphCurStartIndex + i];

            var pos = pt.position - startPt;

            if(pos.x < min.x)
                min.x = pos.x;
            if(pos.x > max.x)
                max.x = pos.x;

            if(pos.y < min.y)
                min.y = pos.y;
            if(pos.y > max.y)
                max.y = pos.y;
        }

        //x graph
        graphXPosition.Setup(timeMin, timeMax, min.x, max.x);

        for(int i = 0; i < pointCount; i++) {
            var pt = tracer.points[mGraphCurStartIndex + i];

            var pos = pt.position - startPt;

            graphXPosition.Plot(timeMin + graphXStep * i, pos.x);
        }

        /*public GraphLineWidget graphPosition;
    public GraphBarWidget graphVelocity;
    public GraphBarWidget graphAccel;*/
    }

    void OnGraphTimeSlider(float val) {
        int ind = Mathf.RoundToInt(val);
        if(mGraphCurStartIndex != ind) {
            mGraphCurStartIndex = ind;
            UpdateGraph();
        }
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
