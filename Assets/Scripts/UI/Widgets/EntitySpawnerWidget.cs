using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntitySpawnerWidget : DragWidget {
    [Header("Display")]
    public Text countLabel;

    [Header("Unit")]
    public string entityPoolGroup = "entityPool";
    public GameObject entityTemplate;
    public int entityCount = 10;

    public override bool active {
        get {
            return base.active && mActiveUnits != null && mActiveUnits.Count < entityCount;
        }
    }

    private M8.PoolController mPool;
    private M8.CacheList<M8.EntityBase> mActiveUnits;

    public override void Init() {
        base.Init();

        UpdateState();
    }

    protected override void DragEnd() {
        if(isDropValid) {
            if(curSpace == DragWidgetSpace.World) {
                Vector2 spawnPoint = cursorWorld.worldPoint;

                var spawn = mPool.Spawn<M8.EntityBase>(entityTemplate.name, "", null, null);

                spawn.releaseCallback += OnEntityRelease;
                spawn.transform.position = spawnPoint;
                
                mActiveUnits.Add(spawn);

                UpdateState();
            }
        }
    }

    protected override void Awake() {
        base.Awake();

        mPool = M8.PoolController.CreatePool(entityPoolGroup);
        mPool.AddType(entityTemplate, entityCount, entityCount);

        mActiveUnits = new M8.CacheList<M8.EntityBase>(entityCount);
    }

    void OnEntityRelease(M8.EntityBase ent) {
        ent.releaseCallback -= OnEntityRelease;

        mActiveUnits.Remove(ent);

        UpdateState();
    }

    private void UpdateState() {
        int availableCount = entityCount - mActiveUnits.Count;

        if(countLabel) countLabel.text = availableCount.ToString();

        ApplyCurDragEnabled();
    }
}
