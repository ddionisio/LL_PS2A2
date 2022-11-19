using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class EndController : GameModeController<EndController> {
    public string musicPath;

    public float startDelay;

    public GameObject endGO;

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        endGO.SetActive(false);
    }

    protected override IEnumerator Start() {
        if(!string.IsNullOrEmpty(musicPath))
            LoLManager.instance.PlaySound(musicPath, true, true);

        yield return base.Start();

        //wait a bit
        yield return new WaitForSeconds(startDelay);

        //show complete
        endGO.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        var speakWait = new WaitForFixedUpdate();

        while(LoLManager.instance.isSpeakQueueActive)
            yield return speakWait;

        LoLManager.instance.Complete();
    }
}
