using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LoLExt;

public class ModalVictoryForce : ModalVictory, M8.UIModal.Interface.IPush, M8.UIModal.Interface.IPop {
    public const string parmTransferTraceGraph = "mvf_transfer";
    public const string parmIndex = "mvf_index";

    [System.Serializable]
    public struct CountData {
        public M8.Animator.Animate animator;
        [M8.Animator.TakeSelector(animatorField = "animator")]
        public string takeFilled;
        [M8.Animator.TakeSelector(animatorField = "animator")]
        public string takeEmpty;
        [M8.Animator.TakeSelector(animatorField = "animator")]
        public string takeEnter;
    }

    public CountData[] counts;
    public Transform transferRoot;

    private TracerGraphControl mTransfer;
    private Transform mTransferLastParent;

    private int mIndex;

    public void ShowTransfer() {
        if(mTransfer)
            mTransfer.ShowGraph();
    }

    public override void SetActive(bool aActive) {
        base.SetActive(aActive);

        if(aActive) {
            if(mIndex < counts.Length)
                counts[mIndex].animator.Play(counts[mIndex].takeEnter);
        }
    }

    void M8.UIModal.Interface.IPop.Pop() {
        if(mTransfer) {
            mTransfer.transform.SetParent(mTransferLastParent, false);
            mTransfer = null;
            mTransferLastParent = null;
        }
    }

    void M8.UIModal.Interface.IPush.Push(M8.GenericParams parms) {
        mIndex = 0;

        if(parms != null) {
            mTransfer = parms.GetValue<TracerGraphControl>(parmTransferTraceGraph);
            mIndex = parms.GetValue<int>(parmIndex);
        }

        //apply index
        for(int i = 0; i < counts.Length; i++) {
            var count = counts[i];

            if(i < mIndex)
                count.animator.Play(count.takeFilled);
            else if(i >= mIndex)
                count.animator.Play(count.takeEmpty);
        }

        //apply transfer
        if(mTransfer) {
            mTransferLastParent = mTransfer.transform.parent;
            mTransfer.transform.SetParent(transferRoot, false);
            mTransfer.transform.localPosition = Vector3.zero;
            mTransfer.transform.localScale = Vector3.one;
        }
    }
}
