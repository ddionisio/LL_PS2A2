using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEntitySpawner : MonoBehaviour {
    [Header("Data")]
    public float angleDefault; //start from right
    public bool angleClockwise;

    public float forceDefault;

    [Header("Entity Data")]
    public GameObject template;
    public string poolGroup;
    public int startCapacity;
    public int maxCapacity;

    [Header("Spawn Data")]
    public Transform spawnPoint;

    public event System.Action<M8.EntityBase> spawnCallback;

    private M8.PoolController mPool;

    private Vector2 mDir;
    private bool mIsDirSet;

    private float mForce;
    private bool mIsForceSet;
        
    public void SetForce(float force) {
        mForce = force;

        mIsForceSet = true;
    }

    public void SetDirAngle(float angle) {
        var dir = Vector2.right;

        if(angleClockwise)
            mDir = M8.MathUtil.RotateAngle(dir, angle);
        else
            mDir = M8.MathUtil.RotateAngle(dir, -angle);

        mIsDirSet = true;
    }

    public void Spawn() {
        if(!mIsForceSet)
            SetForce(forceDefault);
        if(!mIsDirSet)
            SetDirAngle(angleDefault);

        var parms = new M8.GenericParams();
        parms[UnitEntity.parmPosition] = spawnPoint.position;
        parms[UnitStateForceApply.parmDir] = mDir;
        parms[UnitStateForceApply.parmForce] = mForce;

        var ent = mPool.Spawn<M8.EntityBase>(template.name, "", null, parms);

        if(spawnCallback != null)
            spawnCallback(ent);
    }

    void Awake() {
        mPool = M8.PoolController.CreatePool(poolGroup);
        mPool.AddType(template, startCapacity, maxCapacity);
    }
}
