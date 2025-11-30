using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class VoxelPlacer : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform worldParent;
    public Crosshair crosshair;
    public LineRenderer lineRenderer;

    [Header("Block Options")]
    public BlockData[] blockOptions;     // 0 = platform, 1 = music
    public GameObject[] blockPrefabs;    // same order as blockOptions
    private int selectedBlockIndex = 0;
    private BlockData blockToPlace;

    [Header("Placement Settings")]
    public float maxRayDistance = 10f;
    public LayerMask placementMask;

    // Music block dictionary for vertical stack lookup
    public Dictionary<Vector3Int, MusicBlock> musicGrid = new Dictionary<Vector3Int, MusicBlock>();

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

        if (blockToPlace == null) blockToPlace = blockOptions[selectedBlockIndex];
    }

    void HandlePlacement()
    {
        Vector2 crossPos = crosshair != null ? crosshair.GetCrosshairScreenPos() : new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = playerCamera.ScreenPointToRay(crossPos);
        RaycastHit hit;
        Vector3 endPoint = ray.origin + ray.direction * maxRayDistance;

        if (Physics.Raycast(ray, out hit, maxRayDistance, placementMask))
        {
            endPoint = hit.point;
            Vector3 spawnPos = hit.point + hit.normal * 0.5f;
            spawnPos = new Vector3(Mathf.Round(spawnPos.x), Mathf.Round(spawnPos.y), Mathf.Round(spawnPos.z));

            Debug.DrawLine(spawnPos - Vector3.one * 0.5f, spawnPos + Vector3.one * 0.5f, Color.yellow);

            // Place
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (!Physics.CheckBox(spawnPos, Vector3.one * 0.45f, Quaternion.identity, placementMask))
                {
                    GameObject prefabToSpawn = blockPrefabs[selectedBlockIndex];
                    GameObject newBlock = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, worldParent);

                    // Assign data
                    MusicBlock musicComp = newBlock.GetComponent<MusicBlock>();
                    if (musicComp != null)
                    {
                        musicComp.data = (MusicBlockData)blockToPlace;
                        Vector3Int gridPos = Vector3Int.RoundToInt(spawnPos);
                        musicGrid[gridPos] = musicComp;
                    }
                    else
                    {
                        Block blockComp = newBlock.GetComponent<Block>();
                        if (blockComp != null) blockComp.data = blockToPlace;
                    }

                    // Apply material
                    MeshRenderer rend = newBlock.GetComponent<MeshRenderer>();
                    if (rend != null && blockToPlace.material != null)
                        rend.material = blockToPlace.material;
                }
            }

            // Delete
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Block baseBlock = hit.collider.GetComponent<Block>();
                MusicBlock musicBlock = hit.collider.GetComponent<MusicBlock>();

                if (musicBlock != null)
                {
                    Vector3Int gridPos = Vector3Int.RoundToInt(musicBlock.transform.position);
                    musicGrid.Remove(gridPos);
                    Destroy(musicBlock.gameObject);
                    return;
                }

                if (baseBlock != null)
                {
                    Destroy(baseBlock.gameObject);
                    return;
                }
            }
        }

        // LineRenderer for debug
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, endPoint);
        }
    }
}
