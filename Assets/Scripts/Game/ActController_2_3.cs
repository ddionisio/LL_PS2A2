using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActController_2_3 : GameModeController<ActController_2_3> {
    [Header("Gameplay")]
    public Transform enemyEnterPointsRoot;
    public UnitEntity boulderUnit;
    public Selectable playUI;

    [Header("Signals")]
    public M8.Signal signalPlay;

    [Header("Sequence")]
    public GameObject interactionGO;

    private M8.GenericParams mBoulderUnitParms = new M8.GenericParams();

    protected override void OnInstanceDeinit() {

        signalPlay.callback -= OnSignalPlay;

        base.OnInstanceDeinit();
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        //setup boulder stuff
        mBoulderUnitParms[UnitEntity.parmPosition] = (Vector2)boulderUnit.transform.position;
        mBoulderUnitParms[UnitEntity.parmNormal] = Vector2.up;

        boulderUnit.gameObject.SetActive(false);
        //

        interactionGO.SetActive(false);

        playUI.interactable = false;

        //signals
        signalPlay.callback += OnSignalPlay;
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //intro and such

        interactionGO.SetActive(true);

        ActivatePlay(false);
        SpawnNextEnemies();

        //show drag
    }

    public void ActivatePlay(bool playActive) {
        playUI.interactable = playActive;

        boulderUnit.gameObject.SetActive(true);
        boulderUnit.Spawn(mBoulderUnitParms);
    }

    //returns true if we are done
    public bool SpawnNextEnemies() {
        return true;
    }

    void OnSignalPlay() {
        //move enemy units
    }
}
