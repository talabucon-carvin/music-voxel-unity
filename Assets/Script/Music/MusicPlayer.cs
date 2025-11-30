using UnityEngine;

public class MusicNotePlayer : MonoBehaviour
{
    public float checkDistance = 1.5f;
    public LayerMask blockMask;

    private AudioSource audioSource;

    private Block lastBlock = null; //  important!

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Raycast downward
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, checkDistance, blockMask))
        {
            Block currentBlock = hit.collider.GetComponent<Block>();

            // If same block as last frame  do nothing
            if (currentBlock == lastBlock)
                return;

            // New block detected update memory
            lastBlock = currentBlock;

            // ---- Play note based on block type ----
            if (currentBlock is InstrumentBlock inst)
            {
                PlayInstrument(inst);
            }
            else if (currentBlock is PitchBlock pitch)
            {
                TryPlayPitch(pitch);
            }
        }
        else
        {
            // No block below reset so next block plays again
            lastBlock = null;
        }
    }

    void PlayInstrument(InstrumentBlock inst)
    {
        if (inst.instrumentData.baseNote != null)
        {
            audioSource.pitch = 1f; // reset pitch
            audioSource.PlayOneShot(inst.instrumentData.baseNote);
        }
    }

    void TryPlayPitch(PitchBlock pitch)
    {
        if (Physics.Raycast(pitch.transform.position, Vector3.down, out RaycastHit belowHit, 1f))
        {
            if (belowHit.collider.TryGetComponent(out InstrumentBlock inst))
            {
                float pitchMult = Mathf.Pow(2f, pitch.pitchData.semitoneOffset / 12f);
                audioSource.pitch = pitchMult;
                audioSource.PlayOneShot(inst.instrumentData.baseNote);
            }
        }
    }
}
