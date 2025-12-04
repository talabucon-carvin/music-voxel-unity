using UnityEngine;
using System.Collections.Generic;  

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimpleVoxelWorld : MonoBehaviour
{
    [Header("Block Template & Data")]
    public GameObject blockTemplate;  // generic cube with Block.cs
    public BlockData bedrockData;     // ScriptableObject for bedrock

    [Header("World Dimensions")]
    public int width = 10;  // X span (can be negative/positive)
    public int depth = 10;  // initial Z span
    public int floorY = 0;  // Y level

    [Header("Rail Settings")]
    public int railStartZ = 0; // where your sequencer rail begins

    // Keeps track of existing blocks for expansion
    private readonly HashSet<Vector3Int> occupiedCells = new();

    /// <summary>
    /// Generates or expands the floor
    /// </summary>
    public void GenerateFloor(int minX, int maxX, int startZ, int endZ)
    {
        if (blockTemplate == null || bedrockData == null)
        {
            Debug.LogWarning("Block template or bedrock data is missing!");
            return;
        }

        for (int x = minX; x <= maxX; x++)
        {
            for (int z = startZ; z <= endZ; z++)
            {
                Vector3Int pos = new Vector3Int(x, floorY, z);

                // Skip if already exists
                if (occupiedCells.Contains(pos)) continue;

                GameObject block = Instantiate(blockTemplate, pos, Quaternion.identity, transform);

                // Assign BlockData
                Block blockComp = block.GetComponent<Block>();
                if (blockComp != null)
                    blockComp.data = bedrockData;

                // Assign material
                if (bedrockData.material != null)
                    block.GetComponent<MeshRenderer>().material = bedrockData.material;

                occupiedCells.Add(pos);
            }
        }
    }

    /// <summary>
    /// Generate initial floor centered on origin
    /// </summary>
    public void GenerateInitialFloor()
    {
        int halfWidth = width / 2;
        int startX = -halfWidth;
        int endX = width - halfWidth - 1;

        int startZ = railStartZ;
        int endZ = railStartZ + depth - 1;

        GenerateFloor(startX, endX, startZ, endZ);
    }

    /// <summary>
    /// Expands the floor in Z without touching existing blocks
    /// </summary>
    public void ExpandFloorForward(int extraDepth)
    {
        if (extraDepth <= 0) return;

        // Determine X bounds
        int halfWidth = width / 2;
        int startX = -halfWidth;
        int endX = width - halfWidth - 1;

        // Determine Z bounds
        int startZ = railStartZ + occupiedZCount();
        int endZ = startZ + extraDepth - 1;

        GenerateFloor(startX, endX, startZ, endZ);
    }

    /// <summary>
    /// Returns how many Z layers exist
    /// </summary>
    private int occupiedZCount()
    {
        int maxZ = railStartZ - 1;
        foreach (var pos in occupiedCells)
            if (pos.z > maxZ) maxZ = pos.z;
        return maxZ - railStartZ + 1;
    }

    private void Start()
    {
        GenerateInitialFloor();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SimpleVoxelWorld))]
public class SimpleVoxelWorldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SimpleVoxelWorld world = (SimpleVoxelWorld)target;

        if (GUILayout.Button("Generate Initial Floor"))
        {
            world.GenerateInitialFloor();
        }

        if (GUILayout.Button("Expand Floor Forward (Z)"))
        {
            world.ExpandFloorForward(world.depth); // default extra depth = current depth
        }
    }
}
#endif
