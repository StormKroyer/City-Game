using UnityEngine;

public class movement : MonoBehaviour
{
    [Header("Stats")]
    public float jumpForce = 6f;        // jump height
    public int setJumps = 2;            // how many jumps
    private int jumpsRemaining;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;        // movement speed
    public float airControlMultiplier = 0.8f; // less control in air

    private bool canJump;

    [Header("Unity Stuff")]
    public Transform jumpCheck;         // empty object at feet
    public LayerMask whatIsGround;      // ground layer

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // no tilt
    }

    void Update()
    {
        // 🔹 Ground check
        canJump = Physics.OverlapSphere(jumpCheck.position, 0.25f, whatIsGround).Length > 0;
        if (canJump)
        {
            jumpsRemaining = setJumps;
        }

        // 🔹 Jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining >= 1)
        {
            Vector3 v = rb.linearVelocity;
            v.y = jumpForce;   // only override vertical velocity
            rb.linearVelocity = v;

            jumpsRemaining--;
        }
    }

    void FixedUpdate()
    {
        // 🔹 Movement
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;

        // Target velocity (XZ plane only)
        float control = canJump ? 1f : airControlMultiplier;
        Vector3 targetVel = inputDir * moveSpeed * control;

        // Current velocity (XZ only)
        Vector3 currentXZ = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // How much we need to change
        Vector3 velChange = targetVel - currentXZ;

        // Apply velocity change instantly (ignores mass)
        rb.AddForce(velChange, ForceMode.VelocityChange);
    }
}