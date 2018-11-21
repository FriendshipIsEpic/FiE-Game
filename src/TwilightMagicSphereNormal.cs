using UnityEngine;

public class TwilightMagicSphereNormal : MonoBehaviour {
    private float angle;

    private void Start() {
    }

    private void Update() {
        angle = Mathf.Repeat(angle + 0.9f * Time.deltaTime, 1f);

        gameObject.GetComponent<Renderer>().material.SetTextureOffset("_BumpMap", new Vector2(0 - angle, 1));
    }
}
