using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class ActController_1_1 : GameModeController<ActController_1_1> {
    [Header("Widgets")]
    [M8.TagSelector]
    public string tagDragGuide;
    public Transform dragGuideTo;

    [Header("Sequence")]
    public string musicPath;

    public float startDelay = 1f;

    public CameraShakeControl cameraShaker;

    public AnimatorEnterExit titleAnim;
    public AnimatorEnterExit motionIllustrationAnim;
    public ModalDialogController introDialog;
    public ModalDialogController kidnapDialog;
    public float gravityDialogWaitDelay = 0.6f;
    public ModalDialogController gravityDialog;
    public ModalDialogController landDialog;    
    public ModalDialogController goblinPushDialog;
    public ModalDialogController block1FinishDialog;
    public ModalDialogController block2Dialog;

    public GameObject interfaceRootGO;

    public DragWidget unitSpawnerWidget;

    public string blockLandSfxPath;

    public Rigidbody2DMoveController block1;
    public Transform block1StartPt;
    [M8.TagSelector]
    public string block1GoalTag;

    public WheelJoint2D[] block1Wheels;
    public float block1WheelsAirFreq = 10f;
    public float block1WheelsGroundFreq = 1000000f;
    public GameObject block1ForceNetGO;
    public GameObject block1ForceGravityGO;
    public GameObject block1ForceBalancedGO;
    public GameObject block1ForceUnbalancedGO;
    public GameObject block1ForceKnightGO;
    public GameObject block1ForceGoblinGO;
    public Transform block1NetForceDirRoot;
    public GameObject block1NetForceNoneGO;
    public M8.Animator.Animate block1LeftAnim;
    public M8.Animator.Animate block1RightAnim;
    public float block1VelocityXThreshold = 0.03f;

    public Rigidbody2DMoveController block2;
    public Transform block2StartPt;
    public GameObject block2ForcesGO;
    public GameObject block2ForceNetGO;
    public GameObject block2ForceGravityGO;
    public GameObject block2ForceBalancedGO;
    public GameObject block2ForceUnbalancedGO;
    public Transform block2NetForceDirRoot;
    public GameObject block2NetForceNoneGO;
    public M8.Animator.Animate block2LeftAnim;

    public GameObject victoryGO;
    public float victoryStartDelay = 0.5f;
    public float victoryDelay; //delay to show victory modal

    public string victoryModal = "victory_act_1_1";

    [Header("Princess")]
    public GameObject princessGO;
    public M8.Animator.Animate princessAnim;
    [M8.Animator.TakeSelector(animatorField = "princessAnim")]
    public string princessTakeEnter;
    [M8.Animator.TakeSelector(animatorField = "princessAnim")]
    public string princessTakeHelp;

    [Header("Goblin")]
    public GameObject goblinTemplate;
    public string goblinPool;
    public Transform[] goblinPts;

    [Header("Signals")]
    public M8.Signal signalUnitsClear;
    public SignalDragWidget signalUnitDragEnd;

    private DragToGuideWidget mDragGuide;
    private bool mIsDragGuideShown;
    
    private M8.PoolController mGoblinPoolCtrl;
    private M8.CacheList<UnitEntity> mGoblins;

    private Coroutine mBlockForceRout;

    protected override void OnInstanceDeinit() {
        signalUnitDragEnd.callback -= OnSignalUnitDragEnd;

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        mGoblinPoolCtrl = M8.PoolController.GetPool(goblinPool);
        mGoblinPoolCtrl.AddType(goblinTemplate, goblinPts.Length, goblinPts.Length);
        mGoblins = new M8.CacheList<UnitEntity>(goblinPts.Length);

        var dragGuideGO = GameObject.FindGameObjectWithTag(tagDragGuide);
        mDragGuide = dragGuideGO.GetComponent<DragToGuideWidget>();
        mDragGuide.Hide();

		titleAnim.gameObject.SetActive(false);
        motionIllustrationAnim.gameObject.SetActive(false);

        block1ForceNetGO.SetActive(false);
        block1ForceGravityGO.SetActive(false);
        block1ForceBalancedGO.SetActive(false);
        block1ForceUnbalancedGO.SetActive(false);
        block1ForceKnightGO.SetActive(false);
        block1ForceGoblinGO.SetActive(false);
        block1NetForceDirRoot.gameObject.SetActive(false);
        block1NetForceNoneGO.SetActive(false);

        block2ForcesGO.SetActive(false);
        block2ForceNetGO.SetActive(false);
        block2ForceGravityGO.SetActive(false);
        block2ForceBalancedGO.SetActive(false);
        block2ForceUnbalancedGO.SetActive(false);
        block2NetForceDirRoot.gameObject.SetActive(false);
        block2NetForceNoneGO.SetActive(false);

        interfaceRootGO.SetActive(false);

        princessGO.SetActive(false);

        victoryGO.SetActive(false);

        block1.gameObject.SetActive(false);
        block1.transform.position = block1StartPt.position;

        for(int i = 0; i < block1Wheels.Length; i++) {
            var suspension = block1Wheels[i].suspension;
            suspension.frequency = block1WheelsAirFreq;
            block1Wheels[i].suspension = suspension;
        }

        block2.gameObject.SetActive(false);
        block2.transform.position = block2StartPt.position;

        signalUnitDragEnd.callback += OnSignalUnitDragEnd;
    }

    protected override IEnumerator Start() {
		if(!string.IsNullOrEmpty(musicPath))
            LoLManager.instance.PlaySound(musicPath, true, true);

        yield return base.Start();

		titleAnim.gameObject.SetActive(true);
        yield return titleAnim.PlayEnterWait();

        yield return new WaitForSeconds(startDelay);

        motionIllustrationAnim.gameObject.SetActive(true);
        yield return motionIllustrationAnim.PlayEnterWait();

        introDialog.Play();
        while(introDialog.isPlaying)
            yield return null;

        yield return titleAnim.PlayExitWait();
        titleAnim.gameObject.SetActive(false);

        yield return motionIllustrationAnim.PlayExitWait();
        motionIllustrationAnim.gameObject.SetActive(false);

        //princess
        yield return DoPrincessDistress();

        //help
        kidnapDialog.Play();
        while(kidnapDialog.isPlaying)
            yield return null;

        //get block 1 ready
        block1.gameObject.SetActive(true);
        block1.body.simulated = true;

        block1ForceGravityGO.SetActive(true);

        mBlockForceRout = StartCoroutine(DoForceBalance(block1, block1ForceNetGO, block1ForceBalancedGO, block1ForceUnbalancedGO, block1NetForceDirRoot, block1NetForceNoneGO, block1LeftAnim, block1RightAnim));

        //wait a bit, then pause and describe the gravity
        yield return new WaitForSeconds(gravityDialogWaitDelay);

        M8.SceneManager.instance.Pause();

        gravityDialog.Play();
        while(gravityDialog.isPlaying)
            yield return null;

        M8.SceneManager.instance.Resume();

        //wait for block 1 to hit ground
        while(!block1.isGrounded)
            yield return null;
                
        //fancy camera shake
        cameraShaker.Shake();

        if(!string.IsNullOrEmpty(blockLandSfxPath))
            LoLManager.instance.PlaySound(blockLandSfxPath, false, false);

        yield return new WaitForSeconds(1f);
                
        for(int i = 0; i < block1Wheels.Length; i++) {
            var suspension = block1Wheels[i].suspension;
            suspension.frequency = block1WheelsGroundFreq;
            block1Wheels[i].suspension = suspension;
        }
        //

        yield return new WaitForSeconds(1f);

        block1ForceGravityGO.SetActive(false);

        //some more dialog
        landDialog.Play();
        while(landDialog.isPlaying)
            yield return null;

        //show interaction
        interfaceRootGO.SetActive(true);

        //set to only one knight
        var knightSpawner = (EntitySpawnerWidget)unitSpawnerWidget;
        var lastKnightCount = knightSpawner.entityCount;
        knightSpawner.SetEntityCount(1);

        //game ready
        unitSpawnerWidget.active = true;

        //drag instruction
        yield return new WaitForSeconds(0.35f);
        mDragGuide.Show(false, unitSpawnerWidget.icon.transform.position, dragGuideTo.position);

        while(knightSpawner.activeUnitCount == 0)
            yield return null;

        mDragGuide.Hide();
        //

        //wait for block1 to start moving

        //send 1 goblin
        StartCoroutine(DoGoblins());

        //wait for block contact
        while((block1.collisionFlags & Rigidbody2DMoveController.CollisionFlags.Sides) == Rigidbody2DMoveController.CollisionFlags.None)
            yield return null;

        //show knight force
        block1ForceKnightGO.SetActive(true);

        //wait for goblin force display
        while(!block1ForceGoblinGO.activeSelf)
            yield return null;

        //dialog
        goblinPushDialog.Play();
        while(goblinPushDialog.isPlaying)
            yield return null;

        //add the rest of knights counter
        knightSpawner.SetEntityCount(lastKnightCount);

        //show drag again
        mDragGuide.Show(false, unitSpawnerWidget.icon.transform.position, dragGuideTo.position);
        mIsDragGuideShown = true;

        //wait for block1 to contact goal
        bool isBlockFinish = false;

        while(!isBlockFinish) {
            var blockContacts = block1.collisionData;
            for(int i = 0; i < blockContacts.Count; i++) {
                var coll = blockContacts[i].collider;
                if(coll && coll.CompareTag(block1GoalTag)) {
                    isBlockFinish = true;
                    break;
                }
            }

            yield return null;
        }
                
        unitSpawnerWidget.active = false;

        //animation

        //clear out units
        signalUnitsClear.Invoke();

        block1ForceKnightGO.SetActive(false);
        block1ForceGoblinGO.SetActive(false);

        //more dialog
        yield return new WaitForSeconds(2f);

        //clear out block1 info
        StopCoroutine(mBlockForceRout);
        block1ForceNetGO.SetActive(false);
        block1ForceBalancedGO.SetActive(false);
        block1ForceUnbalancedGO.SetActive(false);
        block1NetForceDirRoot.gameObject.SetActive(false);
        block1NetForceNoneGO.SetActive(false);
        //

        block1FinishDialog.Play();
        while(block1FinishDialog.isPlaying)
            yield return null;
        //

        //block 2 ready
        block2.gameObject.SetActive(true);
        block2.body.simulated = true;

        block2ForceGravityGO.SetActive(true);

        mBlockForceRout = StartCoroutine(DoForceBalance(block2, block2ForceNetGO, block2ForceBalancedGO, block2ForceUnbalancedGO, block2NetForceDirRoot, block2NetForceNoneGO, block2LeftAnim, null));

        //wait for block 2 to hit ground
        while(!block2.isGrounded)
            yield return null;

        //fancy camera shake
        cameraShaker.Shake();

        if(!string.IsNullOrEmpty(blockLandSfxPath))
            LoLManager.instance.PlaySound(blockLandSfxPath, false, false);

        block2ForceGravityGO.SetActive(false);

        block2Dialog.Play();
        while(block2Dialog.isPlaying)
            yield return null;

        //game ready
        unitSpawnerWidget.active = true;

        //show drag again
        mDragGuide.Show(false, unitSpawnerWidget.icon.transform.position, dragGuideTo.position);
        mIsDragGuideShown = true;

        //wait for knight contact, then show forces
        while((block2.collisionFlags & Rigidbody2DMoveController.CollisionFlags.Sides) == Rigidbody2DMoveController.CollisionFlags.None)
            yield return null;

        block2ForcesGO.SetActive(true);
        //

        //wait for block2 to contact block 1
        isBlockFinish = false;

        while(!isBlockFinish) {
            var blockContacts = block2.collisionData;
            for(int i = 0; i < blockContacts.Count; i++) {
                var coll = blockContacts[i].collider;
                if(coll == block1.coll) {
                    isBlockFinish = true;
                    break;
                }
            }

            yield return null;
        }

        unitSpawnerWidget.active = false;

        //clear out units
        signalUnitsClear.Invoke();

        StopCoroutine(mBlockForceRout);
        block2ForcesGO.SetActive(false);
        block2ForceNetGO.SetActive(false);
        block2ForceBalancedGO.SetActive(false);
        block2ForceUnbalancedGO.SetActive(false);
        block2NetForceDirRoot.gameObject.SetActive(false);
        block2NetForceNoneGO.SetActive(false);

        //victory
        yield return new WaitForSeconds(victoryStartDelay);

        victoryGO.SetActive(true);

        yield return new WaitForSeconds(victoryDelay);

        //next level
        //GameData.instance.Progress();
        M8.UIModal.Manager.instance.ModalCloseAll();
        M8.UIModal.Manager.instance.ModalOpen(victoryModal);
    }

    IEnumerator DoPrincessDistress() {
        princessGO.SetActive(true);

        princessAnim.Play(princessTakeEnter);

        while(princessAnim.isPlaying)
            yield return null;

        princessAnim.Play(princessTakeHelp);
    }

    IEnumerator DoGoblins() {
        var knightSpawner = (EntitySpawnerWidget)unitSpawnerWidget;
        int lastKnightActiveCount = 0;

        for(int i = goblinPts.Length - 1; i >= 0; i--) {
            var t = goblinPts[i];

            var goblin = mGoblinPoolCtrl.Spawn<UnitEntity>(goblinTemplate.name, "", null, t.position, null);

            mGoblins.Add(goblin);

            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
        }

        bool isFirst = true;

        while(mGoblins.Count > 0) {
            var goblin = mGoblins.RemoveLast();

            while(!goblin.body.simulated) //wait for it to be physically active
                yield return null;

            //wait for knight to be spawned
            while(knightSpawner.activeUnitCount == lastKnightActiveCount)
                yield return null;

            lastKnightActiveCount = knightSpawner.activeUnitCount;

            //wait for block to be moving to the right
            while(block1.body.velocity.x <= block1VelocityXThreshold)
                yield return null;

            //wait a bit
            if(isFirst) {
                isFirst = false;
                yield return new WaitForSeconds(2f);
            }

            //move goblin
            goblin.bodyMoveCtrl.moveHorizontal = -1f;

            //activate goblin force display once it touches
            if(!block1ForceGoblinGO.activeSelf) {
                while((goblin.bodyMoveCtrl.collisionFlags & Rigidbody2DMoveController.CollisionFlags.Sides) == Rigidbody2DMoveController.CollisionFlags.None)
                    yield return null;

                block1ForceGoblinGO.SetActive(true);
            }
        }
    }

    IEnumerator DoForceBalance(Rigidbody2DMoveController block, 
        GameObject blockForceNetGO, GameObject blockForceBalancedGO, GameObject blockForceUnbalancedGO, 
        Transform blockNetForceDirRoot, GameObject blockNetForceNoneGO,
        M8.Animator.Animate leftForceAnim, M8.Animator.Animate rightForceAnim) {
        blockForceNetGO.SetActive(true);

        var waitFixed = new WaitForFixedUpdate();

        Vector2 lastPos = block.transform.position;
        Vector2 lastVel = Vector2.zero;

        const float interval = 0.1f;

        while(block.body.simulated) {
            //wait a bit
            float curTime = 0f;
            while(curTime < interval) {
                yield return waitFixed;
                curTime += Time.fixedDeltaTime;
            }

            Vector2 curPos = block.body.position;

            var curVel = curPos - lastPos;
            lastPos = curPos;
                        
            var accel = (curVel - lastVel) / interval;

            //round up
            accel.x = (float)System.Math.Round(accel.x, 4);
            accel.y = (float)System.Math.Round(accel.y, 4);

            lastVel = curVel;

            bool isVelChanged = accel != Vector2.zero;            

            if(isVelChanged) {
                blockForceBalancedGO.SetActive(false);
                blockForceUnbalancedGO.SetActive(true);

                blockNetForceDirRoot.gameObject.SetActive(true);
                blockNetForceDirRoot.up = accel.normalized;

                blockNetForceNoneGO.SetActive(false);

                if(accel.x > 0f) {
                    if(leftForceAnim) leftForceAnim.Play(0);
                    if(rightForceAnim) rightForceAnim.Stop();
                }
                else {
                    if(leftForceAnim) leftForceAnim.Stop();
                    if(rightForceAnim) rightForceAnim.Play(0);
                }
            }
            else {
                blockForceBalancedGO.SetActive(true);
                blockForceUnbalancedGO.SetActive(false);

                blockNetForceDirRoot.gameObject.SetActive(false);
                blockNetForceNoneGO.SetActive(true);

                if(leftForceAnim) leftForceAnim.Stop();
                if(rightForceAnim) rightForceAnim.Stop();
            }            
        }

        if(leftForceAnim) leftForceAnim.Stop();
        if(rightForceAnim) rightForceAnim.Stop();
    }

    void OnSignalUnitDragEnd(DragWidgetSignalInfo inf) {
        if(inf.dragWidget.isDropValid) {
            if(mIsDragGuideShown) {
                mDragGuide.Hide();
                mIsDragGuideShown = false;
            }
        }
    }
}
