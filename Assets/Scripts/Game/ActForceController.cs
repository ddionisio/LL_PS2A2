﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is for Act 3 levels
/// </summary>
public class ActForceController : GameModeController<ActForceController> {
    [Header("Game")]
    public string musicPath;

    [M8.TagSelector]
    public string playerTag;
    public float playerAliveDuration = 8f;
    [M8.TagSelector]
    public string forceGroupUITag;
    [M8.TagSelector]
    public string playGroupUITag;

    [Header("Graph")]
    public Button graphButton;
    public TracerGraphControl graphControl;

    [Header("Victory")]
    public string victoryModal = "victory_force";
    public int victoryIndex;
    public float victoryDelay = 2f;

    [Header("Signals")]
    public M8.Signal signalGoal;
    public M8.Signal signalDeath;

    [Header("Force Drag Instructions")]
    public bool isForceDragInstructions;
    [M8.TagSelector]
    public string tagDragGuide;
    public EntitySpawnerGroup entitySpawnerGroup;
    public GameObject dragGuideGO;
    public GameObject dragEntityGuideGO;
    public float playGuideGOShowDelay = 3f;
    public GameObject playGuideGO;

    private GameObject mPlayerGO;
    private Rigidbody2D mPlayerBody;

    private GameObject mForceGroupUIGO;
    private GameObject mPlayGroupUIGO;

    private DragToGuideWidget mDragGuide;

    protected override void OnInstanceDeinit() {
        base.OnInstanceDeinit();

        signalGoal.callback -= OnSignalGoal;
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        mPlayerGO = GameObject.FindGameObjectWithTag(playerTag);
        mPlayerBody = mPlayerGO.GetComponent<Rigidbody2D>();
        mPlayerGO.SetActive(false);

        mForceGroupUIGO = GameObject.FindGameObjectWithTag(forceGroupUITag);
        mForceGroupUIGO.SetActive(false);

        mPlayGroupUIGO = GameObject.FindGameObjectWithTag(playGroupUITag);
        mPlayGroupUIGO.SetActive(false);

        graphButton.onClick.AddListener(OnShowGraph);
        graphButton.interactable = false;

        if(!string.IsNullOrEmpty(tagDragGuide)) {
            var go = GameObject.FindGameObjectWithTag(tagDragGuide);
            mDragGuide = go.GetComponent<DragToGuideWidget>();
        }

        if(dragGuideGO) dragGuideGO.SetActive(false);
        if(dragEntityGuideGO) dragEntityGuideGO.SetActive(false);
        if(playGuideGO) playGuideGO.SetActive(false);

        signalGoal.callback += OnSignalGoal;
    }

    protected override IEnumerator Start() {
        if(!string.IsNullOrEmpty(musicPath))
            LoLManager.instance.PlaySound(musicPath, true, true);

        yield return base.Start();

        //spawn player
        mPlayerGO.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        //

        //first time detail

        //activate force selection
        mForceGroupUIGO.SetActive(true);

        //first time - show drags
        if(isForceDragInstructions) {
            yield return new WaitForSeconds(0.4f);

            dragGuideGO.SetActive(true);

            var item = entitySpawnerGroup.widgets[0];

            Vector2 guideStart = item.transform.position;
            Vector2 guideDest = Camera.main.WorldToScreenPoint(dragGuideGO.transform.position);

            mDragGuide.Show(false, guideStart, guideDest);

            //wait for item to be spawned
            while(item.activeUnitCount == 0)
                yield return null;

            //hide guide, show adjust entity guide
            dragGuideGO.SetActive(false);
            mDragGuide.Hide();

            dragEntityGuideGO.SetActive(true);

            yield return new WaitForSeconds(playGuideGOShowDelay);

            playGuideGO.SetActive(true);
            mPlayGroupUIGO.SetActive(true);

            while(!mPlayerBody.simulated)
                yield return null;

            //hide guides
            dragEntityGuideGO.SetActive(false);
            playGuideGO.SetActive(false);
        }
        else {
            //activate play
            mPlayGroupUIGO.SetActive(true);
        }
        
        //player watcher
        StartCoroutine(DoPlayer());
    }

    protected virtual void OnShowGraph() {
        graphControl.ShowGraph();
    }

    void OnSignalGoal() {
        graphButton.gameObject.SetActive(false);

        StartCoroutine(DoVictory());
    }

    IEnumerator DoVictory() {
        yield return new WaitForSeconds(victoryDelay);

        M8.UIModal.Manager.instance.ModalCloseAll();

        //transfer graph control
        var parms = new M8.GenericParams();
        parms.Add(ModalVictoryForce.parmIndex, victoryIndex);
        parms.Add(ModalVictoryForce.parmTransferTraceGraph, graphControl);

        M8.UIModal.Manager.instance.ModalOpen(victoryModal, parms);
    }

    IEnumerator DoPlayer() {
        graphControl.tracer.body = mPlayerBody;

        while(true) {
            //wait for player to enable body
            while(!mPlayerBody.simulated)
                yield return null;

            graphButton.interactable = false;

            //start recording
            graphControl.tracer.Record();

            //player duration
            float lastTime = Time.time;

            do {
                float curTime = Time.time;
                if(curTime - lastTime >= playerAliveDuration) {
                    //player expired
                    signalDeath.Invoke();
                    break;
                }

                yield return null;
            } while(mPlayerBody.simulated);
                

            graphControl.tracer.Stop();
            graphControl.GraphPopulate();

            graphButton.interactable = true;
        }
    }
}
