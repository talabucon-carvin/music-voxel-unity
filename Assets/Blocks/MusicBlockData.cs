using UnityEngine;

[CreateAssetMenu(menuName = "Voxel/Music Block Data")]
public class MusicBlockData : BlockData
{
    public string instrumentName;
    public AudioClip noteClip;
    [Range(0, 127)] public int pitch;
    public float duration = 0.25f; // default 1/16th note
}
