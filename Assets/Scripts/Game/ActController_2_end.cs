﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActController_2_end : GameModeController<ActController_2_end> {

    [Header("Sequence")]
    public float startWait = 1f;
    public ModalDialogController dialog;
    public float endWait = 1f;

    protected override void OnInstanceInit() {
        base.OnInstanceInit();
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        yield return new WaitForSeconds(startWait);

        dialog.Play();
        while(dialog.isPlaying)
            yield return null;

        yield return new WaitForSeconds(endWait);

        //next level
        GameData.instance.Progress();
    }
}
