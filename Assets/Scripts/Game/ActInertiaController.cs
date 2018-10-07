using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActInertiaController : GameModeController<ActInertiaController> {
    [Header("Sequence")]
    public GameObject headerRootGO;

    public float startDelay = 1f;

    public ModalDialogController introDialog;

    public GameObject interfaceRootGO;

    public Rigidbody2DMoveController block1;

    [Header("Signals")]
    public SignalDragWidget signalUnitSpawnDragEnd;

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        headerRootGO.SetActive(false);

        interfaceRootGO.SetActive(false);

        block1.body.simulated = false;
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        headerRootGO.SetActive(true);

        yield return new WaitForSeconds(startDelay);

        //describe inertia
        introDialog.Play();
        while(introDialog.isPlaying)
            yield return null;

        //get block 1 ready
        block1.body.simulated = true;

        //wait for block 1 to hit ground
        while(!block1.isGrounded)
            yield return null;

        //show interaction
        interfaceRootGO.SetActive(true);

        //drag instruction

        //wait for block1 to contact goal


        //animation

        //more dialog

        //next level
    }
}
