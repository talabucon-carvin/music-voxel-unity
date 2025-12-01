using UnityEngine;

public static class PianoRange
{
    // 0 = C, 1 = C#, ... 11 = B
    public static readonly (int min, int max)[] noteRanges =
    {
        (1, 8), // C
        (1, 8), // C#
        (1, 7), // D
        (1, 7), // D#
        (1, 7), // E
        (1, 7), // F
        (1, 7), // F#
        (0, 7), // G
        (0, 7), // G#
        (0, 7), // A
        (0, 7), // A#
        (0, 7), // B
    };
}
