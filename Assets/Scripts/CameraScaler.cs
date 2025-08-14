using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    [Header("Constraints")]
    public float minWorldWidth = 5.6f;   // Minimum allowed width in world units
    public float minWorldHeight = 10f;   // Minimum allowed height in world units

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    void Update()
    {
        float aspect = (float)Screen.width / Screen.height;

        // Compute the orthographic size needed to satisfy both width and height constraints
        float sizeByWidth = (minWorldWidth / aspect) * 0.5f;
        float sizeByHeight = minWorldHeight * 0.5f;

        cam.orthographicSize = Mathf.Max(sizeByWidth, sizeByHeight);
    }
}
