using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Basic calculator
/// </summary>
public class ModalCalculator : M8.UIModal.Controller, M8.UIModal.Interface.IPush {
    public const string parmInitValue = "val";
    public const string parmMaxDigit = "d";

    public enum InputType {
        Invalid,

        Numeric,

        //operators        
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public enum ValueMod {
        Square,
        SquareRoot,
        Invert,
        Negate,
        Cos,
        Sin,
        Tan
    }

    public struct InputData {        
        public InputType type;
        public string displayText;
        public double value;

        public bool isOperator {
            get {
                return IsOperator(type);
            }
        }
    }

    [Header("Data")]
    [SerializeField]
    int _defaultMaxDigits = 16;
        
    [Header("Display")]
    public Text inputLabel;
    public Text numericLabel;

    [Header("Signals")]
    public SignalFloat signalProceed;

    private double mCurValue;
    private bool mCurValueIsSpecial; //when we click on constants such as PI
    private int mMaxDigits;

    private StringBuilder mCurInput = new StringBuilder();
    private List<InputData> mInputs = new List<InputData>();
    
    public void Clear() {
        mInputs.Clear();
        UpdateInputDisplay();

        ClearEntry();
    }

    public void ClearEntry() {
        //if only one input, clear that as well
        if(mInputs.Count == 1) {
            mInputs.Clear();
            UpdateInputDisplay();
        }

        SetCurrentValue(0);
    }

    public void Erase() {
        if(mCurInput.Length > 0) {
            //reset to zero if single digit with negative, or if it's just single digit
            if((mCurInput.Length == 2 && mCurInput[0] == '-') || mCurInput.Length == 1) {
                mCurInput.Remove(0, mCurInput.Length);
                mCurInput.Append('0');
            }
            else
                mCurInput.Remove(mCurInput.Length - 1, 1);
        }

        UpdateCurrentValueFromInput();
    }

    public void Input(int num) {
        //don't add if we are already at limit
        int count = mCurInput.Length;
        if(count > 0) {
            if(mCurInput[0] == '-')
                count--;
            if(CurrentInputGetPeriodIndex() != -1)
                count--;

            if(count >= mMaxDigits)
                return;
        }

        mCurInput.Append(num);
        UpdateCurrentValueFromInput();
    }

    public void Square() {
        ApplyMod(ValueMod.Square);
    }

    public void SquareRoot() {
        ApplyMod(ValueMod.SquareRoot);
    }

    public void Invert() {
        ApplyMod(ValueMod.Invert);
    }

    public void Negate() {
        //ignore if value is 0
        if(mCurValue == 0) {
            return;
        }
        if(mCurInput.Length > 0) {
            //just invert current input
            if(mCurInput[0] == '-')
                mCurInput.Remove(0, 1);
            else
                mCurInput.Insert(0, '-');

            UpdateCurrentValueFromInput();
        }
        else {
            ApplyMod(ValueMod.Negate);
        }
    }

    public void Period() {
        if(mCurInput.Length == 0) {
            mCurInput.Append("0.");
            UpdateCurrentValueFromInput();
        }
        else if(CurrentInputGetPeriodIndex() == -1) {
            mCurInput.Append('.');
            UpdateCurrentValueFromInput();
        }
    }

    public void Divide() {
        AddOperator(InputType.Divide);
    }

    public void Multiply() {
        AddOperator(InputType.Multiply);
    }

    public void Add() {
        AddOperator(InputType.Add);
    }

    public void Subtract() {
        AddOperator(InputType.Subtract);
    }

    public void Equal() {
        EvaluateInputsToCurrentValue();

        mInputs.Clear();
        UpdateInputDisplay();
    }

    public void PI() {
        SetCurrentValue(System.Math.PI);
        mCurValueIsSpecial = true;
    }

    public void Cos() {
        ApplyMod(ValueMod.Cos);
    }

    public void Sin() {
        ApplyMod(ValueMod.Sin);
    }

