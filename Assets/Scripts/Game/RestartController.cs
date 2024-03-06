using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartController : M8.SingletonBehaviour<RestartController> {
    private Coroutine mRout;

    public void Restart() {
        if(mRout != null)
            return;

        mRout = StartCoroutine(DoRestart());
    }

    IEnumerator DoRestart() {
		M8.UIModal.Manager.instance.ModalCloseAll();

        while(M8.UIModal.Manager.instance.isBusy)
            yield return null;

		M8.SceneManager.instance.ResumeForced(); //in case we restarted while scene is paused

		M8.SceneManager.instance.Reload();

        mRout = null;
	}

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
}
