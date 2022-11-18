using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : GameModeController<IntroController> {
    [Header("Intro")]
    public ModalDialogController dialog1;

    [Header("Signals")]
    public M8.Signal signalShowNext;
    public M8.Signal signalNext;

    private bool mIsWaitNext;

    protected override void OnInstanceDeinit() {
        base.OnInstanceDeinit();

        signalNext.callback -= OnSignalNext;
    }

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        signalNext.callback += OnSignalNext;
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //sequence
        dialog1.Play();
        while(dialog1.isPlaying)
            yield return null;

        signalShowNext.Invoke();

        //wait for next to be pressed
        mIsWaitNext = true;
        while(mIsWaitNext)
            yield return null;

        //move to next
        GameData.instance.Progress();
    }

    void OnSignalNext() {
        mIsWaitNext = false;
    }
}
