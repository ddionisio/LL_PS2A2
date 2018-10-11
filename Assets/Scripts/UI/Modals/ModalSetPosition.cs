using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use to position a modal upon opening it.
/// </summary>
public class ModalSetPosition : MonoBehaviour, M8.UIModal.Interface.IPush {
    public const string parmScreenPoint = "setpt_s";
    public const string parmWorldPoint = "setpt_w";
    public const string parmAnchor = "setpt_a";

    [Header("Data")]
    public Transform root;
    public Transform anchorRoot; //hierarchy that includes all anchors if we want to use them as a point

    private Vector3 mDefaultPos;
    
    void Awake() {
        if(!root)
            root = transform;

        mDefaultPos = root.position;
    }

    void M8.UIModal.Interface.IPush.Push(M8.GenericParams parms) {
        Vector3 toPos = mDefaultPos;

        if(parms != null) {
            if(parms.ContainsKey(parmAnchor)) {
                if(anchorRoot) {
                    var anchorName = parms.GetValue<string>(parmAnchor);
                    if(!string.IsNullOrEmpty(anchorName)) {
                        for(int i = 0; i < anchorRoot.childCount; i++) {
                            var anchor = anchorRoot.GetChild(i);
                            if(anchor.name == anchorName) {
                                toPos = anchor.position;
                                break;
                            }
                        }
                    }
                }
            }
            else if(parms.ContainsKey(parmScreenPoint)) {
                //TODO: for now, we assume we are in screen space (Canvas is configured as screen space)
                var ptObj = parms.GetValue<object>(parmScreenPoint);
                if(ptObj is Vector2)
                    toPos = (Vector2)ptObj;
                else if(ptObj is Vector3)
                    toPos = (Vector2)(Vector3)ptObj;
            }
            else if(parms.ContainsKey(parmWorldPoint)) {
                var ptObj = parms.GetValue<object>(parmScreenPoint);
                if(ptObj is Vector2)
                    toPos = (Vector2)ptObj;
                else if(ptObj is Vector3)
                    toPos = (Vector2)(Vector3)ptObj;

                //convert to our space

                //TODO: for now, we assume we are in screen space (Canvas is configured as screen space)
                //TODO: uses camera.main for now                
                var cam = Camera.main;
                toPos = cam.WorldToScreenPoint(toPos);
                toPos.z = 0f;
            }
        }

        root.position = toPos;
    }
}
