using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TracerGraphControl : MonoBehaviour {
    public enum TelemetryType {
        Position,
        Velocity,
        Acceleration
    }

    public enum AxisType {
        X,
        Y
    }

    [System.Serializable]
    public struct TelemetryData {
        [M8.Localize]
        public string textRef;
        public string textFormat;

        [Header("UI")]
        public Text label;
        public Image highlightImage;

        [Header("Graphs")]
        public GraphLineWidget xAxisGraph;
        public GraphLineWidget yAxisGraph;

        public void ApplyAxisText(AxisType axis) {
            label.text = string.Format(textFormat, M8.Localize.Get(textRef), GetAxisString(axis));
        }

        public void Hide() {
            xAxisGraph.gameObject.SetActive(false);
            yAxisGraph.gameObject.SetActive(false);
        }

        public void SetAxisActive(AxisType axis) {
            xAxisGraph.gameObject.SetActive(axis == AxisType.X);
            yAxisGraph.gameObject.SetActive(axis == AxisType.Y);
        }

        private string GetAxisString(AxisType axis) {
            switch(axis) {
                case AxisType.X:
                    return "X";
                case AxisType.Y:
                    return "Y";
                default:
                    return "";
            } 
        }
    }

    public TracerRigidbody2D tracer;

    public GameObject graphGO;

    [Header("Telemetry")]
    public TelemetryData[] telemetries; //index via TelemetryType
    public Color telemetryActiveColor;
    public Color telemetryInactiveColor;
    public GameObject[] axisGOs;

    private TelemetryType mCurTelemetryType = TelemetryType.Position;
    private AxisType mCurAxis = AxisType.X;

    public void ShowGraph() {
        graphGO.SetActive(true);

        //default to X-axis
        ShowGraphX();
    }

    public void SetTelemetry(int telemetryTypeIndex) {
        var telemetryType = (TelemetryType)telemetryTypeIndex;

        if(mCurTelemetryType != telemetryType) {
            telemetries[(int)mCurTelemetryType].highlightImage.color = telemetryInactiveColor;
            telemetries[(int)mCurTelemetryType].Hide();

            telemetries[telemetryTypeIndex].highlightImage.color = telemetryActiveColor;
            telemetries[telemetryTypeIndex].SetAxisActive(mCurAxis);

            mCurTelemetryType = telemetryType;
        }
    }

    public void ShowGraphX() {
        if(mCurAxis != AxisType.X) {
            for(int i = 0; i < telemetries.Length; i++)
                telemetries[i].ApplyAxisText(AxisType.X);

            telemetries[(int)mCurTelemetryType].SetAxisActive(AxisType.X);

            mCurAxis = AxisType.X;

            for(int i = 0; i < axisGOs.Length; i++)
                axisGOs[i].SetActive(i == (int)mCurAxis);
        }
    }

    public void ShowGraphY() {
        if(mCurAxis != AxisType.Y) {
            for(int i = 0; i < telemetries.Length; i++)
                telemetries[i].ApplyAxisText(AxisType.Y);

            telemetries[(int)mCurTelemetryType].SetAxisActive(AxisType.Y);

            mCurAxis = AxisType.Y;

            for(int i = 0; i < axisGOs.Length; i++)
                axisGOs[i].SetActive(i == (int)mCurAxis);
        }
    }

    public void GraphPopulate() {
        if(tracer.points.Count == 0)
            return;

        UpdateGraph();
    }

    void Awake() {
        graphGO.SetActive(false);

        for(int i = 0; i < telemetries.Length; i++) {
            telemetries[i].ApplyAxisText(mCurAxis);

            if(i == (int)mCurTelemetryType) {
                telemetries[i].highlightImage.color = telemetryActiveColor;
                telemetries[i].SetAxisActive(mCurAxis);
            }
            else {
                telemetries[i].highlightImage.color = telemetryInactiveColor;
                telemetries[i].Hide();
            }
        }

        for(int i = 0; i < axisGOs.Length; i++)
            axisGOs[i].SetActive(i == (int)mCurAxis);

        //graphTimeSlider.onValueChanged.AddListener(OnGraphTimeSlider);
    }

    private void UpdateGraph() {
        if(tracer.points.Count == 0)
            return;

        int count = telemetries[0].xAxisGraph.barHorizontalCount;

        float timeDuration = tracer.duration;
        float timeStep = timeDuration / (count - 1);

        //populate graphs
        var startPt = tracer.points[0].position;

        //graphs
        var minPos = tracer.minPosition - startPt;
        var maxPos = tracer.maxPosition - startPt;

        //x-axis setup
        if(minPos.x != maxPos.x)
            telemetries[(int)TelemetryType.Position].xAxisGraph.Setup(0f, timeDuration, minPos.x, maxPos.x);
        else
            telemetries[(int)TelemetryType.Position].xAxisGraph.Setup(0f, timeDuration, minPos.x, minPos.x + 1.0f);

        if(tracer.minVelocity.x != tracer.maxVelocity.x)
            telemetries[(int)TelemetryType.Velocity].xAxisGraph.Setup(0f, timeDuration, tracer.minVelocity.x <= 0f ? tracer.minVelocity.x : 0f, tracer.maxVelocity.x);
        else
            telemetries[(int)TelemetryType.Velocity].xAxisGraph.Setup(0f, timeDuration, tracer.minVelocity.x <= 0f ? tracer.minVelocity.x : 0f, tracer.maxVelocity.x + 1.0f);

        if(tracer.minAccelApprox.x != tracer.maxAccelApprox.x)
            telemetries[(int)TelemetryType.Acceleration].xAxisGraph.Setup(0f, timeDuration, tracer.minAccelApprox.x <= 0f ? tracer.minAccelApprox.x : 0f, tracer.maxAccelApprox.x);
        else
            telemetries[(int)TelemetryType.Acceleration].xAxisGraph.Setup(0f, timeDuration, tracer.minAccelApprox.x <= 0f ? tracer.minAccelApprox.x : 0f, tracer.maxAccelApprox.x + 1.0f);

        //y-axis setup
        if(minPos.y != maxPos.y)
            telemetries[(int)TelemetryType.Position].yAxisGraph.Setup(0f, timeDuration, minPos.y, maxPos.y);
        else
            telemetries[(int)TelemetryType.Position].yAxisGraph.Setup(0f, timeDuration, minPos.y, minPos.y + 1.0f);

        if(tracer.minVelocity.y != tracer.maxVelocity.y)
            telemetries[(int)TelemetryType.Velocity].yAxisGraph.Setup(0f, timeDuration, tracer.minVelocity.y <= 0f ? tracer.minVelocity.y : 0f, tracer.maxVelocity.y);
        else
            telemetries[(int)TelemetryType.Velocity].yAxisGraph.Setup(0f, timeDuration, tracer.minVelocity.y <= 0f ? tracer.minVelocity.y : 0f, tracer.maxVelocity.y + 1.0f);

        if(tracer.minAccelApprox.y != tracer.maxAccelApprox.y)
            telemetries[(int)TelemetryType.Acceleration].yAxisGraph.Setup(0f, timeDuration, tracer.minAccelApprox.y <= 0f ? tracer.minAccelApprox.y : 0f, tracer.maxAccelApprox.y);
        else
            telemetries[(int)TelemetryType.Acceleration].yAxisGraph.Setup(0f, timeDuration, tracer.minAccelApprox.y <= 0f ? tracer.minAccelApprox.y : 0f, tracer.maxAccelApprox.y + 1.0f);

        //plot points

        for(int i = 0; i < count; i++) {
            float t = timeStep * i;

            var ptDat = tracer.GetPointData(t);

            var pos = ptDat.position - startPt;
            var vel = ptDat.velocity;
            var accel = ptDat.accelApprox;

            telemetries[(int)TelemetryType.Position].xAxisGraph.Plot(t, pos.x);
            telemetries[(int)TelemetryType.Velocity].xAxisGraph.Plot(t, vel.x);
            telemetries[(int)TelemetryType.Acceleration].xAxisGraph.Plot(t, accel.x);

            telemetries[(int)TelemetryType.Position].yAxisGraph.Plot(t, pos.y);
            telemetries[(int)TelemetryType.Velocity].yAxisGraph.Plot(t, vel.y);
            telemetries[(int)TelemetryType.Acceleration].yAxisGraph.Plot(t, accel.y);
        }
    }
}
