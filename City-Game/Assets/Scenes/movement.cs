using System.Collections;
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

    public Transform cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // no tilt
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 🔹 Ground check
        canJump = Physics.OverlapSphere(jumpCheck.position, 0.25f, whatIsGround).Length > 0;
        if (canJump)
        {
            jumpsRemaining = setJumps;
            //StartCoroutine(jumpcooldown());
        }
        else
        {
            jumpsRemaining = 0;
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
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // 🔹 Get directions based on player rotation
        Vector3 forward = cam.forward;
        forward.y = 0;
        Vector3 right = cam.right;
        right.y = 0;

        // Keep movement flat (no vertical tilt influence)
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // 🔹 Combine input with rotation
        Vector3 inputDir = (forward * moveZ + right * moveX).normalized;

        float control = canJump ? 1f : airControlMultiplier;
        Vector3 targetVel = inputDir * moveSpeed * control;

        Vector3 currentXZ = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 velChange = targetVel - currentXZ;

        rb.AddForce(velChange, ForceMode.VelocityChange);
    }

    private IEnumerator jumpcooldown()
    {
        jumpsRemaining = setJumps;
        yield return new WaitForSeconds(2);

    }
}