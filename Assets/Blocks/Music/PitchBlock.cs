using UnityEngine;

public class PitchBlock : Block
{
    public PitchBlockData PitchData;
    public int octave;

    // Instrument located directly above this pitch block
    public InstrumentBlock instrumentAbove;

    private AudioSource source;

    void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 1f;
        source.loop = false;
    }

    // Calculate audio pitch from semitone difference
    public float GetUnityPitch()
    {
        int offset = PitchData.semitoneOffset;
        int diff = (octave - PitchData.baseOctave) * 12 + offset;
        return Mathf.Pow(2f, diff / 12f);
    }

    public void PlayNote(float volume = 1f)
    {
        AudioClip clip = instrumentAbove?.instrumentData.baseNote ?? PitchData.baseTone;
        if (clip == null) return;

        source.Stop();
        source.clip = clip;
        source.pitch = GetUnityPitch();
        source.volume = volume;
        source.Play();
    }

    public void PlayWithInstrument(InstrumentBlock inst, float volume)
    {
        float pitchValue = GetUnityPitch();
        AudioClip clip = inst.instrumentData.baseNote;
        if (clip == null) return;

        // Create a temp GameObject with AudioSource
        GameObject tempGO = new GameObject("TempPitch");
        tempGO.transform.position = transform.position;

        AudioSource a = tempGO.AddComponent<AudioSource>();
        a.clip = clip;
        a.spatialBlend = 1f;
        a.volume = volume;
        a.pitch = pitchValue;
        a.Play();

        Destroy(tempGO, clip.length / Mathf.Abs(a.pitch)); // Clean up
    }

}
