using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class RailSnapController : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;  // Reference to your PlayerController
    public Transform player;                   // Usually the same transform
    public CharacterController controller;     // Player's CharacterController

    [Header("Rail Snap Settings")]
    public float railY = -0.83f;               // Exact Y position on rail
    public float railMoveSpeed = 4f;           // Speed along the rail
    public float detectDistance = 0.4f;        // How far down to check for rails

    private RailBlock currentRail;

    void Start()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (playerController.railLocked)
        {
            HandleSnappedMovement();

            // Unsnap with E or Jump
            if (Keyboard.current.eKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Unsnap();
            }
        }
        else
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                TrySnapToRail();
            }
        }
    }

    // -------------------------------------------
    //  Try snapping to rail below player
    // -------------------------------------------
    void TrySnapToRail()
    {
        if (Physics.Raycast(player.position + Vector3.up * 0.2f,
                            Vector3.down,
                            out RaycastHit hit,
                            detectDistance))
        {
            RailBlock rail = hit.collider.GetComponent<RailBlock>();
            if (rail != null)
            {
                SnapToRail(rail, hit.point);
            }
        }
    }

    // -------------------------------------------
    //  Snap player to rail
    // -------------------------------------------
    void SnapToRail(RailBlock rail, Vector3 contactPoint)
    {
        currentRail = rail;
        playerController.railLocked = true;

        // Snap to railY
        Vector3 snapPos = contactPoint;
        snapPos.y = railY;
        player.position = snapPos;

        // Align rotation to rail forward
        Quaternion facing = Quaternion.LookRotation(rail.forwardDir, Vector3.up);
        player.rotation = facing;
    }

    // -------------------------------------------
    //  Move while snapped
    // -------------------------------------------
    void HandleSnappedMovement()
    {
        if (currentRail == null)
        {
            Unsnap();
            return;
        }

        // Forward/back input
        float input = Keyboard.current.wKey.isPressed ? 1f :
                      Keyboard.current.sKey.isPressed ? -1f : 0f;

        if (input != 0)
        {
            Vector3 move = currentRail.forwardDir * input * railMoveSpeed * Time.deltaTime;
            controller.Move(new Vector3(move.x, 0, move.z)); // only horizontal
        }

        // Keep player at railY
        Vector3 pos = player.position;
        pos.y = railY;
        player.position = pos;

        // Reset vertical velocity to prevent bouncing
        playerController.ResetVerticalVelocity();
    }

    // -------------------------------------------
    //  Unsnap from rail
    // -------------------------------------------
    void Unsnap()
    {
        currentRail = null;
        playerController.railLocked = false;
    }
}
