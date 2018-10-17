using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntitySpawnSetText : MonoBehaviour, M8.IPoolSpawn {
    public const string parmText = "entSetText_t";

    [Header("Display")]
    public Text label;

    void M8.IPoolSpawn.OnSpawned(M8.GenericParams parms) {
        if(parms != null) {
            object obj;
            if(parms.TryGetValue<object>(parmText, out obj))
                label.text = obj.ToString();
            else
                label.text = "";
        }
    }
}
