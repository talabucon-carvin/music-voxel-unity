using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public Transform cameraTransform;

    // Rail system lock
    public bool railLocked = false;
    public bool IsRailLocked => railLocked;

    private Vector2 moveInput;
    private PlayerControls controls;

    private CharacterController controller;
    private Vector3 velocity;  // tracks vertical velocity
    private bool isGrounded;

    public Vector2 MoveInput => moveInput;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Enable();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => Jump();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small downward force to stick to ground
        }

        // Movement
        if (!railLocked)
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = cameraTransform.right;
            right.y = 0;
            right.Normalize();

            Vector3 move = forward * moveInput.y + right * moveInput.x;

            if (move.sqrMagnitude > 0.01f)
            {
                // rotate player toward movement
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), 0.15f);
            }

            controller.Move(move * speed * Time.deltaTime);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        if (railLocked)
            return;

        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void ResetVerticalVelocity()
    {
        velocity.y = 0f;
    }

}
