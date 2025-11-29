using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimpleVoxelWorld : MonoBehaviour
{
    [Header("Block Template & Data")]
    public GameObject blockTemplate;  // generic cube with Block.cs
    public BlockData bedrockData;     // ScriptableObject for bedrock

    [Header("World Dimensions")]
    public int width = 10;
    public int depth = 10;

    // Clears old blocks safely
    private void ClearPreviousBlocks()
    {
        if (Application.isPlaying)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }
        else
        {
#if UNITY_EDITOR
            foreach (Transform child in transform)
                DestroyImmediate(child.gameObject);
#endif
        }
    }

    // Generates the bedrock floor
    public void GenerateBedrockFloor()
    {
        ClearPreviousBlocks();

        if (blockTemplate == null || bedrockData == null)
        {
            Debug.LogWarning("Block template or bedrock data is missing!");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                GameObject block = Instantiate(blockTemplate, new Vector3(x, 0, z), Quaternion.identity, transform);

                // Assign BlockData
                Block blockComp = block.GetComponent<Block>();
                if (blockComp != null)
                    blockComp.data = bedrockData;

                // Assign material
                if (bedrockData.material != null)
                    block.GetComponent<MeshRenderer>().material = bedrockData.material;
            }
        }
    }

    private void Start()
    {
        GenerateBedrockFloor();
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

        if (GUILayout.Button("Generate Bedrock Floor"))
        {
            world.GenerateBedrockFloor();
        }
    }
}
#endif
