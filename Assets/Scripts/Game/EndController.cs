using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndController : GameModeController<EndController> {

    protected override void OnInstanceInit() {
        base.OnInstanceInit();
    }

    protected override IEnumerator Start() {
        yield return base.Start();

        //wait a bit

        //show complete
    }
}
