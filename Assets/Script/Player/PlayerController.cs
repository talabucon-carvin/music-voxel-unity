using UnityEngine;
using UnityEngine.InputSystem; // Required for new Input System

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerNewInput : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded;

    private PlayerControls controls;
    private Vector2 moveInput;
    private bool jumpInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        // Move callback
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Jump callback
        controls.Player.Jump.performed += ctx => jumpInput = true;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void FixedUpdate()
    {
        // WASD / Left Stick movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 velocity = move * moveSpeed;
        velocity.y = rb.linearVelocity.y; // preserve vertical velocity
        rb.linearVelocity = velocity;

        // Jump
        if (jumpInput && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpInput = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Simple grounded check
        isGrounded = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }
}
