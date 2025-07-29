using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class ResponsiveCanvasSetup : MonoBehaviour
{
    [Header("Canvas Settings")]
    public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
    
    [Header("Canvas Scaler Settings")]
    public CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    public Vector2 referenceResolution = new Vector2(1920, 1080);
    public float matchWidthOrHeight = 0.5f;
    
    void Start()
    {
        SetupCanvas();
        SetupCanvasScaler();
    }
    
    void SetupCanvas()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = renderMode;
        canvas.sortingOrder = 0;
        
        if (renderMode == RenderMode.ScreenSpaceOverlay)
        {
            canvas.pixelPerfect = false;
        }
    }
    
    void SetupCanvasScaler()
    {
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode = scaleMode;
        
        if (scaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            scaler.referenceResolution = referenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = matchWidthOrHeight;
        }
    }
    
    [ContextMenu("Apply Responsive Settings")]
    public void ApplyResponsiveSettings()
    {
        SetupCanvas();
        SetupCanvasScaler();
        Debug.Log("Applied responsive canvas settings");
    }
}
