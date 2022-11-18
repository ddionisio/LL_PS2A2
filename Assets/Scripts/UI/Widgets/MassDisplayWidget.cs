using UnityEngine;
using UnityEngine.UI;

public class MassDisplayWidget : MonoBehaviour {
    public Text label;
    public string format = "{0:N0} Kg";
    public Rigidbody2D body;

    void OnEnable() {
        label.text = string.Format(format, body.mass);
    }
}
