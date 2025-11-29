using UnityEngine;
using UnityEngine.InputSystem;

public class VoxelPlacer : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public GameObject blockTemplate;
    public BlockData blockToPlace;
    public Transform worldParent;

    [Header("Crosshair Settings")]
    public Crosshair crosshair;          // your Crosshair script
    public LineRenderer lineRenderer;

    [Header("Placement Settings")]
    public float maxRayDistance = 10f;
    public LayerMask placementMask;      // Layer(s) to hit, ignore player layer

    void Update()
    {
        if (playerCamera == null || blockTemplate == null || blockToPlace == null) return;

        // Get crosshair screen position
        Vector2 crosshairScreenPos = crosshair != null
            ? crosshair.GetCrosshairScreenPos()
            : new Vector2(Screen.width / 2f, Screen.height / 2f);
        // Create ray from camera through crosshair
        Ray ray = playerCamera.ScreenPointToRay(crosshairScreenPos);
        RaycastHit hit;
        Vector3 endPoint = ray.origin + ray.direction * maxRayDistance;

        // Raycast using LayerMask
        if (Physics.Raycast(ray, out hit, maxRayDistance, placementMask))
        {
            endPoint = hit.point;

            // Optional debug log
            Debug.Log("Raycast hit: " + hit.collider.name);

            // Calculate block spawn position (snap to grid)
            Vector3 spawnPos = hit.point + hit.normal * 0.5f;
            spawnPos = new Vector3(
                Mathf.Round(spawnPos.x),
                Mathf.Round(spawnPos.y),
                Mathf.Round(spawnPos.z)
            );

            // Debug visualization in Scene view
            Debug.DrawLine(spawnPos - Vector3.one * 0.5f, spawnPos + Vector3.one * 0.5f, Color.yellow);
            Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.blue);

            // Place block on left click
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (!Physics.CheckBox(spawnPos, Vector3.one * 0.45f, Quaternion.identity, placementMask))
                {
                    GameObject newBlock = Instantiate(blockTemplate, spawnPos, Quaternion.identity, worldParent);

                    // Assign BlockData
                    Block blockComp = newBlock.GetComponent<Block>();
                    if (blockComp != null) blockComp.data = blockToPlace;

                    // Assign material
                    if (blockToPlace.material != null)
                    {
                        MeshRenderer renderer = newBlock.GetComponent<MeshRenderer>();
                        if (renderer != null) renderer.material = blockToPlace.material;
                    }
                }
            }
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                // Directly delete the hit object IF it is a block
                Block block = hit.collider.GetComponent<Block>();
                if (block != null)
                {
                    Destroy(block.gameObject);
                    return; // prevent placing & deleting at the same frame
                }
            }

        }

        // Update LineRenderer for Game view
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, endPoint);
        }
    }
}
