using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoxelPlacer : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform worldParent;
    public Crosshair crosshair;
    public LineRenderer lineRenderer;

    [Header("Block Options")]
    public BlockData[] blockOptions;
    public GameObject[] blockPrefabs;
    private int selectedBlockIndex = 0;
    private BlockData blockToPlace;

    [Header("Placement Settings")]
    public float maxRayDistance = 10f;
    public LayerMask placementMask;

    [Header("Instrument Options")]
    public InstrumentBlockData[] instrumentDatas;
    public GameObject[] instrumentPrefabs;
    private int selectedInstrumentIndex = 0;
    private bool instrumentModeActive = false;

    private int selectedPitchIndex = -1;
    private int selectedOctave = 4;

    [Header("Standing-On Trigger")]
    public float checkDistance = 1.5f;
    public LayerMask blockMaskStanding = ~0;
    [SerializeField] Transform feetOrigin;

    private Block lastBlockBelow;
    private Vector3 lastCheckedPosition;
    private Block lastBlockPlayed;

    public static Dictionary<Vector3Int, Block> worldGrid = new();

    void Awake()
    {
        if (blockOptions.Length > 0)
            blockToPlace = blockOptions[selectedBlockIndex];
    }

    void Update()
    {
        HandleScroll();
        HandleHotkeys();
        HandlePlacement();
        HandleDelete();
    }

    void LateUpdate()
    {
        Vector3 pos = feetOrigin.position;

        if (pos != lastCheckedPosition)
        {
            lastCheckedPosition = pos;
            TriggerStandingOnBlock();
        }
    }

    // -------------------------------------------------------
    // INPUT HANDLING
    // -------------------------------------------------------

    void HandleScroll()
    {
        float scroll = Mouse.current.scroll.y.ReadValue();
        if (scroll == 0) return;

        if (instrumentModeActive)
        {
            selectedInstrumentIndex = (selectedInstrumentIndex + (scroll > 0 ? 1 : -1) + instrumentDatas.Length) % instrumentDatas.Length;
            blockToPlace = instrumentDatas[selectedInstrumentIndex];
        }
        else if (selectedPitchIndex != -1)
        {
            AdjustOctave(scroll);
        }
    }

    void HandleHotkeys()
    {
        // Toggle back to normal block mode
        if (Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            instrumentModeActive = false;
            SelectBlock(0);
        }

        // Number hotkeys 1–0  0–9
        // Number keys 1–0 map to pitch blocks 0–9
        if (Keyboard.current.digit1Key.wasPressedThisFrame) { instrumentModeActive = false; SelectBlock(0); }
        if (Keyboard.current.digit2Key.wasPressedThisFrame) { instrumentModeActive = false; SelectBlock(1); }
        if (Keyboard.current.digit3Key.wasPressedThisFrame) { instrumentModeActive = false; SelectBlock(2); }
        if (Keyboard.current.digit4Key.wasPressedThisFrame) { instrumentModeActive = false; SelectBlock(3); }
        if (Keyboard.current.digit5Key.wasPressedThisFrame) { instrumentModeActive = false; SelectBlock(4); }
        if (Keyboard.current.digit6Key.wasPressedThisFrame) { instrumentModeActive = false; SelectBlock(5); }
        if (Keyboard.current.digit7Key.wasPressedThisFrame) { instrumentModeActive = false; SelectBlock(6); }
        if (Keyboard.current.digit8Key.wasPressedThisFrame) { instrumentModeActive = false; SelectBlock(7); }
        if (Keyboard.current.digit9Key.wasPressedThisFrame) { instrumentModeActive = false; SelectBlock(8); }
        if (Keyboard.current.digit0Key.wasPressedThisFrame) { instrumentModeActive = false; SelectBlock(9); }

        // Minus and equals
        if (Keyboard.current.minusKey.wasPressedThisFrame)
        {
            instrumentModeActive = false;
            SelectBlock(10);
        }
        if (Keyboard.current.equalsKey.wasPressedThisFrame)
        {
            instrumentModeActive = false;
            SelectBlock(11);
        }


        // Toggle instrument placement mode
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            instrumentModeActive = !instrumentModeActive;

            blockToPlace = instrumentModeActive
                ? instrumentDatas[selectedInstrumentIndex]
                : blockOptions[selectedBlockIndex];

            Debug.Log(instrumentModeActive ? "Instrument mode ON" : "Instrument mode OFF");
        }

        if (blockToPlace == null && blockOptions.Length > 0)
            blockToPlace = blockOptions[selectedBlockIndex];
    }

    void SelectBlock(int index)
    {
        if (index < 0 || index >= blockOptions.Length) return;

        selectedBlockIndex = index;
        blockToPlace = blockOptions[index];

        if (blockToPlace is PitchBlockData pitchData)
        {
            selectedPitchIndex = pitchData.semitoneOffset;

            var range = PianoRange.noteRanges[selectedPitchIndex];
            selectedOctave = Mathf.Clamp(selectedOctave, range.min, range.max);

            Debug.Log($"Selected pitch: {PitchName(selectedPitchIndex)}{selectedOctave}");
        }
        else
        {
            selectedPitchIndex = -1;
        }
    }

    // -------------------------------------------------------
    // PLACEMENT
    // -------------------------------------------------------

    void HandlePlacement()
    {
        Vector2 crossPos = crosshair ? crosshair.GetCrosshairScreenPos()
                                     : new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray ray = playerCamera.ScreenPointToRay(crossPos);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, placementMask))
        {
            Vector3Int cell = Vector3Int.RoundToInt(hit.point + hit.normal * 0.5f);

            if (Mouse.current.leftButton.wasPressedThisFrame)
                PlaceBlock(cell);
        }
    }

    void PlaceBlock(Vector3Int cell)
    {
        if (worldGrid.ContainsKey(cell)) return;

        GameObject prefab;
        BlockData data;

        if (instrumentModeActive)
        {
            prefab = instrumentPrefabs[selectedInstrumentIndex];
            data = instrumentDatas[selectedInstrumentIndex];
        }
        else
        {
            prefab = blockPrefabs[selectedBlockIndex];
            data = blockOptions[selectedBlockIndex];
        }

        GameObject go = Instantiate(prefab, cell, Quaternion.identity, worldParent);

        Block b = go.GetComponent<Block>();
        if (b == null) { Destroy(go); return; }

        b.data = data;
        worldGrid[cell] = b;

        // Pitch block setup
        if (b is PitchBlock pb)
        {
            pb.octave = selectedOctave;
            pb.instrumentAbove = ScanUpForInstrument(cell);

            if (data is PitchBlockData pbData)
                pb.PitchData = pbData;
        }

        // Instrument block setup
        if (b is InstrumentBlock inst)
            ScanDownAssignInstrument(cell, inst);

        if (data.material)
        {
            MeshRenderer rend = go.GetComponent<MeshRenderer>();
            if (rend) rend.material = data.material;
        }

        PlayStack(b);
    }

    // -------------------------------------------------------
    // DELETE
    // -------------------------------------------------------

    void HandleDelete()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) return;

        Vector2 crossPos = crosshair ? crosshair.GetCrosshairScreenPos()
                                     : new Vector2(Screen.width / 2f, Screen.height / 2f);

        if (Physics.Raycast(playerCamera.ScreenPointToRay(crossPos),
            out RaycastHit hit, maxRayDistance, placementMask))
        {
            DeleteBlock(hit.collider);
        }
    }

    void DeleteBlock(Collider col)
    {
        if (col == null) return;

        Vector3Int cell = Vector3Int.RoundToInt(col.transform.position);

        if (!worldGrid.TryGetValue(cell, out Block b)) return;

        if (b is InstrumentBlock) UnlinkBelowInstrument(cell);
        if (b is PitchBlock pb) pb.instrumentAbove = null;

        worldGrid.Remove(cell);
        Destroy(b.gameObject);

        Debug.Log($"Removed block at {cell}");
    }

    // -------------------------------------------------------
    // STANDING-ON SOUND
    // -------------------------------------------------------
    void TriggerStandingOnBlock()
    {
        if (!Physics.Raycast(feetOrigin.position, Vector3.down, out RaycastHit hit, checkDistance, blockMaskStanding))
        {
            lastBlockBelow = null;
            return;
        }

        Block blockBelow = hit.collider.GetComponent<Block>();
        if (blockBelow == null || blockBelow == lastBlockBelow)
            return;

        lastBlockBelow = blockBelow;
        Vector3Int startPos = Vector3Int.RoundToInt(blockBelow.transform.position);

        var stacks = ScanFullColumn(startPos);

        foreach (var stack in stacks)
        {
            if (stack.instrument != null && stack.pitches.Count > 0)
            {
                foreach (var p in stack.pitches)
                    p.PlayWithInstrument(stack.instrument, 1f);
            }
            else if (stack.instrument != null && stack.pitches.Count == 0)
            {
                stack.instrument.PlayBaseNote(1f);
            }
            else if (stack.instrument == null && stack.pitches.Count > 0)
            {
                foreach (var p in stack.pitches)
                    p.PlayNote(1f);
            }
        }
    }


    public List<(InstrumentBlock instrument, List<PitchBlock> pitches)> ScanFullColumn(Vector3Int start)
    {
        const int MAX_SCAN = 128;
        int x = start.x, z = start.z;

        List<(InstrumentBlock instrument, List<PitchBlock> pitches)> result = new();
        List<PitchBlock> currentPitchStack = new();

        // Scan from bottom to top
        for (int i = -MAX_SCAN; i <= MAX_SCAN; i++)
        {
            Vector3Int scan = start + Vector3Int.up * i;

            if (!worldGrid.TryGetValue(scan, out Block b))
                continue;

            if (b is PitchBlock pb)
            {
                // Only add pitch blocks not already associated with an instrument
                if (pb.instrumentAbove == null)
                    currentPitchStack.Add(pb);
            }
            else if (b is InstrumentBlock inst)
            {
                // Add current pitch stack (if any) as a separate entry
                if (currentPitchStack.Count > 0)
                {
                    result.Add((null, new List<PitchBlock>(currentPitchStack)));
                    currentPitchStack.Clear();
                }

                // Collect all pitch blocks below this instrument
                List<PitchBlock> pitchesBelow = inst.GetPitchStackBelow();
                result.Add((inst, pitchesBelow));
            }
            else
            {
                // Non-pitch, non-instrument block ends any pitch stack
                if (currentPitchStack.Count > 0)
                {
                    result.Add((null, new List<PitchBlock>(currentPitchStack)));
                    currentPitchStack.Clear();
                }
            }
        }

        // Add remaining pitch stack if any
        if (currentPitchStack.Count > 0)
            result.Add((null, new List<PitchBlock>(currentPitchStack)));

        // Sort pitches top-first in each stack
        foreach (var stack in result)
            stack.pitches.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));

        return result;
    }


    // -------------------------------------------------------
    // STACK PLAY
    // -------------------------------------------------------

    public void PlayStack(Block b)
    {
        if (b is PitchBlock pb)
        {
            if (pb.instrumentAbove != null)
                pb.instrumentAbove.PlayAllPitchesBelow(1f);
            else
                pb.PlayNote(1f);
        }
        else if (b is InstrumentBlock inst)
        {
            inst.PlayAllPitchesBelow();
        }
    }

    // -------------------------------------------------------
    // SCAN HELPERS
    // -------------------------------------------------------

    public InstrumentBlock ScanUpForInstrument(Vector3Int pos)
    {
        Vector3Int scan = pos + Vector3Int.up;

        while (worldGrid.TryGetValue(scan, out Block b))
        {
            if (b is InstrumentBlock inst) return inst;
            scan += Vector3Int.up;
        }

        return null;
    }

    public void ScanDownAssignInstrument(Vector3Int instPos, InstrumentBlock inst)
    {
        Vector3Int scan = instPos + Vector3Int.down;

        while (worldGrid.TryGetValue(scan, out Block b))
        {
            if (b is InstrumentBlock) break;

            if (b is PitchBlock pb)
                pb.instrumentAbove = inst;

            scan += Vector3Int.down;
        }
    }

    void UnlinkBelowInstrument(Vector3Int pos)
    {
        Vector3Int scan = pos + Vector3Int.down;

        while (worldGrid.TryGetValue(scan, out Block b))
        {
            if (b is InstrumentBlock) break;
            if (b is PitchBlock pb) pb.instrumentAbove = null;
            scan += Vector3Int.down;
        }
    }

    // -------------------------------------------------------
    // OCTAVE
    // -------------------------------------------------------

    void AdjustOctave(float scroll)
    {
        var range = PianoRange.noteRanges[selectedPitchIndex];
        selectedOctave = Mathf.Clamp(
            selectedOctave + (scroll > 0 ? 1 : -1),
            range.min, range.max
        );

        Debug.Log($"Octave changed: {PitchName(selectedPitchIndex)}{selectedOctave}");
    }

    string PitchName(int i)
    {
        string[] names = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        return (i >= 0 && i < names.Length) ? names[i] : "?";
    }
}
