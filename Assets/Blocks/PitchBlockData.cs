using UnityEngine;

[CreateAssetMenu(menuName = "Voxel/Pitch Block Data")]
public class PitchBlockData : BlockData
{
    public int pitchIndex; // 0=C2, 1=C4, etc.
}