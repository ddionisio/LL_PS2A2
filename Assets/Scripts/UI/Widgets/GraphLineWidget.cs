using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphLineWidget : MonoBehaviour {
    public enum LineMode {
        Connect, //line between point
        Delta, //horizontal, then vertical
    }

    [Header("Data")]
    public int xDecimalRound = 1;
    public bool xDecimalIsRounded = false;
    public int yDecimalRound = 1;
    public bool yDecimalIsRounded = true;

    [Header("Templates")]
    public Transform dotTemplate;
    public int dotCapacity = 20;

    public RectTransform lineTemplate; //ensure pivot is at bottom. height is used as length, and up is oriented towards the end point
    public int lineCapacity = 20;
    public LineMode lineMode = LineMode.Connect;
        
    public string dotLabelYFormat = "{0}";
    public string dotLabelXFormat = "{0}";

    public Text labelHorizontalTemplate;
    public string labelHorizontalFormat = "{0}";
    public Text labelVerticalTemplate;    
    public string labelVerticalFormat = "{0}";

    public RectTransform gridHorizontalTemplate; //set this to appropriate anchor and pivot (x stretch). duplicated in its parent
    public RectTransform gridVerticalTemplate; //set this to appropriate anchor and pivot (y stretch). duplicated in its parent

    [Header("Display")]
    public RectTransform barHorizontal; //add labels here
    public int barHorizontalCount = 10;

    public RectTransform barVertical; //add labels here
    public int barVerticalCount = 10;
    
    public RectTransform lineArea;
    public RectTransform dotArea;
    
    private Text[] mLabelNumberHorz;
    private Text[] mLabelNumberVert;

    private struct DotData {
        public Transform t;
        public Text label;

        public DotData(Transform aT) {
            t = aT;
            label = t.GetComponentInChildren<Text>(true);
        }

        public void ApplyText(string text) {
            if(label) label.text = text;
        }
    }

    private M8.CacheList<DotData> mDots;
    private M8.CacheList<RectTransform> mLines;

    private M8.CacheList<DotData> mDotsCache;
    private M8.CacheList<RectTransform> mLinesCache;

    private float mXMin, mXMax;

    private float mYMin, mYMax;

    private bool mIsInit;

    public void Setup(float xMin, float xMax, float yMin, float yMax) {
        Init();

        Clear();

        //setup horizontal
        mXMin = xDecimalIsRounded ? (float)System.Math.Round(xMin, xDecimalRound) : xMin;
        mXMax = xDecimalIsRounded ? (float)System.Math.Round(xMax, xDecimalRound) : xMax;

        for(int i = 0; i < mLabelNumberHorz.Length; i++) {
            mLabelNumberHorz[i].text = string.Format(labelHorizontalFormat, Mathf.Lerp(mXMin, mXMax, (float)i / (mLabelNumberHorz.Length - 1)));
        }
                
        //setup vertical
        mYMin = yDecimalIsRounded ? (float)System.Math.Round(yMin, yDecimalRound) : yMin;
        mYMax = yDecimalIsRounded ? (float)System.Math.Round(yMax, yDecimalRound) : yMax;
        
        for(int i = 0; i < mLabelNumberVert.Length; i++) {
            mLabelNumberVert[i].text = string.Format(labelVerticalFormat, Mathf.Lerp(mYMin, mYMax, (float)i / (mLabelNumberVert.Length - 1)));
        }
    }

    public void Plot(float x, float y) {
        Init();

        if(mDots.IsFull)
            return;

        if(xDecimalIsRounded)
            x = (float)System.Math.Round(x, xDecimalRound);

        if(yDecimalIsRounded)
            y = (float)System.Math.Round(y, yDecimalRound);

        float xLen = Mathf.Abs(mXMax - mXMin);
        float nX = xLen != 0f ? (x - mXMin) / xLen : 0f;

        float yLen = Mathf.Abs(mYMax - mYMin);
        float nY = yLen != 0f ? (y - mYMin) / yLen : 0f;

        Transform lastDotTrans = mDots.Count > 0 ? mDots[mDots.Count - 1].t : null;

        //add dot
        var dotAreaSize = dotArea.rect.size;
        var dotTrans = mDotsCache.RemoveLast();

        // hover detail
        var sb = new System.Text.StringBuilder();
        if(!string.IsNullOrEmpty(dotLabelYFormat))
            sb.AppendFormat(dotLabelYFormat, y);
        if(!string.IsNullOrEmpty(dotLabelXFormat))
            sb.AppendLine().AppendFormat(dotLabelXFormat, x);

        dotTrans.ApplyText(sb.ToString());
        //

        dotTrans.t.localPosition = new Vector2(nX * dotAreaSize.x, nY * dotAreaSize.y);
        dotTrans.t.SetAsFirstSibling();
        dotTrans.t.gameObject.SetActive(true);
        mDots.Add(dotTrans);

        //add line
        if(!mLines.IsFull && lastDotTrans) {
            Vector2 dPos = dotTrans.t.localPosition - lastDotTrans.localPosition;
            float len = dPos.magnitude;
            if(len > 0f) {
                var dir = dPos / len;

                switch(lineMode) {
                    case LineMode.Connect:
                        var lineAreaSize = lineArea.rect.size;
                        var lineRectTrans = mLinesCache.RemoveLast();
                        lineRectTrans.localPosition = lastDotTrans.localPosition;
                        lineRectTrans.up = dir;

                        var lineSize = lineRectTrans.sizeDelta;
                        lineSize.y = len;
                        lineRectTrans.sizeDelta = lineSize;

                        lineRectTrans.gameObject.SetActive(true);
                        mLines.Add(lineRectTrans);
                        break;

                    case LineMode.Delta:
                        if(dPos.y == 0f) {
                            var lineRectTransX = mLinesCache.RemoveLast();
                            lineRectTransX.localPosition = lastDotTrans.localPosition;
                            lineRectTransX.up = new Vector2(1f, 0f);

                            var lineSizeX = lineRectTransX.sizeDelta;
                            lineSizeX.y = Mathf.Abs(dPos.x);
                            lineRectTransX.sizeDelta = lineSizeX;

                            lineRectTransX.gameObject.SetActive(true);
                            mLines.Add(lineRectTransX);
                        }
                        else if(dPos.y > 0f) {
                            //x
                            var lineRectTransX = mLinesCache.RemoveLast();
                            lineRectTransX.localPosition = lastDotTrans.localPosition;
                            lineRectTransX.up = new Vector2(1f, 0f);

                            var lineSizeX = lineRectTransX.sizeDelta;
                            lineSizeX.y = Mathf.Abs(dPos.x);
                            lineRectTransX.sizeDelta = lineSizeX;

                            lineRectTransX.gameObject.SetActive(true);
                            mLines.Add(lineRectTransX);

                            //y
                            var lineRectTransY = mLinesCache.RemoveLast();
                            lineRectTransY.localPosition = new Vector2(dotTrans.t.localPosition.x, lastDotTrans.localPosition.y);
                            lineRectTransY.up = new Vector2(0f, 1f);

                            var lineSizeY = lineRectTransY.sizeDelta;
                            lineSizeY.y = Mathf.Abs(dPos.y);
                            lineRectTransY.sizeDelta = lineSizeY;

                            lineRectTransY.gameObject.SetActive(true);
                            mLines.Add(lineRectTransY);
                        }
                        else {
                            //x
                            var lineRectTransX = mLinesCache.RemoveLast();
                            lineRectTransX.localPosition = new Vector2(lastDotTrans.localPosition.x, dotTrans.t.localPosition.y);
                            lineRectTransX.up = new Vector2(1f, 0f);

                            var lineSizeX = lineRectTransX.sizeDelta;
                            lineSizeX.y = Mathf.Abs(dPos.x);
                            lineRectTransX.sizeDelta = lineSizeX;

                            lineRectTransX.gameObject.SetActive(true);
                            mLines.Add(lineRectTransX);

                            //y
                            var lineRectTransY = mLinesCache.RemoveLast();
                            lineRectTransY.localPosition = lastDotTrans.localPosition;
                            lineRectTransY.up = new Vector2(0f, -1f);

                            var lineSizeY = lineRectTransY.sizeDelta;
                            lineSizeY.y = Mathf.Abs(dPos.y);
                            lineRectTransY.sizeDelta = lineSizeY;

                            lineRectTransY.gameObject.SetActive(true);
                            mLines.Add(lineRectTransY);
                        }
                        break;
                }
            }
        }
    }

    public void Clear() {
        if(mDots == null || mLines == null)
            return;

        for(int i = 0; i < mDots.Count; i++) {
            mDots[i].t.gameObject.SetActive(false);
            mDotsCache.Add(mDots[i]);
        }

        mDots.Clear();

        for(int i = 0; i < mLines.Count; i++) {
            mLines[i].gameObject.SetActive(false);
            mLinesCache.Add(mLines[i]);
        }

        mLines.Clear();
    }

    void Init() {
        if(mIsInit)
            return;
        
        //generate labels
        mLabelNumberHorz = new Text[barHorizontalCount];

        //horizontal
        float x = 0f;
        float xOfs = barHorizontal.rect.width / (barHorizontalCount - 1);

        for(int i = 0; i < barHorizontalCount; i++) {
            if(i > 0 && i < barHorizontalCount - 1) {
                var newGridVert = Instantiate(gridVerticalTemplate);
                newGridVert.SetParent(gridVerticalTemplate.parent);
                newGridVert.anchorMin = new Vector2(0f, 0f);
                newGridVert.anchorMax = new Vector2(0f, 1f);
                newGridVert.sizeDelta = new Vector2(1f, 0f);
                newGridVert.anchoredPosition = new Vector2(x, 0f);                
            }

            var newLabel = Instantiate(labelHorizontalTemplate);
            var newLabelRT = newLabel.rectTransform;

            newLabelRT.SetParent(barHorizontal);
            newLabelRT.anchoredPosition = new Vector2(x, 0f);

            mLabelNumberHorz[i] = newLabel;

            x += xOfs;
        }

        labelHorizontalTemplate.gameObject.SetActive(false);
        gridVerticalTemplate.gameObject.SetActive(false);
        //

        //vertical
        mLabelNumberVert = new Text[barVerticalCount];

        float y = 0f;
        float yOfs = barVertical.rect.height / (barVerticalCount - 1);

        for(int i = 0; i < barVerticalCount; i++) {
            if(i > 0 && i < barVerticalCount - 1) {
                var newGridHorz = Instantiate(gridHorizontalTemplate);
                newGridHorz.SetParent(gridHorizontalTemplate.parent);
                newGridHorz.anchorMin = new Vector2(0f, 0f);
                newGridHorz.anchorMax = new Vector2(1f, 0f);
                newGridHorz.sizeDelta = new Vector2(0f, 1f);
                newGridHorz.anchoredPosition = new Vector2(0f, y);
            }

            var newLabel = Instantiate(labelVerticalTemplate);
            var newLabelRT = newLabel.rectTransform;

            newLabelRT.SetParent(barVertical);
            newLabelRT.anchoredPosition = new Vector2(0f, y);

            mLabelNumberVert[i] = newLabel;

            y += yOfs;
        }

        labelVerticalTemplate.gameObject.SetActive(false);        
        gridHorizontalTemplate.gameObject.SetActive(false);
        //

        //setup caches
        mDots = new M8.CacheList<DotData>(dotCapacity);
        mDotsCache = new M8.CacheList<DotData>(dotCapacity);

        for(int i = 0; i < dotCapacity; i++) {
            var newDot = Instantiate(dotTemplate);
            newDot.SetParent(dotArea);

            var newDotDat = new DotData(newDot);

            newDot.gameObject.SetActive(false);

            mDotsCache.Add(newDotDat);
        }

        dotTemplate.gameObject.SetActive(false);

        mLines = new M8.CacheList<RectTransform>(lineCapacity);
        mLinesCache = new M8.CacheList<RectTransform>(lineCapacity);

        for(int i = 0; i < lineCapacity; i++) {
            var newLine = Instantiate(lineTemplate);
            newLine.SetParent(lineArea);
            newLine.gameObject.SetActive(false);
            mLines.Add(newLine);
        }

        lineTemplate.gameObject.SetActive(false);

        mIsInit = true;
    }
}
