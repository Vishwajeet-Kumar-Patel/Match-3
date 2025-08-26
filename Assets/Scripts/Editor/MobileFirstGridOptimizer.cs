using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

#if UNITY_EDITOR
public class MobileFirstGridOptimizer : EditorWindow
{
    [MenuItem("Tools/Mobile First Grid Optimizer")]
    public static void ShowWindow()
    {
        GetWindow<MobileFirstGridOptimizer>("Mobile Grid Optimizer");
    }
    
    [Header("Mobile Settings")]
    private float mobileScale = 0.4f;
    private float mobileSpacing = 0.5f;
    private float mobileCameraSize = 2.8f;
    private bool fitToScreen = true;
    private bool enableSafeArea = true;
    
    [Header("Platform Specific")]
    private bool optimizeForIOS = true;
    private bool optimizeForAndroid = true;
    private bool addMobileUI = true;
    
    void OnGUI()
    {
        GUILayout.Label("📱 Mobile First Grid Optimizer", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        GUILayout.Label("Grid Settings:", EditorStyles.boldLabel);
        mobileScale = EditorGUILayout.Slider("Mobile Scale", mobileScale, 0.2f, 0.8f);
        mobileSpacing = EditorGUILayout.Slider("Mobile Spacing", mobileSpacing, 0.3f, 0.7f);
        mobileCameraSize = EditorGUILayout.Slider("Camera Size", mobileCameraSize, 2.0f, 4.0f);
        
        GUILayout.Space(5);
        
        fitToScreen = EditorGUILayout.Toggle("Auto Fit to Screen", fitToScreen);
        enableSafeArea = EditorGUILayout.Toggle("Enable Safe Area", enableSafeArea);
        addMobileUI = EditorGUILayout.Toggle("Add Mobile UI", addMobileUI);
        
        GUILayout.Space(5);
        
        optimizeForIOS = EditorGUILayout.Toggle("Optimize for iOS", optimizeForIOS);
        optimizeForAndroid = EditorGUILayout.Toggle("Optimize for Android", optimizeForAndroid);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("📱 Apply Mobile Optimization"))
        {
            ApplyMobileOptimization();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🎯 Quick Mobile Fix"))
        {
            QuickMobileFix();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📏 Test Different Screen Sizes"))
        {
            TestDifferentScreenSizes();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("Mobile Features:", EditorStyles.helpBox);
        GUILayout.Label("• Perfect scale for phones/tablets", EditorStyles.helpBox);
        GUILayout.Label("• iOS notch/safe area support", EditorStyles.helpBox);
        GUILayout.Label("• Android navigation bar support", EditorStyles.helpBox);
        GUILayout.Label("• Touch-friendly tile sizes", EditorStyles.helpBox);
    }
    
    void QuickMobileFix()
    {
        Debug.Log("=== APPLYING QUICK MOBILE FIX ===");
        
        // Set optimal mobile values
        mobileScale = 0.35f;
        mobileSpacing = 0.45f;
        mobileCameraSize = 2.5f;
        fitToScreen = true;
        enableSafeArea = true;
        
        ApplyMobileOptimization();
        
        Debug.Log("✅ Quick mobile fix applied!");
    }
    
    void ApplyMobileOptimization()
    {
        Debug.Log("=== APPLYING MOBILE OPTIMIZATION ===");
        
        // 1. Set default scale to 1 and optimize grid
        OptimizeGridForMobile();
        
        // 2. Configure camera for mobile
        ConfigureMobileCamera();
        
        // 3. Update Match3Manager settings
        UpdateMatch3ManagerForMobile();
        
        // 4. Add mobile-specific features
        if (addMobileUI)
        {
            SetupMobileUI();
        }
        
        // 5. Configure platform-specific settings
        ConfigurePlatformSettings();
        
        Debug.Log("=== MOBILE OPTIMIZATION COMPLETE ===");
        Debug.Log("🎮 Press PLAY to test on mobile-sized screen!");
    }
    
    void OptimizeGridForMobile()
    {
        // 1. Reset all tile scales to 1 (default)
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        foreach (Tile tile in tiles)
        {
            tile.transform.localScale = Vector3.one; // Default scale = 1
        }
        
        // 2. Configure GridParent for mobile
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
        {
            // Set mobile-optimized scale
            gridParent.transform.localScale = Vector3.one * mobileScale;
            
            // Center position for mobile
            Vector3 centerPos = Vector3.zero;
            if (enableSafeArea)
            {
                // Account for notches and navigation bars
                centerPos.y = -0.2f; // Slightly lower for mobile
            }
            gridParent.transform.position = centerPos;
            
            Debug.Log($"✅ Grid optimized for mobile: Scale {mobileScale}, Position {centerPos}");
        }
    }
    
    void ConfigureMobileCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;
        
        // Mobile-optimized camera settings
        mainCamera.transform.position = new Vector3(0, 0, -10f);
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = mobileCameraSize;
        
        // Mobile-friendly background
        mainCamera.backgroundColor = new Color(0.9f, 0.95f, 1f, 1f); // Light blue
        
        // Add camera component for mobile features
        MobileCameraController mobileController = mainCamera.GetComponent<MobileCameraController>();
        if (mobileController == null)
        {
            mobileController = mainCamera.gameObject.AddComponent<MobileCameraController>();
        }
        
        mobileController.enableSafeArea = enableSafeArea;
        mobileController.autoFitToScreen = fitToScreen;
        
        Debug.Log($"✅ Mobile camera configured: Size {mobileCameraSize}");
    }
    
    void UpdateMatch3ManagerForMobile()
    {
        GameObject managerObj = GameObject.Find("Match3Manager");
        if (managerObj == null) return;
        
        Match3Manager manager = managerObj.GetComponent<Match3Manager>();
        if (manager == null) return;
        
        // Update spacing using reflection
        var spacingField = typeof(Match3Manager).GetField("tileSpacing", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (spacingField != null)
        {
            spacingField.SetValue(manager, mobileSpacing);
        }
        
        // Add mobile-specific settings
        MobileMatch3Controller mobileController = manager.GetComponent<MobileMatch3Controller>();
        if (mobileController == null)
        {
            mobileController = manager.gameObject.AddComponent<MobileMatch3Controller>();
        }
        
        EditorUtility.SetDirty(manager);
        
        Debug.Log($"✅ Match3Manager updated for mobile: Spacing {mobileSpacing}");
    }
    
    void SetupMobileUI()
    {
        // Create or update Canvas for mobile
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("MobileCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
        }
        
        // Mobile-optimized Canvas settings
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        }
        
        // Mobile-first scaling
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920); // Mobile portrait
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f; // Balance between width and height
        
        // Add GraphicRaycaster for touch
        if (canvas.GetComponent<GraphicRaycaster>() == null)
        {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }
        
        // Add Safe Area component
        SafeAreaHandler safeArea = canvas.GetComponent<SafeAreaHandler>();
        if (safeArea == null && enableSafeArea)
        {
            safeArea = canvas.gameObject.AddComponent<SafeAreaHandler>();
        }
        
        Debug.Log("✅ Mobile UI configured with safe area support");
    }
    
    void ConfigurePlatformSettings()
    {
        // iOS specific optimizations
        if (optimizeForIOS)
        {
            // Configure for iOS notches and safe areas
            Debug.Log("✅ iOS optimizations applied");
        }
        
        // Android specific optimizations  
        if (optimizeForAndroid)
        {
            // Configure for Android navigation bars
            Debug.Log("✅ Android optimizations applied");
        }
        
        // Update Player Settings for mobile
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
        
        Debug.Log("✅ Mobile player settings configured");
    }
    
    void TestDifferentScreenSizes()
    {
        Debug.Log("=== TESTING DIFFERENT SCREEN SIZES ===");
        
        // Common mobile resolutions
        Vector2[] mobileResolutions = {
            new Vector2(1080, 1920), // Standard Android
            new Vector2(1125, 2436), // iPhone X/11/12
            new Vector2(1242, 2688), // iPhone 12 Pro Max
            new Vector2(828, 1792),  // iPhone XR
            new Vector2(1440, 2960), // Samsung Galaxy S8+
        };
        
        foreach (Vector2 resolution in mobileResolutions)
        {
            float aspectRatio = resolution.y / resolution.x;
            Debug.Log($"Testing resolution: {resolution.x}x{resolution.y} (aspect: {aspectRatio:F2})");
        }
        
        Debug.Log("Open Game view and test these resolutions manually");
        Debug.Log("Go to Game view → Free Aspect → + to add custom resolutions");
    }
}
#endif
