using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingCharacterController : MonoBehaviour {
    [Header("Locomotion")]
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float acceleration = 100f;
    [SerializeField] private AnimationCurve accelerationFactor;
    [SerializeField] private float maxAccelForce = 75f;
    [SerializeField] private AnimationCurve maxAccelForceFactor;
    [SerializeField] private float gravity = 10f;

    [Header("Floating")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float floatingOffset;
    [SerializeField] private float needleLength = 0.5f;
    [SerializeField] private float rideSpringStrength = 1f;
    [SerializeField] private float rideSpringDamper = 1f;
    
    [Header("Jump")]
    [SerializeField] private float jumpForceFactor = 15f;
    [SerializeField] private float jumpFallGravityFactor = 5f;
    [SerializeField] private float coyoteTime = 0.25f;
    [SerializeField] private bool useJumpBuffering = true;
    [SerializeField] private float jumpBuffering = 0.125f;

    public enum JumpState {
        None,
        Launch,
        Rise,
        Fall,
        Landing,
    }

    private Rigidbody rb;
    private CapsuleCollider col;
    private Transform colTransform;
    private Vector3 forward; 
    private Vector3 velocity;
    private float rideHeight;
    private float raycastHeight;
    private float remainCoyoteTime;

    private bool grounded;
    private bool fastFall;
    private float lastJumpRequest;
    private JumpState jumpState = JumpState.None;
    
    private Transform bucket;
    private RigidPlatform currentPlatform;

    public GameObject lastHit { get; private set; }
    public Action<GameObject> onHit;
    public Action<JumpState> onJumpState;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        col = GetComponentInChildren<CapsuleCollider>();
        bucket = transform.parent;
        grounded = true;

        if (col) {
            colTransform = col.transform;
            rideHeight = colTransform.localPosition.y + col.center.y + floatingOffset;
            raycastHeight = rideHeight + needleLength;
        }
    }

    public void SetForward(Vector3 forward) {
        this.forward = forward;
    }
    
    public bool CanJump() {
        return grounded && jumpState == JumpState.None;
    }

    public float GetJumpBufferingTime() {
        return jumpBuffering;
    }
    
    public bool StartJump() {
        if (!CanJump()) {
            lastJumpRequest = Time.time;
            return false;
        }

        SetJumpState(JumpState.Launch);
        return true;
    }
    
    public void StopJump() {
        if (jumpState == JumpState.Rise) {
            fastFall = true;
        }
    }

    private void FixedUpdate() {
        if (col == null) {
            return;
        }

        Locomotion();
        Jump();
        Floating();
    }

    private void SetJumpState(JumpState state) {
        if (jumpState == state) {
            return;
        }
        
        if (state == JumpState.Launch) {
            fastFall = false;
        }

        jumpState = state;
        onJumpState?.Invoke(state);
        
        if (useJumpBuffering && state == JumpState.None) {
            var diff = Time.time - lastJumpRequest;
            if (diff <= jumpBuffering) {
                StartJump();
            }
        }        
    }

    private void Jump() {
        if (jumpState == JumpState.None) {
            return;
        }

        if (jumpState == JumpState.Launch) {
            var prev = rb.velocity;
            rb.velocity = new Vector3(prev.x, 0f, prev.z);
            rb.AddForce(Vector3.up * jumpForceFactor, ForceMode.Impulse);
            SetJumpState(JumpState.Rise);
            return;
        }

        if (jumpState is JumpState.Landing) {
            SetJumpState(JumpState.None);
            return;
        }

        var rv = rb.velocity;
        var g = new Vector3(0, -gravity, 0);
        if (rv.y > 0) {
            var factor = jumpFallGravityFactor * (fastFall ? 2 : 1);
            rb.AddForce(g * factor, ForceMode.Acceleration);
        } else {
            if (grounded) {
                SetJumpState(JumpState.Landing);
            } else {
                SetJumpState(JumpState.Fall);
                rb.AddForce(g * jumpFallGravityFactor, ForceMode.Acceleration);
            }
        }
    }

    private void Locomotion() {
        var dt = Time.fixedDeltaTime;
        var velDot = Vector3.Dot(forward, velocity.normalized);
        var accel = acceleration * accelerationFactor.Evaluate(velDot);

        var nextVel = forward * maxSpeed;
        velocity = Vector3.MoveTowards(velocity, nextVel, accel * dt);


        var cv = rb.velocity;
        if (!grounded) {
            cv.y = 0;
        } else if (jumpState != JumpState.None && jumpState != JumpState.Landing) {
            cv.y = 0;
        }

        var force = (velocity - cv) / dt;
        var maxAccel = maxAccelForce * maxAccelForceFactor.Evaluate(velDot);
        force = Vector3.ClampMagnitude(force, maxAccel);

        if (force == Vector3.zero) {
            return;
        }
        rb.AddForce(force, ForceMode.Acceleration);
    }
    
    private void Floating() {
        var dt = Time.fixedDeltaTime;
        var origin = colTransform.position;
        origin.y += col.center.y;

        var hit = Physics.Raycast(origin, Vector3.down, out var rayHit, raycastHeight, layerMask.value);
        RefreshPlatform(rayHit);
        
        if (hit) {
            if (!grounded) {
                grounded = true;
                remainCoyoteTime = coyoteTime;
            }

            SetHitObject(rayHit.transform.gameObject);
            if (jumpState != JumpState.None && jumpState != JumpState.Landing) {
                return;
            }

            var vel = rb.velocity;
            var rayDir = Vector3.down;
            var otherVel = Vector3.zero;
            var hitBody = rayHit.rigidbody;
            if (hitBody != null) {
                otherVel = hitBody.velocity;
            }

            var rayDirVel = Vector3.Dot(rayDir, vel);
            var otherDirVel = Vector3.Dot(rayDir, otherVel);
            var relVel = rayDirVel - otherDirVel;
            var x = rayHit.distance - rideHeight;

            var springForce = (x * rideSpringStrength) - (relVel * rideSpringDamper);
            rb.AddForce(rayDir * springForce, ForceMode.Acceleration);
        } else {
            if (remainCoyoteTime > 0) {
                remainCoyoteTime -= dt;
                return;
            }
            
            if (grounded) {
                grounded = false;
            }

            SetHitObject(null);
            if (jumpState != JumpState.None && jumpState != JumpState.Landing) {
                return;
            }

            var down = -gravity * jumpFallGravityFactor;
            rb.AddForce(new Vector3(0, down, 0), ForceMode.Acceleration);
        }
    }

    private void SetHitObject(GameObject go) {
        if (this.lastHit == go) {
            return;
        }
        
        this.lastHit = go;
        onHit?.Invoke(go);
    }

    private void RefreshPlatform(RaycastHit rayHit) {
        if (grounded && rayHit.transform != null) {
            var platform = rayHit.transform.GetComponent<RigidPlatform>();
            if (platform == null) {
                ClearPlatform();
                return;
            }
            
            if (platform != currentPlatform) {
                var parent = platform.GetParent();
                transform.SetParent(parent.transform);
                currentPlatform = platform;
            }
        } else {
            ClearPlatform();
        }
    }

    private void ClearPlatform() {
        if (currentPlatform == null) {
            return;
        }
        
        transform.SetParent(bucket);
        currentPlatform = null;
    }

    public void SetMaxSpeed(float speed) {
        maxSpeed = speed;
    }

    private void OnDrawGizmos() {
        if (colTransform == null) {
            return;
        }
        
        var pos = colTransform.position;
        pos.y += col.center.y;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + velocity);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + Vector3.down * rideHeight);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + Vector3.down * raycastHeight);
    }
}
