using UnityEngine;
using UnityEngine.InputSystem;

public class VoxelPlacer : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;          // Player camera
    public GameObject blockTemplate;     // Generic cube prefab
    public BlockData blockToPlace;       // The type of block to place
    public Transform worldParent;        // Usually SimpleVoxelWorld.transform

    [Header("Placement Settings")]
    public float maxRayDistance = 10f;

    void Update()
    {
        if (playerCamera == null || blockTemplate == null || blockToPlace == null) return;

        // Raycast from center of screen (crosshair)
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            Vector3 spawnPos = hit.collider.transform.position + hit.normal;
            spawnPos = new Vector3(
                Mathf.Round(spawnPos.x),
                Mathf.Round(spawnPos.y),
                Mathf.Round(spawnPos.z)
            );

            // Place block when left mouse button is pressed (modern Input System)
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (!Physics.CheckBox(spawnPos, Vector3.one * 0.45f))
                {
                    GameObject newBlock = Instantiate(blockTemplate, spawnPos, Quaternion.identity, worldParent);

                    // Assign BlockData
                    Block blockComp = newBlock.GetComponent<Block>();
                    if (blockComp != null)
                        blockComp.data = blockToPlace;

                    // Assign material
                    if (blockToPlace.material != null)
                    {
                        MeshRenderer renderer = newBlock.GetComponent<MeshRenderer>();
                        if (renderer != null)
                            renderer.material = blockToPlace.material;
                    }
                }
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.green);
        }
    }
}
