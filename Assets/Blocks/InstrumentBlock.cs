using UnityEngine;

// Top block in stack
public class InstrumentBlock : MonoBehaviour
{
    public InstrumentBlockData data;

    // You can later trigger notes for this instrument
    public AudioClip GetNoteClip(int pitchIndex)
    {
        if (data == null || data.noteClips == null || pitchIndex < 0 || pitchIndex >= data.noteClips.Length)
            return null;
        return data.noteClips[pitchIndex];
    }
}