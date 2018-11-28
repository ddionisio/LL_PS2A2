using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndController : GameModeController<EndController> {
    public float startDelay;
    public GameObject endGO;

    protected override void OnInstanceInit() {
        base.OnInstanceInit();

        endGO.SetActive(false);
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //wait a bit
        yield return new WaitForSeconds(startDelay);

        //show complete
        endGO.SetActive(true);
    }
}