    public void Tan() {
        ApplyMod(ValueMod.Tan);
    }

    public void Proceed() {
        Close();

        if(signalProceed)
            signalProceed.Invoke((float)mCurValue);
    }

    //void Awake() {
    //}

    void M8.UIModal.Interface.IPush.Push(M8.GenericParams parms) {
        double val = 0;

        if(parms != null) {
            if(parms.ContainsKey(parmInitValue)) {
                object obj = parms.GetValue<object>(parmInitValue);
                if(obj is float)
                    val = (float)obj;
                else if(obj is double)
                    val = (double)obj;
                else
                    val = 0;
            }

            if(parms.ContainsKey(parmMaxDigit)) {
                mMaxDigits = parms.GetValue<int>(parmMaxDigit);
                if(mMaxDigits <= 0)
                    mMaxDigits = _defaultMaxDigits;
            }
            else
                mMaxDigits = _defaultMaxDigits;
        }

        SetCurrentValue(val);

        ClearInput();
    }

    private static double EvaluateInputType(InputType type, double lhs, double rhs) {
        switch(type) {
            case InputType.Add:
                return lhs + rhs;
            case InputType.Subtract:
                return lhs - rhs;
            case InputType.Multiply:
                return lhs * rhs;
            case InputType.Divide:
                return lhs / rhs;
            default:
                return lhs;
        }
    }

    private static bool IsOperator(InputType type) {
        return type != InputType.Invalid && type != InputType.Numeric;
    }

    private void ApplyMod(ValueMod mod) {
        var inputLastInd = mInputs.Count - 1;

        double val;
        if(inputLastInd != -1 && mInputs[inputLastInd].type == InputType.Numeric)
            val = mInputs[inputLastInd].value;
        else
            val = mCurValue;

        string strFormat;
        double newVal;
                
        switch(mod) {
            case ValueMod.Square:
                strFormat = "({0})²";
                newVal = val * val;
                break;
            case ValueMod.SquareRoot:
                strFormat = "√({0})";
                newVal = System.Math.Sqrt(val);
                break;
            case ValueMod.Invert:
                strFormat = "1/({0})";
                newVal = 1.0 / val;
                break;
            case ValueMod.Negate:
                strFormat = "-({0})";
                newVal = -val;
                break;
            case ValueMod.Cos:
                strFormat = "cos({0})";
                newVal = System.Math.Cos(val * (System.Math.PI / 180.0));
                break;
            case ValueMod.Sin:
                strFormat = "sin({0})";
                newVal = System.Math.Sin(val * (System.Math.PI / 180.0));
                break;
            case ValueMod.Tan:
                strFormat = "tan({0})";
                newVal = System.Math.Tan(val * (System.Math.PI / 180.0));
                break;
            default:
                strFormat = "{0}";
                newVal = 0f;
                break;
        }

        //check for last value from input and modify it
        if(inputLastInd != -1 && mInputs[inputLastInd].type == InputType.Numeric) {
            //encapsulate value
            var inputVal = mInputs[inputLastInd];
            ModifyInput(inputLastInd, InputType.Numeric, string.Format(strFormat, inputVal.displayText), newVal);
        }
        else {
            //apply to current input and add to input
            AddInput(InputType.Numeric, string.Format(strFormat, mCurValue), newVal);
        }

        //clear input and apply value
        SetCurrentValue(newVal);
    }

