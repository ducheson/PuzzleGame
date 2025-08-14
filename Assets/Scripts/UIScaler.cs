using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class UIScaler : MonoBehaviour
{
    [SerializeField] private Vector2 referenceResolution = new Vector2(1080, 1920); // Works for portrait & landscape
    [SerializeField, Range(0f, 1f)] private float landscapeMatch = 0.0f; // Prefer width in landscape
    [SerializeField, Range(0f, 1f)] private float portraitMatch = 1.0f;  // Prefer height in portrait

    private CanvasScaler scaler;

    void Awake()
    {
        scaler = GetComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        UpdateScaler();
    }

    void Update()
    {
        // Optional: update every frame if device can rotate
        UpdateScaler();
    }

    private void UpdateScaler()
    {
        float screenAspect = (float)Screen.width / Screen.height;
        bool isLandscape = screenAspect > 1.0f;

        scaler.matchWidthOrHeight = isLandscape ? landscapeMatch : portraitMatch;
    }
}
