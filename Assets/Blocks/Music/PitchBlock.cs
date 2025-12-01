using UnityEngine;

public class PitchBlock : Block
{
    public int octave; // runtime assigned

    public PitchBlockData PitchData => data as PitchBlockData;

    // Optional: method to calculate frequency/pitch shift
    public float GetUnityPitch()
    {
        int semitones = PitchData.semitoneOffset + (octave * 12);
        return Mathf.Pow(2f, semitones / 12f);
    }
}