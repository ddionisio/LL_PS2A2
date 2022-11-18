using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActController_2_3 : GameModeController<ActController_2_3> {
    [System.Serializable]
    public struct SpawnInfo {
        public int[] pointIndices;
        public int speedMin;
        public int speedMax;
        public int accelMin;
        public int accelMax;
    }

    public struct SpawnData {
        public M8.EntityBase ent;
        public Vector2 spawnPt;
        public Vector2 enterPt;
    }

    [Header("Gameplay")]    
    public UnitEntity boulderUnit;
    public Selectable playUI;

    [Header("Spawn Info")]
    public GameObject spawnTemplate;
    public string spawnPoolGroup = "enemy";
    public Transform spawnEnterPointsRoot; //where to move to during spawning state
    public float spawnStartDistance = 14f;
    public float spawnMoveDelay;
    public DG.Tweening.Ease spawnMoveEase = DG.Tweening.Ease.OutSine;

    public SpawnInfo[] spawns;

    [Header("Signals")]
    public M8.Signal signalSpawnGoalReached;
    public M8.Signal signalProceed;

    [Header("Sequence")]
    public GameObject interactionGO;
    public GameObject proceedGO;

    private M8.GenericParams mBoulderUnitParms = new M8.GenericParams();

    private Vector2[] mEnterPoints;

    private int mCurSpawnInd = 0;

    private M8.PoolController mPool;
    private M8.CacheList<SpawnData> mSpawns;
    private M8.GenericParams mSpawnParm;

    private bool mIsProceedWait;

    private int mSpawnReachGoalCount;

    protected override void OnInstanceDeinit() {
        signalSpawnGoalReached.callback -= OnSignalSpawnReachGoal;
        signalProceed.callback -= OnSignalProceed;

        if(mSpawns != null) {
            for(int i = 0; i < mSpawns.Count; i++) {
                if(mSpawns[i].ent)
                    mSpawns[i].ent.releaseCallback -= OnSpawnReleased;
            }
        }

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        //spawn stuff
        int enterCount = spawnEnterPointsRoot.childCount;

        mPool = M8.PoolController.CreatePool(spawnPoolGroup);
        mPool.AddType(spawnTemplate, enterCount, enterCount);

        mEnterPoints = new Vector2[enterCount];
        for(int i = 0; i < mEnterPoints.Length; i++)
            mEnterPoints[i] = spawnEnterPointsRoot.GetChild(i).position;

        mSpawns = new M8.CacheList<SpawnData>(enterCount);

        mSpawnParm = new M8.GenericParams();
        mSpawnParm[UnitVelocityMoveController.parmDir] = Vector2.left;

        //setup boulder stuff
        mBoulderUnitParms[UnitEntity.parmPosition] = (Vector2)boulderUnit.transform.position;
        mBoulderUnitParms[UnitEntity.parmNormal] = Vector2.up;

        boulderUnit.gameObject.SetActive(false);
        //

        interactionGO.SetActive(false);
        proceedGO.SetActive(false);

        playUI.interactable = false;

        //signals
        signalSpawnGoalReached.callback += OnSignalSpawnReachGoal;
        signalProceed.callback += OnSignalProceed;
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro and such

        interactionGO.SetActive(true);

        ActivatePlay(false);

        mCurSpawnInd = 0;
        SpawnCurrentEnemies(true);

        //wait for enemies to enter
        yield return DoMoveSpawnsToEnter();

        //dialog
        
        //make sure the player gets it right
        bool proceed = false;
        while(!proceed) {
            //show drag

            playUI.interactable = true;

            //wait for spawns to be released
            while(mSpawns.Count > 0)
                yield return null;

            if(mSpawnReachGoalCount == 0)
                proceed = true;
            else {
                //dialog to try again

                ActivatePlay(true);
                SpawnCurrentEnemies(false);

                //wait for enemies to enter
                yield return DoMoveSpawnsToEnter();
            }
        }

        mCurSpawnInd = 1;

        //more dialog

        bool generateTelemetry = true;

        //go through the rest of the spawns
        while(mCurSpawnInd < spawns.Length) {            
            ActivatePlay(false);
            SpawnCurrentEnemies(generateTelemetry);

            //wait for enemies to enter
            yield return DoMoveSpawnsToEnter();

            playUI.interactable = true;

            //wait for spawns to be released
            while(mSpawns.Count > 0)
                yield return null;

            //check if all enemies reached goal
            if(mSpawnReachGoalCount == spawns[mCurSpawnInd].pointIndices.Length) {
                //show try again

                //restart
                generateTelemetry = false;
            }
            else { //go to next spawns
                generateTelemetry = true;
                mCurSpawnInd++;
            }
        }

        playUI.interactable = false;

        //complete

        //wait to proceed
        interactionGO.SetActive(false);
        proceedGO.SetActive(true);

        mIsProceedWait = true;
        while(mIsProceedWait)
            yield return null;

        GameData.instance.Progress();
    }

    IEnumerator DoMoveSpawnsToEnter() {
        var easeFunc = DG.Tweening.Core.Easing.EaseManager.ToEaseFunction(spawnMoveEase);

        var curTime = 0f;
        while(curTime < spawnMoveDelay) {
            yield return null;

            curTime += Time.deltaTime;

            var t = easeFunc(curTime, spawnMoveDelay, 0f, 0f);

            for(int i = 0; i < mSpawns.Count; i++) {
                var spawnDat = mSpawns[i];

                spawnDat.ent.transform.position = Vector2.Lerp(spawnDat.spawnPt, spawnDat.enterPt, t);
            }
        }
    }

    private void ActivatePlay(bool playActive) {
        playUI.interactable = playActive;

        boulderUnit.gameObject.SetActive(true);
        boulderUnit.Spawn(mBoulderUnitParms);
    }

    //returns true if we are done
    private void SpawnCurrentEnemies(bool generateTelemetry) {
        if(mCurSpawnInd == spawns.Length)
            return;

        var spawnDat = spawns[mCurSpawnInd];

        mSpawnReachGoalCount = 0;

        if(generateTelemetry) {
            mSpawnParm[UnitVelocityMoveController.parmSpeed] = (float)Random.Range(spawnDat.speedMin, spawnDat.speedMax + 1);
            mSpawnParm[UnitVelocityMoveController.parmAccel] = (float)Random.Range(spawnDat.accelMin, spawnDat.accelMax + 1);
        }

        int spawnCount = Mathf.Min(mEnterPoints.Length, spawnDat.pointIndices.Length);
        for(int i = 0; i < spawnCount; i++) {
            int spawnPtInd = Mathf.Clamp(spawnDat.pointIndices[i], 0, mEnterPoints.Length - 1); //fail-safe

            var enterPt = mEnterPoints[spawnPtInd];
            var spawnPt = new Vector2(enterPt.x + spawnStartDistance, enterPt.y);

            var ent = mPool.Spawn<M8.EntityBase>(spawnTemplate.name, "", null, spawnPt, mSpawnParm);

            ent.releaseCallback += OnSpawnReleased;

            mSpawns.Add(new SpawnData() { ent = ent, enterPt = enterPt, spawnPt = spawnPt });
        }
    }

    void OnSpawnReleased(M8.EntityBase ent) {
        for(int i = 0; i < mSpawns.Count; i++) {
            if(mSpawns[i].ent == ent) {
                ent.releaseCallback -= OnSpawnReleased;
                mSpawns.RemoveAt(i);

                //scoring

                break;
            }
        }
    }

    void OnSignalProceed() {
        mIsProceedWait = false;
    }

    void OnSignalSpawnReachGoal() {
        mSpawnReachGoalCount++;
    }
}
