using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCrossoutCounterWidget : MonoBehaviour, M8.UIModal.Interface.IPush, M8.UIModal.Interface.IActive {
    public const string parmTargetCount = "tgtcrossout_c";
    public const string parmTargetCrossCount = "tgtcrossout_cc";

    public TargetCrossoutCounterItemWidget template;
    public Transform root;
    public float autoIncrementDelay = 0.4f; //when used in modal

    private TargetCrossoutCounterItemWidget[] mItems;
    private int mCurCounterInd;
    private int mCount = 0;

    private int mAutoCrossCount = 0;

    public void Increment() {
        if(mItems == null || mCurCounterInd >= mCount)
            return;

        mItems[mCurCounterInd].crossGO.SetActive(true);

        mCurCounterInd++;
    }

    public void Setup(int count) {
        if(mItems != null) {
            //refresh
            for(int i = 0; i < mCount; i++) {
                mItems[i].gameObject.SetActive(true);
                mItems[i].crossGO.SetActive(false);
            }

            if(mItems.Length < count) {
                System.Array.Resize(ref mItems, count);

                //add new items
                for(int i = mCount; i < count; i++) {
                    mItems[i] = Instantiate(template);
                    mItems[i].transform.SetParent(root, false);
                    mItems[i].gameObject.SetActive(true);
                    mItems[i].crossGO.SetActive(false);
                }
            }
            else if(mItems.Length > count) {
                //turn off excess
                for(int i = count; i < mCount; i++)
                    mItems[i].gameObject.SetActive(false);
            }

            mCount = count;
        }
        else {
            //create everything
            mItems = new TargetCrossoutCounterItemWidget[count];

            for(int i = 0; i < count; i++) {
                mItems[i] = Instantiate(template);
                mItems[i].transform.SetParent(root, false);
                mItems[i].gameObject.SetActive(true);
                mItems[i].crossGO.SetActive(false);
            }

            mCount = count;
        }

        mCurCounterInd = 0;        
    }

    void OnEnable() {

    }

    void Awake() {
        template.gameObject.SetActive(false);
    }

    void M8.UIModal.Interface.IActive.SetActive(bool aActive) {
        if(aActive) {
            if(mAutoCrossCount > 0)
                StartCoroutine(DoAutoCross());
        }
        else {
            if(mItems != null) {
                for(int i = 0; i < mCount; i++)
                    mItems[i].crossGO.SetActive(false);
            }
        }
    }

    void M8.UIModal.Interface.IPush.Push(M8.GenericParams parms) {
        if(parms != null) {
            int count = parms.GetValue<int>(parmTargetCount);
            Setup(count);

            mAutoCrossCount = parms.GetValue<int>(parmTargetCrossCount);
        }
    }

    IEnumerator DoAutoCross() {
        var wait = new WaitForSeconds(autoIncrementDelay);

        for(int i = 0; i < mAutoCrossCount; i++) {
            yield return wait;

            Increment();
        }
    }
}
