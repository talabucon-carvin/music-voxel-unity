using UnityEngine;

public class MusicBlock : MonoBehaviour
{
    public MusicBlockData data; // Inspector-friendly, no inheritance conflicts

    public void TriggerNote()
    {
        if (data != null && data.noteClip != null)
        {
            AudioSource.PlayClipAtPoint(data.noteClip, transform.position);
        }
    }
}