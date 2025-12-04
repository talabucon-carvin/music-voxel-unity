using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class RailSnapController : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;  // Reference to PlayerController
    public Transform player;                   // Usually the same transform
    public CharacterController controller;     // Player's CharacterController

    [Header("Rail Snap Settings")]
    public float railY = -0.83f;               // Height on the rail
    public float railMoveSpeed = 4f;           // Speed along the rail
    public float detectDistance = 0.4f;        // Raycast distance down for rail
    private int lastRailZ = int.MinValue;      // Track last Z scanned

    private RailBlock currentRail;
    public VoxelPlacer voxelPlacer;
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
                TrySnapToRail();
        }

        // Automatic scanning along Z
        int currentZ = Mathf.FloorToInt(player.position.z);
            
        Debug.Log("Player Z=" + player.position.z + " lastRailZ=" + lastRailZ);
        if (currentZ != lastRailZ)
        {
            Debug.Log("firing");

            lastRailZ = currentZ;
            AutoScanRow(currentZ);
        }
    }

    void TrySnapToRail()
    {
        if (Physics.Raycast(player.position + Vector3.up * 0.2f,
                            Vector3.down,
                            out RaycastHit hit,
                            detectDistance))
        {
            RailBlock rail = hit.collider.GetComponent<RailBlock>();
            if (rail != null)
                SnapToRail(rail, hit.point);
        }
    }

    void SnapToRail(RailBlock rail, Vector3 contactPoint)
    {
        currentRail = rail;
        playerController.railLocked = true;

        Vector3 snapPos = contactPoint;
        snapPos.y = railY;
        player.position = snapPos;

        // Align rotation to rail forward
        player.rotation = Quaternion.LookRotation(rail.forwardDir, Vector3.up);
    }

    void HandleSnappedMovement()
    {
        if (currentRail == null)
        {
            Unsnap();
            return;
        }

        float input = Keyboard.current.wKey.isPressed ? 1f :
                      Keyboard.current.sKey.isPressed ? -1f : 0f;

        if (input != 0)
        {
            Vector3 move = currentRail.forwardDir * input * railMoveSpeed * Time.deltaTime;
            controller.Move(new Vector3(move.x, 0, move.z)); // horizontal only
        }

        // Keep player on railY
        Vector3 pos = player.position;
        pos.y = railY;
        player.position = pos;

        // Reset vertical velocity to prevent bouncing
        playerController.ResetVerticalVelocity();
    }

    void Unsnap()
    {
        currentRail = null;
        playerController.railLocked = false;
    }

    private void AutoScanRow(int z)
    {
        if (voxelPlacer == null)
            voxelPlacer = FindObjectOfType<VoxelPlacer>();

        if (voxelPlacer == null)
            Debug.LogWarning("RailSnapController could not find a VoxelPlacer in the scene!");
        int minX = -20; // set to cover all vertical stacks
        int maxX = 20;

        for (int x = minX; x <= maxX; x++)
        {
            Debug.Log("Auto-scanning z=" + z + " x=" + x);

            // y doesn't matter; PlayColumnAt scans full column
            Vector3Int startPos = new Vector3Int(x, 0, z);
            voxelPlacer.PlayColumnAt(startPos);
            Debug.Log("Playing column at " + startPos);
        }
    }

}
