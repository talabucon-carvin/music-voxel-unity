using UnityEngine;

[CreateAssetMenu(menuName = "Voxel/Pitch Block Data")]
public class PitchBlockData : BlockData
{
    [Header("Pitch offset from base note (0–11)")]
    public int semitoneOffset;

    public AudioClip baseTone; // fallback tone
}