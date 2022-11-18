using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashHelpWidget : MonoBehaviour {
    [M8.TagSelector]
    public string tagPlayer;
    public string gameFlag;

    public GameObject helperGO;
    public float helperShowDuration = 4f;

    public M8.Signal signalStop;

    void OnEnable() {
        helperGO.SetActive(false);
    }

    void OnDestroy() {
        signalStop.callback -= OnSignalStop;
    }

    void Awake() {
        signalStop.callback += OnSignalStop;
    }

    void OnSignalStop() {
        var flagVal = GameData.instance.GetFlag(gameFlag);
        if(flagVal == 0) {
            StopAllCoroutines();
            StartCoroutine(DoShow());

            //flag shown
            GameData.instance.SetFlag(gameFlag, 1);
        }
    }

    IEnumerator DoShow() {
        yield return new WaitForSeconds(0.4f);

        helperGO.SetActive(true);

        yield return new WaitForSeconds(helperShowDuration);

        helperGO.SetActive(false);
    }
}
