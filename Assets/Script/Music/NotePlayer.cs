using System.Collections.Generic;
using UnityEngine;

public class NotePlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSourcePrefab; // Prefab or existing AudioSource
    public int poolSize = 8;              // Number of simultaneous notes allowed

    private List<AudioSource> audioPool;
    private int poolIndex = 0;

    public bool monophonic = false; // set true for piano solo

    private AudioSource lastPlayedSource;


    void Awake()
    {
        // Initialize the pool
        audioPool = new List<AudioSource>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource src = Instantiate(audioSourcePrefab, transform);
            src.playOnAwake = false;
            audioPool.Add(src);
        }
    }

    /// <summary>
    /// Plays a note with optional volume and pitch.
    /// Handles overlapping notes automatically.
    /// </summary>
    /// <param name="clip">The AudioClip to play</param>
    /// <param name="volume">Volume multiplier (0–1)</param>
    /// <param name="pitch">Pitch multiplier (default 1)</param>
    private List<AudioSource> activeNotes = new List<AudioSource>();

    public void PlayNote(AudioClip clip, float volume = 1f, float pitch = 1f, bool isChord = false)
    {
        if (clip == null) return;

        AudioSource src = audioPool[poolIndex];
        poolIndex = (poolIndex + 1) % poolSize;
        src.pitch = pitch;

        if (!isChord && activeNotes.Count > 0)
        {
            // Solo legato: stop previous note(s)
            foreach (var note in activeNotes)
            {
                if (note.isPlaying) note.Stop();
            }
            activeNotes.Clear();
        }

        src.PlayOneShot(clip, volume);

        // Track this note
        activeNotes.Add(src);
    }
}
