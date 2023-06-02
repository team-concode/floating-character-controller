using UnityEngine;

public class Actor : MonoBehaviour {
    [SerializeField] private FloatingCharacterController controller;
    
    private void Update() {
        var forward = GetForward();

        if (forward != Vector3.zero) {
            forward.Normalize();
            controller.SetForward(forward);
        } else {
            controller.SetForward(Vector3.zero);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            controller.StartJump();
        } else if (Input.GetKeyUp(KeyCode.Space)) {
            controller.StopJump();
        }
    }

    private Vector3 GetForward() {
        var forward = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) {
            forward.x -= 1f;
        }

        if (Input.GetKey(KeyCode.D)) {
            forward.x += 1f;
        }

        if (Input.GetKey(KeyCode.S)) {
            forward.z -= 1f;
        }

        if (Input.GetKey(KeyCode.W)) {
            forward.z += 1f;
        }

        return forward;
    }
}
