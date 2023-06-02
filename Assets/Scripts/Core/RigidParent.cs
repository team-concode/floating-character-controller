using UnityEngine;

/// <summary>
/// https://github.com/joebinns/stylised-character-controller
/// Acts to mirror a RigidBody's position and rotation.
/// This is intended to be used to avoid cases in which a rigidbody parent has a rigidbody child, avoiding strange results.
/// </summary>
public class RigidParent : MonoBehaviour {
    public Rigidbody target;

    private Transform tf;
    private Transform targetTf;

    private void Start() {
        tf = transform;
        if (target == null) {
            return;
        }
        
        targetTf = target.transform;
        tf.position = targetTf.position;
        tf.rotation = targetTf.rotation;
    }

    private void FixedUpdate() {
        if (targetTf == null) {
            return;
        }
        
        tf.position = targetTf.position;
        tf.rotation = targetTf.rotation;
    }
}