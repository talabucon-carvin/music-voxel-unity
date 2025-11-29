using UnityEngine;

[CreateAssetMenu(menuName = "Voxel/Block Data")]
public class BlockData : ScriptableObject
{
    public string blockName;
    public Material material;

    public bool isBreakable;
    public float hardness;
    public float lightEmission;
}