    private void AddOperator(InputType type) {
        string opText;

        switch(type) {
            case InputType.Add:
                opText = "+";
                break;
            case InputType.Subtract:
                opText = "-";
                break;
            case InputType.Multiply:
                opText = "x";
                break;
            case InputType.Divide:
                opText = "/";
                break;
            default:
                opText = "";
                break;
        }

        int inputLastIndex = mInputs.Count - 1;

        //input empty?
        if(inputLastIndex < 0) {
            //add current input and operator
            AddInput(InputType.Numeric, mCurValue.ToString(), mCurValue);

            AddInput(type, opText, 0);

            mCurInput.Remove(0, mCurInput.Length);
        }
        else {
            if(mInputs[inputLastIndex].type == InputType.Numeric) {
                //add operator
                AddInput(type, opText, 0);
            }
            //check if current input is filled
            else if(mCurInput.Length == 0 && !mCurValueIsSpecial) {
                //replace last operator
                if(mInputs[inputLastIndex].isOperator)
                    ModifyInput(inputLastIndex, type, opText, 0);
            }
            else {
                //add current input, evaluate, and then add operator
                AddInput(InputType.Numeric, mCurValue.ToString(), mCurValue);
                
                //evaluate value
                EvaluateInputsToCurrentValue();

                AddInput(type, opText, 0);
            }
        }
    }
    
    private void EvaluateInputsToCurrentValue() {
        if(mInputs.Count < 2)
            return;

        double evalValue = 0;
        InputType lastType = InputType.Invalid;

        for(int i = 0; i < mInputs.Count; i++) {
            var inp = mInputs[i];

            if(!inp.isOperator) {
                //value?
                if(lastType == InputType.Invalid) //just set current value
                    evalValue = inp.value;
                else if(inp.type == InputType.Numeric) {
                    //apply last operator and update curVal
                    evalValue = EvaluateInputType(lastType, evalValue, inp.value);
                }
            }

            lastType = inp.type;
        }

        //operate on current value
        if(IsOperator(lastType))
            evalValue = EvaluateInputType(lastType, evalValue, mCurValue);

        SetCurrentValue(evalValue);
    }

    private void ModifyInput(int ind, InputType type, string displayText, double val) {
        mInputs[ind] = new InputData() { type = type, displayText = displayText, value = val };
        UpdateInputDisplay();
    }

    private void AddInput(InputType type, string displayText, double val) {
        mInputs.Add(new InputData() { type = type, displayText = displayText, value = val });
        UpdateInputDisplay();
    }

    private int GetLastInputNumericIndex() {
        for(int i = mInputs.Count - 1; i >= 0; i--) {
            if(mInputs[i].type == InputType.Numeric) {
                return i;
            }
        }

        return -1;
    }

    private int CurrentInputGetPeriodIndex() {
        for(int i = mCurInput.Length - 1; i >= 0; i--) {
            if(mCurInput[i] == '.')
                return i;
        }
        return -1;
    }

    private void ClearInput() {
        mInputs.Clear();
        UpdateInputDisplay();
    }

    private void UpdateInputDisplay() {
        if(inputLabel) {
            if(mInputs.Count > 0) {
                var sb = new StringBuilder();
                for(int i = 0; i < mInputs.Count; i++) {
                    sb.Append(mInputs[i].displayText);

                    if(i < mInputs.Count - 1)
                        sb.Append(' ');
                }

                inputLabel.text = sb.ToString();
            }
            else
                inputLabel.text = "";
        }
    }

    private void SetCurrentValue(double val) {
        mCurInput.Remove(0, mCurInput.Length);
        mCurValue = val;
        mCurValueIsSpecial = false;
        UpdateCurrentInputDisplay();
    }
    
    private void UpdateCurrentValueFromInput() {
        int startInd = 0;
        int len = mCurInput.Length;

        if(len > 0) {
            double parseVal;
            if(double.TryParse(mCurInput.ToString(startInd, len), out parseVal))
                mCurValue = parseVal;
        }
        else
            mCurValue = 0;

        mCurValueIsSpecial = false;

        //update display
        UpdateCurrentInputDisplay();
    }

    private void UpdateCurrentInputDisplay() {
        if(!numericLabel)
            return;

        if(mCurInput.Length > 0) {
            numericLabel.text = mCurInput.ToString();
        }
        else {
            if(System.Math.Abs(mCurValue) <= Mathf.Epsilon)
                numericLabel.text = "0";
            else
                numericLabel.text = mCurValue.ToString();
        }
    }
}
