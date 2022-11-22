using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverMoveToLastSibling : UIBehaviour, IPointerEnterHandler {
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        transform.SetAsLastSibling();
    }
}
