using UnityEngine;
using UnityEngine.UI;

public class UIAnchorHelper : MonoBehaviour
{
    [Header("Anchor Presets")]
    public AnchorPreset anchorPreset = AnchorPreset.MiddleCenter;
    public bool setPosition = true;
    public bool setSize = false;
    
    [Header("Custom Anchor Settings")]
    public Vector2 anchorMin = new Vector2(0.5f, 0.5f);
    public Vector2 anchorMax = new Vector2(0.5f, 0.5f);
    public Vector2 anchoredPosition = Vector2.zero;
    public Vector2 sizeDelta = new Vector2(100, 100);
    
    public enum AnchorPreset
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight,
        StretchTop, StretchMiddle, StretchBottom,
        StretchLeft, StretchCenter, StretchRight,
        StretchAll,
        Custom
    }
    
    void Start()
    {
        ApplyAnchor();
    }
    
    [ContextMenu("Apply Anchor")]
    public void ApplyAnchor()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning("UIAnchorHelper requires a RectTransform component!");
            return;
        }
        
        if (anchorPreset != AnchorPreset.Custom)
        {
            SetAnchorPreset(rectTransform, anchorPreset, setPosition, setSize);
        }
        else
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            if (setPosition)
                rectTransform.anchoredPosition = anchoredPosition;
            if (setSize)
                rectTransform.sizeDelta = sizeDelta;
        }
    }
    
    void SetAnchorPreset(RectTransform rectTransform, AnchorPreset preset, bool position, bool size)
    {
        switch (preset)
        {
            case AnchorPreset.TopLeft:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                if (position) rectTransform.anchoredPosition = new Vector2(50, -50);
                break;
            case AnchorPreset.TopCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                if (position) rectTransform.anchoredPosition = new Vector2(0, -50);
                break;
            case AnchorPreset.TopRight:
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                if (position) rectTransform.anchoredPosition = new Vector2(-50, -50);
                break;
            case AnchorPreset.MiddleLeft:
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                if (position) rectTransform.anchoredPosition = new Vector2(50, 0);
                break;
            case AnchorPreset.MiddleCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                if (position) rectTransform.anchoredPosition = Vector2.zero;
                break;
            case AnchorPreset.MiddleRight:
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                if (position) rectTransform.anchoredPosition = new Vector2(-50, 0);
                break;
            case AnchorPreset.BottomLeft:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                if (position) rectTransform.anchoredPosition = new Vector2(50, 50);
                break;
            case AnchorPreset.BottomCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 0);
                if (position) rectTransform.anchoredPosition = new Vector2(0, 50);
                break;
            case AnchorPreset.BottomRight:
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                if (position) rectTransform.anchoredPosition = new Vector2(-50, 50);
                break;
            case AnchorPreset.StretchTop:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                if (position) rectTransform.anchoredPosition = new Vector2(0, -25);
                if (size) rectTransform.sizeDelta = new Vector2(0, 50);
                break;
            case AnchorPreset.StretchMiddle:
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                if (position) rectTransform.anchoredPosition = new Vector2(0, 0);
                if (size) rectTransform.sizeDelta = new Vector2(0, 50);
                break;
            case AnchorPreset.StretchBottom:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                if (position) rectTransform.anchoredPosition = new Vector2(0, 25);
                if (size) rectTransform.sizeDelta = new Vector2(0, 50);
                break;
            case AnchorPreset.StretchLeft:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 1);
                if (position) rectTransform.anchoredPosition = new Vector2(25, 0);
                if (size) rectTransform.sizeDelta = new Vector2(50, 0);
                break;
            case AnchorPreset.StretchCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                if (position) rectTransform.anchoredPosition = new Vector2(0, 0);
                if (size) rectTransform.sizeDelta = new Vector2(50, 0);
                break;
            case AnchorPreset.StretchRight:
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                if (position) rectTransform.anchoredPosition = new Vector2(-25, 0);
                if (size) rectTransform.sizeDelta = new Vector2(50, 0);
                break;
            case AnchorPreset.StretchAll:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                if (position) rectTransform.anchoredPosition = Vector2.zero;
                if (size) rectTransform.sizeDelta = Vector2.zero;
                break;
        }
    }
}
