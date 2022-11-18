using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintControl : MonoBehaviour {
    [M8.TagSelector]
    public string hintGroupTag;

    public Button button;

    public int showHintStopCount = 4; //show the button after certain stops

    public GameObject hintToolTipGO;
    public string hintShowFlag;

    [Header("Signals")]
    public M8.Signal signalPlay;
    public M8.Signal signalStop;
        
    private GameObject[] mHintGOs;
    private int mCurHintCount;

    private int mCurShowHintStopCount;

    void OnDisable() {
        signalPlay.callback -= OnSignalPlay;
        signalStop.callback -= OnSignalStop;
    }

    void OnEnable() {
        signalPlay.callback += OnSignalPlay;
        signalStop.callback += OnSignalStop;

        button.gameObject.SetActive(false);

        hintToolTipGO.SetActive(false);

        mCurHintCount = 0;
        mCurShowHintStopCount = 0;

        for(int i = 0; i < mHintGOs.Length; i++) {
            if(mHintGOs[i])
                mHintGOs[i].SetActive(false);
        }
    }

    void Awake() {
        var hintGroupGO = GameObject.FindGameObjectWithTag(hintGroupTag);
        if(hintGroupGO) {
            var hintGroupTrans = hintGroupGO.transform;

            var count = hintGroupTrans.childCount;

            mHintGOs = new GameObject[count];
            for(int i = 0; i < count; i++) {
                mHintGOs[i] = hintGroupTrans.GetChild(i).gameObject;
                mHintGOs[i].SetActive(false);
            }
        }
        else
            mHintGOs = new GameObject[0];

        button.onClick.AddListener(OnHintClick);
    }

    void OnHintClick() {
        int countAdd = Mathf.FloorToInt(mHintGOs.Length * 0.5f);
        mCurHintCount += countAdd;
        if(mCurHintCount >= mHintGOs.Length)
            mCurHintCount = mHintGOs.Length;

        for(int i = 0; i < mCurHintCount; i++) {
            if(mHintGOs[i])
                mHintGOs[i].SetActive(true);
        }

        hintToolTipGO.SetActive(false);

        button.interactable = false;
    }

    void OnSignalPlay() {
        if(button.gameObject.activeSelf)
            button.interactable = false;

        for(int i = 0; i < mCurHintCount; i++) {
            if(mHintGOs[i])
                mHintGOs[i].SetActive(false);
        }

        hintToolTipGO.SetActive(false);
    }

    void OnSignalStop() {
        if(button.gameObject.activeSelf) {
            button.interactable = true;
        }
        else {
            mCurShowHintStopCount++;
            if(mCurShowHintStopCount == showHintStopCount) {
                button.gameObject.SetActive(true);

                button.interactable = true;

                var flag = GameData.instance.GetFlag(hintShowFlag);
                if(flag == 0) {
                    hintToolTipGO.SetActive(true);

                    GameData.instance.SetFlag(hintShowFlag, 1);
                }
            }
        }
    }
}
