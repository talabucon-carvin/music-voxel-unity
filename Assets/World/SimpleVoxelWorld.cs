using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimpleVoxelWorld : MonoBehaviour
{
    [Header("Block Prefab")]
    public GameObject bedrockPrefab;

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

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (bedrockPrefab)
                    Instantiate(bedrockPrefab, new Vector3(x, 0, z), Quaternion.identity, transform);
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
