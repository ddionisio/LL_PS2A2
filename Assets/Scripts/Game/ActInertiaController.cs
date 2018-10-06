using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActInertiaController : GameModeController<ActInertiaController> {
    [Header("Sequence")]
    public GameObject headerRootGO;

    public float startDelay = 1f;

    public ModalDialogController introDialog;

    public GameObject interfaceRootGO;

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        if(headerRootGO) headerRootGO.SetActive(false);
        if(interfaceRootGO) interfaceRootGO.SetActive(false);
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        if(headerRootGO) headerRootGO.SetActive(true);

        yield return new WaitForSeconds(startDelay);

        //describe inertia
        introDialog.Play();
        while(introDialog.isPlaying)
            yield return null;

        //show interaction
        if(interfaceRootGO) interfaceRootGO.SetActive(true);

        //wait for contraption to get to the goal

        //animation

        //more dialog

        //next level
    }
}
