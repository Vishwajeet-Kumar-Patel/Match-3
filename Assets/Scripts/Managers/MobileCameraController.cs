using UnityEngine;

public class MobileCameraController : MonoBehaviour
{
    [Header("Mobile Camera Settings")]
    public bool enableSafeArea = true;
    public bool autoFitToScreen = true;
    public float safeAreaPadding = 0.1f;
    
    private Camera cam;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        if (autoFitToScreen)
        {
            AdjustForMobile();
        }
    }
    
    void Update()
    {
        // Continuously adjust for orientation changes
        if (autoFitToScreen && Screen.orientation != lastOrientation)
        {
            AdjustForMobile();
            lastOrientation = Screen.orientation;
        }
    }
    
    private ScreenOrientation lastOrientation;
    
    void AdjustForMobile()
    {
        if (cam == null) return;
        
        // Get screen dimensions
        float screenAspect = (float)Screen.width / Screen.height;
        
        // Adjust camera for mobile aspect ratios
        if (screenAspect < 0.6f) // Very tall screens (modern phones)
        {
            cam.orthographicSize = 3.0f;
        }
        else if (screenAspect < 0.75f) // Standard phones
        {
            cam.orthographicSize = 2.8f;
        }
        else // Tablets
        {
            cam.orthographicSize = 3.5f;
        }
        
        // Handle safe area
        if (enableSafeArea && Screen.safeArea.height < Screen.height)
        {
            float safeAreaRatio = Screen.safeArea.height / Screen.height;
            cam.orthographicSize *= (1f + (1f - safeAreaRatio) * 0.5f);
            
            Vector3 pos = transform.position;
            pos.y = -(1f - safeAreaRatio) * safeAreaPadding;
            transform.position = pos;
        }
        
        Debug.Log($"Mobile camera adjusted: Size {cam.orthographicSize:F1}, Aspect {screenAspect:F2}");
    }
    
    // Public method to manually trigger adjustment
    public void ForceAdjustForMobile()
    {
        AdjustForMobile();
    }
}
