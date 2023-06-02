using UnityEngine;

public class Platform : MonoBehaviour {
    [SerializeField] private Vector3 start;
    [SerializeField] private Vector3 end;
    [SerializeField] private float duration = 3f;

    private float times;

    private void Update() {
        times += Time.deltaTime;

        var iteration = Mathf.FloorToInt(times / duration);
        var from = start;
        var to = end;
        if (iteration % 2 == 1) {
            from = end;
            to = start;
        }

        var v = times % duration;
        transform.localPosition = Vector3.Lerp(from, to, v);
    }
}
