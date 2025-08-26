using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class GridSizeAndBackgroundFixer : EditorWindow
{
    [MenuItem("Tools/Grid Size & Background Fix")]
    public static void ShowWindow()
    {
        GetWindow<GridSizeAndBackgroundFixer>("Grid & Background Fix");
    }
    
    private float tileSize = 0.6f;
    private float tileSpacing = 0.7f;
    private float cameraSize = 3.5f;
    private Color backgroundColor = new Color(0.8f, 0.8f, 0.9f, 1f);
    private Color tileBackgroundColor = new Color(0.3f, 0.3f, 0.5f, 1f);
    
    void OnGUI()
    {
        GUILayout.Label("🎯 Grid Size & Background Fix", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        GUILayout.Label("Tile Settings:", EditorStyles.boldLabel);
        tileSize = EditorGUILayout.Slider("Tile Size", tileSize, 0.3f, 1.0f);
        tileSpacing = EditorGUILayout.Slider("Tile Spacing", tileSpacing, 0.4f, 1.0f);
        
        GUILayout.Space(5);
        
        GUILayout.Label("Camera Settings:", EditorStyles.boldLabel);
        cameraSize = EditorGUILayout.Slider("Camera Size", cameraSize, 2.0f, 6.0f);
        
        GUILayout.Space(5);
        
        GUILayout.Label("Colors:", EditorStyles.boldLabel);
        backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
        tileBackgroundColor = EditorGUILayout.ColorField("Tile Background", tileBackgroundColor);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🔧 Apply Size & Background Fix"))
        {
            ApplySizeAndBackgroundFix();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🎯 Quick Small Grid (Recommended)"))
        {
            QuickSmallGridFix();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🔄 Clear & Regenerate"))
        {
            ClearAndRegenerate();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("This will:", EditorStyles.helpBox);
        GUILayout.Label("• Make tiles smaller and tighter", EditorStyles.helpBox);
        GUILayout.Label("• Add tile backgrounds", EditorStyles.helpBox);
        GUILayout.Label("• Put elements above backgrounds", EditorStyles.helpBox);
        GUILayout.Label("• Optimize camera for smaller grid", EditorStyles.helpBox);
    }
    
    void QuickSmallGridFix()
    {
        Debug.Log("=== APPLYING QUICK SMALL GRID FIX ===");
        
        // Set optimal small values
        tileSize = 0.5f;
        tileSpacing = 0.6f;
        cameraSize = 3.2f;
        
        ApplySizeAndBackgroundFix();
        
        Debug.Log("✅ Quick small grid applied!");
    }
    
    void ApplySizeAndBackgroundFix()
    {
        Debug.Log("=== APPLYING SIZE & BACKGROUND FIX ===");
        
        // 1. Fix camera for smaller grid
        FixCameraForSmallerGrid();
        
        // 2. Update tile prefab with background
        CreateTileWithBackground();
        
        // 3. Update Match3Manager settings
        UpdateMatch3ManagerSettings();
        
        // 4. Adjust grid positioning
        AdjustGridPosition();
        
        Debug.Log("=== FIX COMPLETE ===");
        Debug.Log("🎮 Clear grid and press PLAY to see smaller tiles with backgrounds!");
    }
    
    void FixCameraForSmallerGrid()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Calculate grid bounds for 8x8 with new spacing
            float gridWidth = 7 * tileSpacing; // 7 spaces between 8 tiles
            float gridHeight = 7 * tileSpacing;
            
            // Center camera on grid center
            float centerX = gridWidth / 2f;
            float centerY = -gridHeight / 2f;
            
            mainCamera.transform.position = new Vector3(centerX, centerY, -10f);
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = cameraSize;
            mainCamera.backgroundColor = backgroundColor;
            
            Debug.Log($"✅ Camera optimized: Position ({centerX:F1}, {centerY:F1}, -10), Size {cameraSize}");
        }
    }
    
    void CreateTileWithBackground()
    {
        // Load existing tile prefab
        GameObject tilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/TilePrefab.prefab");
        if (tilePrefab == null)
        {
            Debug.LogWarning("❌ TilePrefab not found!");
            return;
        }
        
        // Create a new improved prefab
        GameObject newTile = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
        if (newTile == null) return;
        
        // 1. Scale the main tile
        newTile.transform.localScale = new Vector3(tileSize, tileSize, 1f);
        
        // 2. Create background tile
        GameObject background = new GameObject("TileBackground");
        background.transform.SetParent(newTile.transform);
        background.transform.localPosition = Vector3.zero;
        background.transform.localScale = Vector3.one;
        
        // Add background sprite renderer
        SpriteRenderer bgRenderer = background.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = CreateTileBackgroundSprite();
        bgRenderer.color = tileBackgroundColor;
        bgRenderer.sortingOrder = -1; // Behind the main sprite
        
        // 3. Ensure main sprite is above background
        SpriteRenderer mainRenderer = newTile.GetComponent<SpriteRenderer>();
        if (mainRenderer != null)
        {
            mainRenderer.sortingOrder = 0; // Above background
        }
        
        // 4. Save the updated prefab
        PrefabUtility.SaveAsPrefabAsset(newTile, "Assets/Prefabs/TilePrefab.prefab");
        DestroyImmediate(newTile);
        
        Debug.Log($"✅ Tile prefab updated with background and size {tileSize}");
    }
    
    Sprite CreateTileBackgroundSprite()
    {
        // Create a simple square texture for tile background
        Texture2D texture = new Texture2D(64, 64);
        Color[] colors = new Color[64 * 64];
        
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }
    
    void UpdateMatch3ManagerSettings()
    {
        GameObject match3ManagerObj = GameObject.Find("Match3Manager");
        if (match3ManagerObj != null)
        {
            Match3Manager manager = match3ManagerObj.GetComponent<Match3Manager>();
            if (manager != null)
            {
                // Use reflection to update private fields
                var spacingField = typeof(Match3Manager).GetField("tileSpacing", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (spacingField != null)
                {
                    spacingField.SetValue(manager, tileSpacing);
                    Debug.Log($"✅ Match3Manager spacing updated to {tileSpacing}");
                }
                
                EditorUtility.SetDirty(manager);
            }
        }
    }
    
    void AdjustGridPosition()
    {
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
        {
            // Calculate position to center the smaller grid
            float offsetX = -(7 * tileSpacing) / 2f;
            float offsetY = (7 * tileSpacing) / 2f;
            
            gridParent.transform.position = new Vector3(offsetX, offsetY, 0);
            
            Debug.Log($"✅ GridParent positioned at ({offsetX:F1}, {offsetY:F1}, 0)");
        }
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
            
            Debug.Log($"✅ Cleared {childCount} tiles for regeneration");
        }
        
        Debug.Log("🎮 Press PLAY to generate new smaller grid with backgrounds!");
    }
}
#endif
