using UnityEngine;
using System.Collections.Generic;

public class RailGenerator : MonoBehaviour
{
    [Header("Rail Settings")]
    public GameObject railPrefab;      // The visual rail block
    public Material railMaterial;      // Material to apply
    public float railY = -0.83f;       // Height of the rail above the floor

    public int startZ = 0;             // Where the rail begins
    public int initialLength = 16;     // First length of the rail

    private HashSet<int> railZs = new();  // Track Z positions to prevent duplicates

    void Start()
    {
        GenerateRail(initialLength);
    }

    public void GenerateRail(int length)
    {
        int endZ = startZ + length - 1;

        for (int z = startZ; z <= endZ; z++)
        {
            if (railZs.Contains(z))
                continue;

            Vector3 pos = new Vector3(0, railY, z); // use Vector3 now
            GameObject rail = Instantiate(railPrefab, pos, Quaternion.identity, transform);

            // Material assignment
            MeshRenderer rend = rail.GetComponentInChildren<MeshRenderer>();
            if (rend != null && railMaterial != null)
                rend.material = railMaterial;

            railZs.Add(z);
        }
    }

    public void ExpandRail(int extraLength)
    {
        if (extraLength <= 0) return;

        int existingLength = railZs.Count;
        GenerateRail(existingLength + extraLength);
    }
}
