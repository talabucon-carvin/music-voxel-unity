using UnityEngine;

[CreateAssetMenu(menuName = "Voxel/Blocks/Instrument Block Data")]
public class InstrumentBlockData : BlockData
{
    // The base audio sample (usually Middle C)
    public AudioClip baseNote;

    // Optional: volume, ADSR, etc later
}
