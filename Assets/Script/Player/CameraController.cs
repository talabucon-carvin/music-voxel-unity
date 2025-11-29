using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
    [Header("References")]
    public Transform target;        // Player or target to follow
    public float distance = 4f;

    [Header("Settings")]
    public float sensitivity = 2f;
    public float minPitch = -20f;
    public float maxPitch = 70f;

    private float yaw;
    private float pitch;
    private Vector2 look;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Enable();

        controls.Player.Look.performed += ctx => look = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => look = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!target) return;

        yaw += look.x * sensitivity;
        pitch -= look.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = target.position - rotation * Vector3.forward * distance;
        transform.rotation = rotation;
    }
}
