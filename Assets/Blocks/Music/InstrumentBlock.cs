using UnityEngine;
using System.Collections.Generic;

public class InstrumentBlock : Block
{
    public InstrumentBlockData instrumentData;

    // Get all pitch blocks below the instrument block and return them as a stack (top-first).
    public List<PitchBlock> GetPitchStackBelow()
    {
        List<PitchBlock> stack = new List<PitchBlock>();
        Vector3Int scan = Vector3Int.RoundToInt(transform.position) + Vector3Int.down;

        // Scan downwards and collect pitch blocks until another instrument block is found
        while (VoxelPlacer.worldGrid.TryGetValue(scan, out Block b))
        {
            if (b is InstrumentBlock) break;  // Stop if another instrument block is found
            if (b is PitchBlock pb) stack.Add(pb);
            scan += Vector3Int.down;
        }

        stack.Reverse(); // Reverse the stack so top pitch block comes first
        return stack;
    }

    // Play all pitch blocks below this instrument, without playing the instrument's base note.
    public void PlayAllPitchesBelow(float volume = 1f)
    {
        Vector3Int scan = Vector3Int.RoundToInt(transform.position) + Vector3Int.down;
        var pitches = new List<PitchBlock>();

        // Scan downwards and collect pitch blocks
        while (true)
        {
            if (!VoxelPlacer.worldGrid.TryGetValue(scan, out Block b)) break;
            if (b is InstrumentBlock) break;  // Stop scanning if another instrument block is found
            if (b is PitchBlock pb) pitches.Add(pb);
            scan += Vector3Int.down;
        }

        if (pitches.Count > 0)
        {
            pitches.Reverse(); // Reverse the list so top pitch block comes first
            foreach (var pb in pitches)
            {
                pb.PlayNote(volume); // Play the note of each pitch block
            }
        }
    }

    // (Optional) Play only the base note for this instrument
    // You could add a method to play the base note of the instrument if you decide to allow for that.
    public void PlayBaseNote(float volume = 1f)
    {
        if (instrumentData == null || instrumentData.baseNote == null)
        {
            Debug.LogWarning($"No base note assigned for instrument: {instrumentData?.blockName}");
            return;
        }

        // Use an AudioSource on the instrument block itself
        AudioSource source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();

        source.clip = instrumentData.baseNote;
        source.volume = volume;
        source.pitch = 1f; // base pitch
        source.spatialBlend = 1f; // 3D sound
        source.loop = false;

        source.Stop();
        source.Play();

        Debug.Log($"Playing base note for instrument: {instrumentData.blockName}");
    }

}
