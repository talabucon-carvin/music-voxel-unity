using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public Texture2D crosshairTexture; // assign your texture here
    public float size = 32f;           // size of the crosshair in pixels
    public float verticalOffset = -50f; // negative = up, positive = down

    void OnGUI()
    {
        // Calculate center of the screen
        float x = (Screen.width - size) / 2;
        float y = (Screen.height - size) / 2 + verticalOffset;

        // Draw the texture at the adjusted position
        GUI.DrawTexture(new Rect(x, y, size, size), crosshairTexture);
    }
}
