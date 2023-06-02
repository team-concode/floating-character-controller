using UnityEngine;

/// <summary>
/// https://github.com/joebinns/stylised-character-controller
/// Acts to mirror a RigidBody's position and rotation.
/// This is intended to be used to avoid cases in which a rigidbody parent has a rigidbody child, avoiding strange results.
/// </summary>
public class RigidPlatform : MonoBehaviour {
    [SerializeField] private RigidParent rigidParent;

    public RigidParent GetParent() {
        return rigidParent;
    }
}