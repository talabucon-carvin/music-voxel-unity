using UnityEngine;

[CreateAssetMenu(menuName = "Voxel/Instrument Block Data")]
public class InstrumentBlockData : BlockData
{
    public string instrumentName;
    public AudioClip[] noteClips; // Map pitch index -> audio clip
}