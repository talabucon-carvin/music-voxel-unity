using UnityEngine;
using UnityEngine.InputSystem;

public class VoxelPlacer : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform worldParent;
    public Crosshair crosshair;
    public LineRenderer lineRenderer;

    [Header("Block Options")]
    public BlockData[] blockOptions;     // 0 = platform, 1 = piano instrument, 2 = pitch C, etc.
    public GameObject[] blockPrefabs;    // MUST match order
    private int selectedBlockIndex = 0;
    private BlockData blockToPlace;

    [Header("Placement Settings")]
    public float maxRayDistance = 10f;
    public LayerMask placementMask;

    void Update()
    {
        if (playerCamera == null || blockOptions.Length == 0 || blockPrefabs.Length == 0) return;

        HandleHotkeys();
        HandlePlacement();
    }

    void HandleHotkeys()
    {
        int maxKeys = Mathf.Min(10, blockOptions.Length);

        for (int i = 0; i < maxKeys; i++)
        {
            if (Keyboard.current[i + Key.Digit1].wasPressedThisFrame)
            {
                selectedBlockIndex = i;
                blockToPlace = blockOptions[selectedBlockIndex];
                Debug.Log("Selected block: " + blockToPlace.blockName);
            }
        }

        if (blockToPlace == null)
            blockToPlace = blockOptions[selectedBlockIndex];
    }

    void HandlePlacement()
    {
        Vector2 crossPos = crosshair != null
            ? crosshair.GetCrosshairScreenPos()
            : new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray ray = playerCamera.ScreenPointToRay(crossPos);
        RaycastHit hit;
        Vector3 endPoint = ray.origin + ray.direction * maxRayDistance;

        if (Physics.Raycast(ray, out hit, maxRayDistance, placementMask))
        {
            endPoint = hit.point;

            Vector3 spawnPos = hit.point + hit.normal * 0.5f;
            spawnPos = new Vector3(
                Mathf.Round(spawnPos.x),
                Mathf.Round(spawnPos.y),
                Mathf.Round(spawnPos.z)
            );

            Debug.DrawLine(spawnPos - Vector3.one * 0.5f, spawnPos + Vector3.one * 0.5f, Color.yellow);

            // PLACE BLOCK
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (!Physics.CheckBox(spawnPos, Vector3.one * 0.45f, Quaternion.identity, placementMask))
                {
                    GameObject prefabToSpawn = blockPrefabs[selectedBlockIndex];
                    GameObject newBlock = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, worldParent);

                    // Assign correct BlockData
                    Block blockComp = newBlock.GetComponent<Block>();
                    if (blockComp != null)
                        blockComp.data = blockToPlace;

                    // Apply material
                    if (blockToPlace.material != null)
                    {
                        MeshRenderer rend = newBlock.GetComponent<MeshRenderer>();
                        if (rend != null)
                            rend.material = blockToPlace.material;
                    }
                }
            }

            // DELETE BLOCK
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Block hitBlock = hit.collider.GetComponent<Block>();
                if (hitBlock != null)
                {
                    Destroy(hitBlock.gameObject);
                    return;
                }
            }
        }

        // LineRenderer (debug)
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, endPoint);
        }
    }
}
