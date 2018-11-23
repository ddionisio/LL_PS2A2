using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalVictoryForce : M8.UIModal.Controller, M8.UIModal.Interface.IPush, M8.UIModal.Interface.IPop {
    public const string parmTransferTraceGraph = "mvf_transfer";
    public const string parmIndex = "mvf_index";

    public Transform transferRoot;

    private TracerGraphControl mTransfer;
    private Transform mTransferLastParent;

    public void ShowTransfer() {
        if(mTransfer)
            mTransfer.ShowGraph();
    }

    void M8.UIModal.Interface.IPop.Pop() {
        if(mTransfer) {
            mTransfer.transform.parent = mTransferLastParent;
            mTransfer = null;
            mTransferLastParent = null;
        }
    }

    void M8.UIModal.Interface.IPush.Push(M8.GenericParams parms) {
        int index = 0;

        if(parms != null) {
            mTransfer = parms.GetValue<TracerGraphControl>(parmTransferTraceGraph);
            index = parms.GetValue<int>(parmIndex);
        }

        //apply index

        //apply transfer
        if(mTransfer) {
            mTransferLastParent = mTransfer.transform.parent;
            mTransfer.transform.SetParent(transferRoot, true);
        }
    }
}
