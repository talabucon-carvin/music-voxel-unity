using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public Transform cameraTransform;  // drag Main Camera here

    private Vector2 moveInput;
    private PlayerControls controls;
    public Vector2 MoveInput => moveInput;  // expose current move vector
    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Enable();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void Update()
    {
        // Convert input to world movement relative to camera
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
            // move player
            transform.position += move * speed * Time.deltaTime;
        }
    }
}
