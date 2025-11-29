using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;

    [Header("Animator Settings")]
    public string speedParameter = "Speed";   // name of the Animator parameter
    public float speedDampTime = 0.1f;        // smooth blending

    void Awake()
    {
        // Auto-find components on the same GameObject
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        if (animator == null)
            Debug.LogError("Animator component not found on Player!");
        if (characterController == null)
            Debug.LogError("CharacterController component not found on Player!");
    }

    void Update()
    {
        if (animator == null || characterController == null) return;

        // Horizontal speed
        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        float speed = horizontalVelocity.magnitude;

        // Update Animator parameter smoothly
        animator.SetFloat(speedParameter, speed, speedDampTime, Time.deltaTime);
    }
}
