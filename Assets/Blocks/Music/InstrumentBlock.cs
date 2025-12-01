using UnityEngine;

public class InstrumentBlock : Block
{
    public InstrumentBlockData instrumentData;

    [HideInInspector]
    public AudioSource audioSource;

    void Awake()
    {
        // Add an AudioSource to this block, so it can play sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;  // 3D sound
        audioSource.playOnAwake = false;
    }

    //public void PlayNote(float pitchMultiplier)
    //{
    //    if (instrumentData == null || instrumentData.baseNote == null)
    //    {
    //        Debug.LogWarning("InstrumentBlock missing audio data!");
    //        return;
    //    }

    //    audioSource.pitch = pitchMultiplier;
    //    audioSource.PlayOneShot(instrumentData.baseNote);
    //}
}
