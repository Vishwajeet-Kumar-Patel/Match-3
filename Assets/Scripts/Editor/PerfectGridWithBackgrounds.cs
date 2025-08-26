using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class PerfectGridWithBackgrounds : EditorWindow
{
    [MenuItem("Tools/Perfect Grid with Backgrounds")]
    public static void ShowWindow()
    {
        GetWindow<PerfectGridWithBackgrounds>("Perfect Grid Fix");
    }
    
    [Header("Grid Settings")]
    private float perfectScale = 0.7f;
    private float perfectSpacing = 0.8f;
    private float cameraSize = 4.2f;
    private Vector3 gridPosition = new Vector3(-2.8f, 2.8f, 0f);
    
    [Header("Background Settings")]
    private Color tileBackgroundColor = new Color(0.2f, 0.25f, 0.4f, 0.9f);
    private Color tileBorderColor = new Color(0.15f, 0.2f, 0.35f, 1f);
    private float backgroundScale = 1.2f;
    
    void OnGUI()
    {
        GUILayout.Label("🎯 Perfect Grid with Backgrounds", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        GUILayout.Label("Grid Settings:", EditorStyles.boldLabel);
        perfectScale = EditorGUILayout.Slider("Tile Scale", perfectScale, 0.4f, 1.0f);
        perfectSpacing = EditorGUILayout.Slider("Tile Spacing", perfectSpacing, 0.5f, 1.2f);
        cameraSize = EditorGUILayout.Slider("Camera Size", cameraSize, 3.0f, 6.0f);
        
        GUILayout.Space(5);
        
        GUILayout.Label("Background Settings:", EditorStyles.boldLabel);
        tileBackgroundColor = EditorGUILayout.ColorField("Background Color", tileBackgroundColor);
        tileBorderColor = EditorGUILayout.ColorField("Border Color", tileBorderColor);
        backgroundScale = EditorGUILayout.Slider("Background Scale", backgroundScale, 1.0f, 1.5f);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("✨ Apply Perfect Fix"))
        {
            ApplyPerfectFix();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🎯 Quick Perfect Setup"))
        {
            QuickPerfectSetup();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🎨 Add Tile Backgrounds"))
        {
            CreateAndAddTileBackgrounds();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🔄 Clear & Regenerate"))
        {
            ClearAndRegenerate();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("This will create:", EditorStyles.helpBox);
        GUILayout.Label("• Perfect sized 8x8 grid", EditorStyles.helpBox);
        GUILayout.Label("• Beautiful tile backgrounds", EditorStyles.helpBox);
        GUILayout.Label("• Centered, screen-fitted layout", EditorStyles.helpBox);
        GUILayout.Label("• Professional game appearance", EditorStyles.helpBox);
    }
    
    void QuickPerfectSetup()
    {
        Debug.Log("=== APPLYING QUICK PERFECT SETUP ===");
        
        // Set optimal values for great appearance
        perfectScale = 0.75f;
        perfectSpacing = 0.85f;
        cameraSize = 4.5f;
        gridPosition = new Vector3(-2.975f, 2.975f, 0f);
        backgroundScale = 1.15f;
        
        ApplyPerfectFix();
        
        Debug.Log("✅ Quick perfect setup applied!");
    }
    
    void ApplyPerfectFix()
    {
        Debug.Log("=== APPLYING PERFECT GRID FIX ===");
        
        // 1. Reset any mobile scaling
        ResetMobileScaling();
        
        // 2. Set perfect grid settings
        SetPerfectGridSettings();
        
        // 3. Configure perfect camera
        ConfigurePerfectCamera();
        
        // 4. Create and add tile backgrounds
        CreateAndAddTileBackgrounds();
        
        // 5. Position grid perfectly
        PositionGridPerfectly();
        
        Debug.Log("=== PERFECT GRID COMPLETE ===");
        Debug.Log("🎮 Press PLAY to see your beautiful Match3 grid!");
    }
    
    void ResetMobileScaling()
    {
        // Reset GridParent scaling that was set by mobile optimizer
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
        {
            gridParent.transform.localScale = Vector3.one; // Reset to normal scale
            Debug.Log("✅ Reset mobile scaling to normal");
        }
        
        // Remove mobile components that might interfere
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            MobileCameraController mobileController = mainCamera.GetComponent<MobileCameraController>();
            if (mobileController != null)
            {
                DestroyImmediate(mobileController);
                Debug.Log("✅ Removed mobile camera controller");
            }
        }
    }
    
    void SetPerfectGridSettings()
    {
        // Update Match3Manager settings
        GameObject managerObj = GameObject.Find("Match3Manager");
        if (managerObj != null)
        {
            Match3Manager manager = managerObj.GetComponent<Match3Manager>();
            if (manager != null)
            {
                // Update tile spacing
                var spacingField = typeof(Match3Manager).GetField("tileSpacing", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (spacingField != null)
                {
                    spacingField.SetValue(manager, perfectSpacing);
                }
                
                EditorUtility.SetDirty(manager);
                Debug.Log($"✅ Match3Manager spacing set to {perfectSpacing}");
            }
        }
        
        // Set all tiles to perfect scale
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        foreach (Tile tile in tiles)
        {
            tile.transform.localScale = Vector3.one * perfectScale;
        }
        
        Debug.Log($"✅ {tiles.Length} tiles scaled to {perfectScale}");
    }
    
    void ConfigurePerfectCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;
        
        // Perfect camera settings for beautiful view
        mainCamera.transform.position = new Vector3(0, 0, -10f);
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = cameraSize;
        mainCamera.backgroundColor = new Color(0.9f, 0.95f, 1f, 1f); // Light blue background
        
        Debug.Log($"✅ Camera configured: Size {cameraSize}, centered view");
    }
    
    void CreateAndAddTileBackgrounds()
    {
        Debug.Log("=== CREATING TILE BACKGROUNDS ===");
        
        // Create background sprite
        Sprite backgroundSprite = CreateBeautifulBackgroundSprite();
        
        // Add backgrounds to all tiles
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        int count = 0;
        
        foreach (Tile tile in tiles)
        {
            // Remove existing background
            Transform existingBG = tile.transform.Find("TileBackground");
            if (existingBG != null)
            {
                DestroyImmediate(existingBG.gameObject);
            }
            
            // Create new beautiful background
            GameObject background = new GameObject("TileBackground");
            background.transform.SetParent(tile.transform);
            background.transform.localPosition = Vector3.zero;
            background.transform.localScale = Vector3.one * backgroundScale;
            
            // Add sprite renderer with beautiful background
            SpriteRenderer bgRenderer = background.AddComponent<SpriteRenderer>();
            bgRenderer.sprite = backgroundSprite;
            bgRenderer.color = tileBackgroundColor;
            bgRenderer.sortingOrder = -5; // Behind the tile element
            
            // Ensure tile element is in front
            if (tile.spriteRenderer != null)
            {
                tile.spriteRenderer.sortingOrder = 0;
            }
            
            count++;
        }
        
        Debug.Log($"✅ Added beautiful backgrounds to {count} tiles!");
    }
    
    Sprite CreateBeautifulBackgroundSprite()
    {
        int size = 128;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float maxRadius = size * 0.45f;
        int borderWidth = 6;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                // Create rounded rectangle with gradient
                bool isInBounds = (x >= borderWidth && x < size - borderWidth && 
                                 y >= borderWidth && y < size - borderWidth) ||
                                 distance <= maxRadius;
                
                if (!isInBounds)
                {
                    colors[index] = Color.clear; // Transparent outside
                }
                else if (x < borderWidth || x >= size - borderWidth || 
                        y < borderWidth || y >= size - borderWidth ||
                        distance > maxRadius - borderWidth)
                {
                    colors[index] = tileBorderColor; // Border
                }
                else
                {
                    // Create subtle gradient from center
                    float gradientFactor = 1f - (distance / maxRadius) * 0.2f;
                    Color baseColor = tileBackgroundColor;
                    colors[index] = new Color(
                        baseColor.r * gradientFactor,
                        baseColor.g * gradientFactor,
                        baseColor.b * gradientFactor,
                        baseColor.a
                    );
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    void PositionGridPerfectly()
    {
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent == null) return;
        
        // Calculate perfect position to center the 8x8 grid
        float gridWidth = 7 * perfectSpacing;  // 7 spaces between 8 tiles
        float gridHeight = 7 * perfectSpacing;
        
        Vector3 centerOffset = new Vector3(-gridWidth / 2f, gridHeight / 2f, 0f);
        gridParent.transform.position = centerOffset;
        
        Debug.Log($"✅ Grid positioned perfectly at {centerOffset}");
    }
    
    void ClearAndRegenerate()
    {
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
        {
            int childCount = gridParent.transform.childCount;
            
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(gridParent.transform.GetChild(i).gameObject);
            }
            
            Debug.Log($"✅ Cleared {childCount} tiles");
        }
        
        Debug.Log("🎮 Press PLAY to generate new perfect grid with backgrounds!");
    }
}
#endif
