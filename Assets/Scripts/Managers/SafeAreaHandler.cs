using UnityEngine;
using UnityEngine.UI;

public class SafeAreaHandler : MonoBehaviour
{
    [Header("Safe Area Settings")]
    public bool applyToCanvas = true;
    public bool applyToUI = true;
    public float paddingMultiplier = 1.0f;
    
    private RectTransform rectTransform;
    private Rect lastSafeArea = new Rect(0, 0, 0, 0);
    private Canvas canvas;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponent<Canvas>();
        ApplySafeArea();
    }
    
    void Update()
    {
        // Check if safe area has changed (orientation change, etc.)
        if (lastSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }
    
    void ApplySafeArea()
    {
        if (rectTransform == null) return;
        
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        
        // Apply padding multiplier
        if (paddingMultiplier != 1.0f)
        {
            Vector2 center = (anchorMin + anchorMax) * 0.5f;
            Vector2 size = (anchorMax - anchorMin) * paddingMultiplier;
            
            anchorMin = center - size * 0.5f;
            anchorMax = center + size * 0.5f;
            
            // Clamp to screen bounds
            anchorMin = Vector2.Max(anchorMin, Vector2.zero);
            anchorMax = Vector2.Min(anchorMax, Vector2.one);
        }
        
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        
        lastSafeArea = Screen.safeArea;
        
        // Debug info
        float safeAreaPercent = (safeArea.width * safeArea.height) / (Screen.width * Screen.height);
        Debug.Log($"Safe area applied: {safeAreaPercent * 100:F1}% of screen used");
    }
    
    // Public method to force safe area update
    public void ForceSafeAreaUpdate()
    {
        ApplySafeArea();
    }
    
    // Method to get safe area information
    public SafeAreaInfo GetSafeAreaInfo()
    {
        Rect safeArea = Screen.safeArea;
        return new SafeAreaInfo
        {
            safeArea = safeArea,
            screenSize = new Vector2(Screen.width, Screen.height),
            hasNotch = safeArea.y > 0 || safeArea.height < Screen.height,
            hasHomeIndicator = safeArea.x > 0 || safeArea.width < Screen.width
        };
    }
}

[System.Serializable]
public struct SafeAreaInfo
{
    public Rect safeArea;
    public Vector2 screenSize;
    public bool hasNotch;
    public bool hasHomeIndicator;
}
