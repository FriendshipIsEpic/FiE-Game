using UnityEngine;

public class TwilightMagicSphere : MonoBehaviour {
    private float angle;

    private void Start() {
    }

    private void Update() {
        angle += 60f * Time.deltaTime;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}
