using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEntitySpawner : MonoBehaviour {
    [Header("Entity Data")]
    public GameObject template;
    public string poolGroup;
    public int startCapacity;
    public int maxCapacity;

    [Header("Spawn Data")]
    public Transform spawnPoint;

    private M8.PoolController mPool;

    public M8.EntityBase Spawn(Vector2 dir, float force) {
        M8.EntityBase ret = null;

        var parms = new M8.GenericParams();
        parms[UnitEntity.parmPosition] = spawnPoint.position;
        parms[UnitStateForceApply.parmDir] = dir;
        parms[UnitStateForceApply.parmForce] = force;

        ret = mPool.Spawn<M8.EntityBase>(template.name, "", null, parms);

        return ret;
    }

    void Awake() {
        mPool = M8.PoolController.CreatePool(poolGroup);
        mPool.AddType(template, startCapacity, maxCapacity);
    }
}
