using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalSetTitle : MonoBehaviour, M8.UIModal.Interface.IActive, M8.UIModal.Interface.IPush {
    public const string parmTextRef = "settitle_tRef";
    public const string parmText = "settitle_t";

    [Header("Display")]
    public Text titleLabel;
    [M8.Localize]
    public string titleDefaultTextRef;

    [Header("Speak")]
    public bool speakOnActive;
    public string speakGroup;
    public int speakGroupIndex;

    private string mTextRef;

    void M8.UIModal.Interface.IActive.SetActive(bool aActive) {
        if(aActive) {
            if(speakOnActive) {
                if(!string.IsNullOrEmpty(mTextRef)) {
                    if(!string.IsNullOrEmpty(speakGroup))
                        LoLManager.instance.SpeakTextQueue(mTextRef, speakGroup, speakGroupIndex);
                    else
                        LoLManager.instance.SpeakText(mTextRef);
                }
            }
        }
    }

    void M8.UIModal.Interface.IPush.Push(M8.GenericParams parms) {
        mTextRef = titleDefaultTextRef;
        string text = "";

        if(parms != null) {
            if(parms.ContainsKey(parmText)) {
                mTextRef = null;
                text = parms.GetValue<string>(parmText);
            }
            else if(parms.ContainsKey(parmTextRef)) {
                mTextRef = parms.GetValue<string>(parmTextRef);
            }
        }

        if(titleLabel) {
            if(!string.IsNullOrEmpty(mTextRef))
                titleLabel.text = M8.Localize.Get(mTextRef);
            else
                titleLabel.text = text;
        }
    }
}
