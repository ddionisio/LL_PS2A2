using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolProxy : MonoBehaviour {
    
    public void ReleaseGroup(string group) {
        var pool = M8.PoolController.GetPool(group);
        if(pool)
            pool.ReleaseAll();
    }
}
