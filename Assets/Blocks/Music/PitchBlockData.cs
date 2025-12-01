using UnityEngine;

[CreateAssetMenu(menuName = "Voxel/Pitch Block Data")]
public class PitchBlockData : BlockData
{
    [Header("Pitch offset from base note (0–11)")]
    public int semitoneOffset;

    [Header("Octave of base audio clip")]
    public int baseOctave = 4;  // e.g., C4

    public AudioClip baseTone; // fallback tone
}