using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Texture2D crosshairTexture; // assign your texture here
    public float size = 32f;           // crosshair size
    public float verticalOffset = -50f; // moves crosshair up/down

    private Rect crosshairRect;

    void OnGUI()
    {
        float x = (Screen.width - size) / 2f;
        float y = (Screen.height - size) / 2f + verticalOffset;

        crosshairRect = new Rect(x, y, size, size);

        GUI.DrawTexture(crosshairRect, crosshairTexture);
    }

    // VERY IMPORTANT: return the exact pixel at the center
    public Vector2 GetCrosshairScreenPos()
    {
        return new Vector2(
            crosshairRect.x + crosshairRect.width * 0.5f,
            Screen.height - (crosshairRect.y + crosshairRect.height * 0.5f)
        );
    }
}
