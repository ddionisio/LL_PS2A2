using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class RestartConfirm : MonoBehaviour {
    public string modalConfirm = "confirm";
    [M8.Localize]
    public string titleRef;
    [M8.Localize]
    public string descRef;

    [Header("Speak")]
    public string speakGroup = "confirm";
    public float speakDelay = 0.3f;
    public bool speakTitle;
    public bool speakDesc = true;

    public void Invoke() {
        string title = !string.IsNullOrEmpty(titleRef) ? M8.Localize.Get(titleRef) : "";
        string desc = !string.IsNullOrEmpty(descRef) ? M8.Localize.Get(descRef) : "";

        M8.UIModal.Dialogs.ConfirmDialog.Open(modalConfirm, title, desc, OnConfirm);

        if(speakTitle || speakDesc)
            StartCoroutine(DoSpeak());
    }

    void OnConfirm(bool confirm) {
        if(confirm) {
            //M8.UIModal.Manager.instance.ModalCloseAll();
            //M8.SceneManager.instance.Reload();
            RestartController.instance.Restart();
        }
    }

    IEnumerator DoSpeak() {
        if(speakDelay > 0f)
            yield return new WaitForSeconds(speakDelay);

        if(speakTitle && !string.IsNullOrEmpty(titleRef))
            LoLManager.instance.SpeakTextQueue(titleRef, speakGroup, 0);

        if(speakDesc && !string.IsNullOrEmpty(descRef))
            LoLManager.instance.SpeakTextQueue(descRef, speakGroup, 1);
    }
}
