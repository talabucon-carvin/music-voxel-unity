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
        // ` (BACKQUOTE) — PLATFORM
        if (Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            SelectBlock(0);
        }

        // 1–0 = 10 keys pitch C to pitch A
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectBlock(1);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectBlock(2);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectBlock(3);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectBlock(4);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectBlock(5);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SelectBlock(6);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SelectBlock(7);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) SelectBlock(8);
        if (Keyboard.current.digit9Key.wasPressedThisFrame) SelectBlock(9);
        if (Keyboard.current.digit0Key.wasPressedThisFrame) SelectBlock(10);

        // - (minus) and = (equals) pitch A# and pitch B
        if (Keyboard.current.minusKey.wasPressedThisFrame) SelectBlock(11);
        if (Keyboard.current.equalsKey.wasPressedThisFrame) SelectBlock(12);

        // I key INSTRUMENT BLOCK
        if (Keyboard.current.iKey.wasPressedThisFrame) SelectBlock(13);

        if (blockToPlace == null)
            blockToPlace = blockOptions[selectedBlockIndex];
    }

    void SelectBlock(int index)
    {
        if (index < 0 || index >= blockOptions.Length)
            return;

        selectedBlockIndex = index;
        blockToPlace = blockOptions[index];

        Debug.Log("Selected block: " + blockToPlace.blockName);
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
