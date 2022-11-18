using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenModalSignal : MonoBehaviour {
    public string modal;
    public M8.Signal signal;
    public bool closeAllModals = true; //close other modals first before opening modal?

    void OnDestroy() {
        signal.callback -= OnSignal;
    }

    void Awake() {
        signal.callback += OnSignal;
    }

    void OnSignal() {
        if(closeAllModals)
            M8.UIModal.Manager.instance.ModalCloseAll();

        M8.UIModal.Manager.instance.ModalOpen(modal);
    }
}
