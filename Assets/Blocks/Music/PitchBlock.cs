using UnityEngine;

public class PitchBlock : Block
{
    public int baseOctave = 4;   // What the audio clip represents (C4)
    public int octave;           // Runtime-assigned, set in VoxelPlacer

    public PitchBlockData PitchData => data as PitchBlockData;

    public float GetUnityPitch()
    {
        int semitonesFromClip = PitchData.semitoneOffset + (octave - baseOctave) * 12;
        float pitch = Mathf.Pow(2f, semitonesFromClip / 12f);

        Debug.Log($"PitchBlock Debug: baseOctave={baseOctave}, octave={octave}, semitoneOffset={PitchData.semitoneOffset}, semitonesFromClip={semitonesFromClip}, pitch={Mathf.Pow(2f, semitonesFromClip / 12f)}");

        return pitch;


    }
}
