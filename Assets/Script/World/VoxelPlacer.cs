using UnityEngine;

public class VoxelPlacer : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;          // Player camera
    public GameObject blockTemplate;     // Your generic Block prefab
    public BlockData blockToPlace;       // The type of block the player is placing
    public Transform worldParent;        // Usually the SimpleVoxelWorld.transform

    [Header("Placement Settings")]
    public float maxRayDistance = 10f;
    public KeyCode placeKey = KeyCode.Mouse0;

    void Update()
    {
        if (playerCamera == null || blockTemplate == null || blockToPlace == null) return;

        // Raycast from center of screen (crosshair)
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            // Draw red line to show the ray hitting a block
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            // Calculate spawn position based on hit normal, snapped to integer grid
            Vector3 spawnPos = hit.collider.transform.position + hit.normal;
            spawnPos = new Vector3(
                Mathf.Round(spawnPos.x),
                Mathf.Round(spawnPos.y),
                Mathf.Round(spawnPos.z)
            );

            // Place block on left click
            if (Input.GetKeyDown(placeKey))
            {
                // Prevent placing inside an existing block
                if (!Physics.CheckBox(spawnPos, Vector3.one * 0.45f))
                {
                    // Instantiate template
                    GameObject newBlock = Instantiate(blockTemplate, spawnPos, Quaternion.identity, worldParent);

                    // Assign the BlockData to this instance
                    Block blockComp = newBlock.GetComponent<Block>();
                    if (blockComp != null)
                        blockComp.data = blockToPlace;

                    // Apply material if defined in BlockData
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
            // Draw green line if ray hits nothing
            Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.green);
        }
    }
}
