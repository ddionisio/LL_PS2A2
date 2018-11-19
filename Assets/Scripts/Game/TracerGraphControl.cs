using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TracerGraphControl : MonoBehaviour {
    public TracerRigidbody2D tracer;

    public GameObject graphGO;
    public Slider graphTimeSlider;
    [M8.Localize]
    public string graphTimeRangeTextRef;
    public Text graphTimeRangeLabel;
    public Text graphTimeMaxLabel;
    public float graphTimeStep = 0.1f;

    [Header("X-axis")]
    public GameObject[] graphXGOs;
    public GraphLineWidget graphXPosition;
    public GraphLineWidget graphXVelocity;
    public GraphLineWidget graphXAccel;

    [Header("Y-axis")]
    public GameObject[] graphYGOs;
    public GraphLineWidget graphYPosition;
    public GraphLineWidget graphYVelocity;
    public GraphLineWidget graphYAccel;

    private int mGraphCurStartIndex;
    private int mGraphTimeCount;
            
    public void ShowGraph() {
        graphGO.SetActive(true);

        //default to X-axis
        ShowGraphX();
    }

    public void ShowGraphX() {
        for(int i = 0; i < graphXGOs.Length; i++) {
            if(graphXGOs[i])
                graphXGOs[i].SetActive(true);
        }

        for(int i = 0; i < graphYGOs.Length; i++) {
            if(graphYGOs[i])
                graphYGOs[i].SetActive(false);
        }
    }

    public void ShowGraphY() {
        for(int i = 0; i < graphYGOs.Length; i++) {
            if(graphYGOs[i])
                graphYGOs[i].SetActive(true);
        }

        for(int i = 0; i < graphXGOs.Length; i++) {
            if(graphXGOs[i])
                graphXGOs[i].SetActive(false);
        }
    }

    public void GraphPopulate() {
        if(tracer.points.Count == 0)
            return;
        
        mGraphCurStartIndex = 0;

        UpdateGraph();

        //setup slider
        int count = graphXPosition.barHorizontalCount;
        float timeDuration = tracer.points.Count * graphTimeStep;

        if(tracer.points.Count > count) {
            graphTimeSlider.interactable = true;
            graphTimeSlider.minValue = 0f;
            graphTimeSlider.maxValue = tracer.points.Count - count;
            graphTimeMaxLabel.text = (graphTimeSlider.maxValue * graphTimeStep).ToString("0.0");
        }
        else {
            graphTimeSlider.interactable = false;
            graphTimeMaxLabel.text = timeDuration.ToString("0.0");
        }
                
        graphTimeSlider.value = 0f;
    }

    void Awake() {
        graphGO.SetActive(false);

        graphTimeSlider.onValueChanged.AddListener(OnGraphTimeSlider);
    }

    private void UpdateGraph() {
        if(tracer.points.Count == 0)
            return;

        int count = graphXPosition.barHorizontalCount;
        float timeDuration = count * graphTimeStep;
        float graphXStep = timeDuration / (count - 1);

        float timeMin = mGraphCurStartIndex * graphTimeStep;
        float timeMax = timeMin + timeDuration;

        //populate graphs
        int pointCount = Mathf.Min(count, tracer.points.Count - mGraphCurStartIndex);

        var startPt = tracer.points[0].position;

        //graphs
        var minPos = tracer.minPosition - startPt;
        var maxPos = tracer.maxPosition - startPt;

        graphTimeRangeLabel.text = string.Format(M8.Localize.Get(graphTimeRangeTextRef), timeMin, timeMax);

        //x-axis setup
        if(minPos.x != maxPos.x)
            graphXPosition.Setup(timeMin, timeMax, minPos.x, maxPos.x);
        else
            graphXPosition.Setup(timeMin, timeMax, minPos.x, minPos.x + 1.0f);

        if(tracer.minVelocity.x != tracer.maxVelocity.x)
            graphXVelocity.Setup(timeMin, timeMax, tracer.minVelocity.x <= 0f ? tracer.minVelocity.x : 0f, tracer.maxVelocity.x);
        else
            graphXVelocity.Setup(timeMin, timeMax, tracer.minVelocity.x <= 0f ? tracer.minVelocity.x : 0f, tracer.maxVelocity.x + 1.0f);

        if(tracer.minAccelApprox.x != tracer.maxAccelApprox.x)
            graphXAccel.Setup(timeMin, timeMax, tracer.minAccelApprox.x <= 0f ? tracer.minAccelApprox.x : 0f, tracer.maxAccelApprox.x);
        else
            graphXAccel.Setup(timeMin, timeMax, tracer.minAccelApprox.x <= 0f ? tracer.minAccelApprox.x : 0f, tracer.maxAccelApprox.x + 1.0f);

        //y-axis setup
        if(minPos.y != maxPos.y)
            graphYPosition.Setup(timeMin, timeMax, minPos.y, maxPos.y);
        else
            graphYPosition.Setup(timeMin, timeMax, minPos.y, minPos.y + 1.0f);

        if(tracer.minVelocity.y != tracer.maxVelocity.y)
            graphYVelocity.Setup(timeMin, timeMax, tracer.minVelocity.y <= 0f ? tracer.minVelocity.y : 0f, tracer.maxVelocity.y);
        else
            graphYVelocity.Setup(timeMin, timeMax, tracer.minVelocity.y <= 0f ? tracer.minVelocity.y : 0f, tracer.maxVelocity.y + 1.0f);

        if(tracer.minAccelApprox.y != tracer.maxAccelApprox.y)
            graphYAccel.Setup(timeMin, timeMax, tracer.minAccelApprox.y <= 0f ? tracer.minAccelApprox.y : 0f, tracer.maxAccelApprox.y);
        else
            graphYAccel.Setup(timeMin, timeMax, tracer.minAccelApprox.y <= 0f ? tracer.minAccelApprox.y : 0f, tracer.maxAccelApprox.y + 1.0f);

        for(int i = 0; i < pointCount; i++) {
            var pt = tracer.points[mGraphCurStartIndex + i];

            var pos = pt.position - startPt;
            var vel = pt.velocity;
            var accel = pt.accelApprox;

            float t = timeMin + graphXStep * i;

            graphXPosition.Plot(t, pos.x);
            graphXVelocity.Plot(t, vel.x);
            graphXAccel.Plot(t, accel.x);

            graphYPosition.Plot(t, pos.y);
            graphYVelocity.Plot(t, vel.y);
            graphYAccel.Plot(t, accel.y);
        }
    }

    void OnGraphTimeSlider(float val) {
        int ind = Mathf.RoundToInt(val);
        if(mGraphCurStartIndex != ind) {
            mGraphCurStartIndex = ind;
            UpdateGraph();
        }
    }
}
