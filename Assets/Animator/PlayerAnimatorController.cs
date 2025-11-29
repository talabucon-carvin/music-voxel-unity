using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private PlayerController movementScript;

    [Header("Animator Settings")]
    public string speedParameter = "Speed";   // Animator parameter name
    public string jumpParameter = "IsJumping";
    public float speedDampTime = 0.1f;        // Smooth blending

    private bool jumpInput = false;

    void Awake()
    {
        // Auto-find components on the same GameObject
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator component not found on Player!");

        movementScript = GetComponent<PlayerController>();
        if (movementScript == null)
            Debug.LogError("ThirdPersonMovement component not found on Player!");
    }

    void Update()
    {
        if (animator == null || movementScript == null) return;

        float speed = movementScript.MoveInput.magnitude;
        bool isWalking = speed > 0.1f; // threshold for moving
        animator.SetBool("IsWalking", isWalking);

        // Jump can still be handled separately
        animator.SetBool("IsJumping", jumpInput);
    }

    // Call these from your Jump Input Action
    public void OnJumpPerformed()
    {
        jumpInput = true;
    }

    public void OnJumpCanceled()
    {
        jumpInput = false;
    }
}
