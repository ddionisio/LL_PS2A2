using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this as a convenience to open dialog and store its values
/// </summary>
[System.Serializable]
public class ModalDialogParam {
    public string modal = "dialog";

    public Sprite portrait;

    [M8.Localize]
    public string nameTextRef;
    [M8.Localize]
    public string dialogTextRef;

    public void Open(System.Action nextCallback) {
        ModalDialog.OpenApplyPortrait(modal, portrait, nameTextRef, dialogTextRef, nextCallback);
    }

    public void Close() {
        if(M8.UIModal.Manager.instance.ModalIsInStack(modal))
            M8.UIModal.Manager.instance.ModalCloseUpTo(modal, true);
    }
}
