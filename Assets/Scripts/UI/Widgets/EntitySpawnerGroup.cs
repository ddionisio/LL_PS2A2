using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawnerGroup : MonoBehaviour {
    [System.Serializable]
    public struct Data {
        public GameObject prefab;
        public int count;
    }

    [Header("Data")]
    public EntitySpawnerWidget template; //make sure this is in the root to duplicate (not a prefab!)
    public Data[] items;

    [Header("Drag Cursors")]
    public DragCursorWidget cursorUI;
    public DragCursorWorld cursorWorld;

    public EntitySpawnerWidget[] widgets { get; private set; }

    void Awake() {
        var root = template.transform.parent;
        root.gameObject.SetActive(false); //don't allow awake and enable to occur when instantiating items

        widgets = new EntitySpawnerWidget[items.Length];

        for(int i = 0; i < items.Length; i++) {
            var item = items[i];

            var displayInfo = item.prefab.GetComponent<UnitUIDisplayInfo>();

            var inst = Instantiate(template, root);

            inst.name = item.prefab.name;

            inst.iconSpriteUI = displayInfo.uiIcon;
            inst.iconSpriteWorld = displayInfo.uiWorldIcon;

            inst.cursorUI = cursorUI;
            inst.cursorWorld = cursorWorld;
            inst.entityTemplate = item.prefab;
            inst.entityCount = item.count;

            inst.initOnEnable = false;
            inst.Init();

            widgets[i] = inst;
        }

        template.gameObject.SetActive(false);

        root.gameObject.SetActive(true);
                
        cursorUI.gameObject.SetActive(false);
        cursorWorld.gameObject.SetActive(false);
    }
}
