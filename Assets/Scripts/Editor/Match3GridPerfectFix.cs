using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

#if UNITY_EDITOR
public class Match3GridPerfectFix : EditorWindow
{
    [MenuItem("Tools/Perfect Grid Setup")]
    public static void ShowWindow()
    {
        GetWindow<Match3GridPerfectFix>("Perfect Grid Setup");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Perfect Match3 Grid Setup", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🎯 Perfect Grid Positioning"))
        {
            SetupPerfectGrid();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📷 Optimize Camera for 8x8 Grid"))
        {
            OptimizeCamera();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🔄 Clear and Regenerate Grid"))
        {
            ClearAndRegenerate();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("✨ Complete Setup + Test"))
        {
            CompleteSetupAndTest();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("This will fix:", EditorStyles.helpBox);
        GUILayout.Label("• Grid positioning (center on screen)", EditorStyles.helpBox);
        GUILayout.Label("• Camera framing for perfect 8x8 view", EditorStyles.helpBox);
        GUILayout.Label("• Ensure Match3Manager is working", EditorStyles.helpBox);
        GUILayout.Label("• Add missing UI elements", EditorStyles.helpBox);
    }
    
    void SetupPerfectGrid()
    {
        Debug.Log("=== SETTING UP PERFECT GRID ===");
        
        // 1. Position GridParent correctly
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
        {
            // For 8x8 grid with 0.8 spacing, center should be at (2.8, -2.8)
            gridParent.transform.position = new Vector3(-0.35f, 0.35f, 0);
            Debug.Log("✅ GridParent positioned for centered 8x8 grid");
        }
        
        // 2. Optimize camera
        OptimizeCamera();
        
        // 3. Configure Match3Manager spacing
        ConfigureMatch3Manager();
        
        Debug.Log("✅ Perfect grid setup complete!");
    }
    
    void OptimizeCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Perfect camera settings for 8x8 grid
            mainCamera.transform.position = new Vector3(2.8f, -2.8f, -10f);
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 4.5f;
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.3f); // Nice dark blue
            
            Debug.Log("✅ Camera optimized: Position (2.8, -2.8, -10), Size 4.5");
        }
    }
    
    void ConfigureMatch3Manager()
    {
        GameObject match3ManagerObj = GameObject.Find("Match3Manager");
        if (match3ManagerObj != null)
        {
            Match3Manager manager = match3ManagerObj.GetComponent<Match3Manager>();
            if (manager != null)
            {
                // Set optimal tile spacing
                var spacingField = typeof(Match3Manager).GetField("tileSpacing", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (spacingField != null)
                {
                    spacingField.SetValue(manager, 0.8f);
                    Debug.Log("✅ Match3Manager tile spacing set to 0.8");
                }
                
                Debug.Log("✅ Match3Manager configured");
            }
            else
            {
                Debug.LogWarning("⚠️ Match3Manager script not found on GameObject!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Match3Manager GameObject not found!");
        }
    }
    
    void ClearAndRegenerate()
    {
        // Clear existing tiles
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
        {
            int childCount = gridParent.transform.childCount;
            
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(gridParent.transform.GetChild(i).gameObject);
            }
            
            Debug.Log($"✅ Cleared {childCount} tiles for regeneration");
        }
    }
    
    void CompleteSetupAndTest()
    {
        Debug.Log("=== COMPLETE SETUP AND TEST ===");
        
        // 1. Perfect grid setup
        SetupPerfectGrid();
        
        // 2. Clear old tiles
        ClearAndRegenerate();
        
        // 3. Create UI if missing
        CreateBasicUI();
        
        // 4. Verify all components
        VerifySetup();
        
        Debug.Log("=== SETUP COMPLETE ===");
        Debug.Log("🎮 Press PLAY to test your Match3 game!");
        Debug.Log("Expected: Centered 8x8 grid with your sprites");
    }
    
    void CreateBasicUI()
    {
        // Create Canvas if missing
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            Debug.Log("✅ Created Canvas");
        }
        
        // Create EventSystem if missing
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            Debug.Log("✅ Created EventSystem");
        }
        
        // Create basic UI text
        CreateUIText(canvas.transform, "ScoreText", "Score: 0", TextAnchor.UpperLeft, new Vector2(-800, 450));
        CreateUIText(canvas.transform, "MovesText", "Moves: 30", TextAnchor.UpperRight, new Vector2(800, 450));
    }
    
    void CreateUIText(Transform parent, string name, string text, TextAnchor alignment, Vector2 position)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return;
        
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(300, 50);
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 32;
        textComponent.color = Color.white;
        textComponent.alignment = alignment;
    }
    
    void VerifySetup()
    {
        Debug.Log("=== VERIFICATION ===");
        
        // Check GridParent
        GameObject gridParent = GameObject.Find("GridParent");
        Debug.Log(gridParent != null ? "✅ GridParent exists" : "❌ GridParent missing");
        
        // Check Match3Manager
        GameObject match3Manager = GameObject.Find("Match3Manager");
        bool hasScript = match3Manager != null && match3Manager.GetComponent<Match3Manager>() != null;
        Debug.Log(hasScript ? "✅ Match3Manager with script exists" : "❌ Match3Manager script missing");
        
        // Check TilePrefab
        GameObject tilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/TilePrefab.prefab");
        Debug.Log(tilePrefab != null ? "✅ TilePrefab exists" : "❌ TilePrefab missing");
        
        // Check GridSettings
        GridSettings gridSettings = AssetDatabase.LoadAssetAtPath<GridSettings>("Assets/ScriptableObjects/LevelGridSettings.asset");
        Debug.Log(gridSettings != null ? "✅ GridSettings exists" : "❌ GridSettings missing");
        
        // Check Camera
        Camera camera = Camera.main;
        Debug.Log(camera != null ? "✅ Main Camera exists" : "❌ Main Camera missing");
        
        if (hasScript && tilePrefab != null && gridSettings != null)
        {
            Debug.Log("🎉 ALL SYSTEMS READY! Press Play to test!");
        }
        else
        {
            Debug.Log("⚠️ Some components missing. Check the logs above.");
        }
    }
}
#endif
