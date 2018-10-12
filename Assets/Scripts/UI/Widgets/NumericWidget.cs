using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Grab number from modal that provides numeric input, will then signal that number upon call to Next
/// </summary>
public class NumericWidget : MonoBehaviour {
    [Header("Data")]
    public string modal;
    public float initialValue;
    public bool isValueCapped;
    public float minValue;
    public float maxValue;
    public Transform inputAnchor;
    
    [Header("Display")]
    public Text numericLabel;
    public string numericFormat = "G16";

    [Header("Signals")]
    public SignalFloat signalNumericProcess; //listen to grab number
    public SignalFloat signalNumericNext; //invoked on calling Next

    private float mCurVal;
    private M8.GenericParams mModalParms = new M8.GenericParams();

    public void OpenNumericModal() {
        mModalParms[ModalCalculator.parmInitValue] = mCurVal;
        mModalParms[ModalSetPosition.parmScreenPoint] = (Vector2)inputAnchor.position;

        M8.UIModal.Manager.instance.ModalOpen(modal, mModalParms);
    }

    public void Next() {
        if(signalNumericNext)
            signalNumericNext.Invoke(mCurVal);
    }

    void OnDisable() {
        if(signalNumericProcess)
            signalNumericProcess.callback -= OnSignalNumericProcess;
    }

    void OnEnable() {
        mCurVal = initialValue;
        UpdateDisplay();

        if(signalNumericProcess)
            signalNumericProcess.callback += OnSignalNumericProcess;
    }

    void OnSignalNumericProcess(float val) {
        if(isValueCapped)
            mCurVal = Mathf.Clamp(val, minValue, maxValue);
        else
            mCurVal = val;

        UpdateDisplay();
    }

    private void UpdateDisplay() {
        if(numericLabel) numericLabel.text = mCurVal.ToString(numericFormat);
    }
}
