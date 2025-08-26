using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class PerfectTileBackgroundFix : EditorWindow
{
    [MenuItem("Tools/Perfect Tile Background Fix")]
    public static void ShowWindow()
    {
        GetWindow<PerfectTileBackgroundFix>("Perfect Tile BG Fix");
    }
    
    private Color backgroundColor = new Color(0.15f, 0.15f, 0.25f, 1f);
    private Color borderColor = new Color(0.1f, 0.1f, 0.2f, 1f);
    private float borderWidth = 0.05f;
    private bool removeTopLeftTile = true;
    
    void OnGUI()
    {
        GUILayout.Label("🎯 Perfect Tile Background Fix", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        backgroundColor = EditorGUILayout.ColorField("Tile Background", backgroundColor);
        borderColor = EditorGUILayout.ColorField("Border Color", borderColor);
        borderWidth = EditorGUILayout.Slider("Border Width", borderWidth, 0.02f, 0.1f);
        removeTopLeftTile = EditorGUILayout.Toggle("Remove Top-Left Dark Square", removeTopLeftTile);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("✨ Apply Perfect Background Fix"))
        {
            ApplyPerfectBackgroundFix();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🎯 Quick Fix All Issues"))
        {
            QuickFixAllIssues();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📏 Perfect Screen Fit"))
        {
            PerfectScreenFit();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("This will:", EditorStyles.helpBox);
        GUILayout.Label("• Add backgrounds to ALL tiles", EditorStyles.helpBox);
        GUILayout.Label("• Create beautiful borders", EditorStyles.helpBox);
        GUILayout.Label("• Remove the dark top-left square", EditorStyles.helpBox);
        GUILayout.Label("• Perfect screen positioning", EditorStyles.helpBox);
    }
    
    void QuickFixAllIssues()
    {
        Debug.Log("=== APPLYING COMPLETE FIX ===");
        
        // 1. Create background sprite
        CreatePerfectBackgroundSprite();
        
        // 2. Add backgrounds to all tiles
        AddBackgroundsToAllTiles();
        
        // 3. Remove the problematic top-left tile
        if (removeTopLeftTile)
        {
            RemoveTopLeftDarkSquare();
        }
        
        // 4. Perfect screen fit
        PerfectScreenFit();
        
        Debug.Log("✅ ALL ISSUES FIXED!");
        Debug.Log("🎮 Your grid should now look perfect!");
    }
    
    void ApplyPerfectBackgroundFix()
    {
        CreatePerfectBackgroundSprite();
        AddBackgroundsToAllTiles();
        
        if (removeTopLeftTile)
        {
            RemoveTopLeftDarkSquare();
        }
    }
    
    void CreatePerfectBackgroundSprite()
    {
        // Create folder if needed
        if (!AssetDatabase.IsValidFolder("Assets/Sprites"))
        {
            AssetDatabase.CreateFolder("Assets", "Sprites");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Sprites/UI"))
        {
            AssetDatabase.CreateFolder("Assets/Sprites", "UI");
        }
        
        // Create the background texture
        Texture2D bgTexture = CreateTileBackgroundTexture();
        
        // Save as PNG
        string path = "Assets/Sprites/UI/PerfectTileBackground.png";
        System.IO.File.WriteAllBytes(path, bgTexture.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        
        // Configure import settings
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePivot = new Vector2(0.5f, 0.5f);
            importer.spritePixelsPerUnit = 100f;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
        }
        
        Debug.Log("✅ Perfect tile background sprite created!");
    }
    
    Texture2D CreateTileBackgroundTexture()
    {
        int size = 100;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        
        int borderPixels = Mathf.RoundToInt(borderWidth * size);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                
                // Create rounded corners and border
                bool isBorder = x < borderPixels || x >= size - borderPixels || 
                              y < borderPixels || y >= size - borderPixels;
                
                // Create rounded corners
                float centerX = size / 2f;
                float centerY = size / 2f;
                float maxDistance = size / 2f - borderPixels;
                float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                
                if (distance > maxDistance)
                {
                    colors[index] = borderColor;
                }
                else if (isBorder)
                {
                    colors[index] = borderColor;
                }
                else
                {
                    colors[index] = backgroundColor;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return texture;
    }
    
    void AddBackgroundsToAllTiles()
    {
        // Load the background sprite
        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/PerfectTileBackground.png");
        if (bgSprite == null)
        {
            Debug.LogError("❌ Background sprite not found! Create it first.");
            return;
        }
        
        // Find all tiles in the scene
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        
        int count = 0;
        foreach (Tile tile in tiles)
        {
            // Remove existing background if any
            Transform existingBG = tile.transform.Find("TileBackground");
            if (existingBG != null)
            {
                DestroyImmediate(existingBG.gameObject);
            }
            
            // Create new perfect background
            GameObject background = new GameObject("TileBackground");
            background.transform.SetParent(tile.transform);
            background.transform.localPosition = Vector3.zero;
            background.transform.localScale = Vector3.one * 1.1f; // Slightly larger than tile
            
            // Add sprite renderer
            SpriteRenderer bgRenderer = background.AddComponent<SpriteRenderer>();
            bgRenderer.sprite = bgSprite;
            bgRenderer.sortingOrder = -10; // Far behind everything
            
            // Ensure main tile sprite is in front
            if (tile.spriteRenderer != null)
            {
                tile.spriteRenderer.sortingOrder = 0;
            }
            
            count++;
        }
        
        Debug.Log($"✅ Added perfect backgrounds to {count} tiles!");
    }
    
    void RemoveTopLeftDarkSquare()
    {
        // Find the tile at position (0,0) or any problematic tiles
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        
        foreach (Tile tile in tiles)
        {
            // Check if this is the problematic top-left tile
            if (tile.row == 0 && tile.column == 0)
            {
                // Check if it has a dark/problematic sprite
                SpriteRenderer sr = tile.spriteRenderer;
                if (sr != null && sr.sprite != null)
                {
                    Debug.Log($"Found top-left tile: {tile.name} with sprite: {sr.sprite.name}");
                    
                    // You could either:
                    // 1. Replace with a different sprite
                    // 2. Remove the tile entirely
                    // 3. Move it to a different position
                    
                    // For now, let's regenerate this tile with a random sprite
                    if (Match3Manager.Instance != null)
                    {
                        // Force regenerate this specific tile
                        tile.SetTileType(Random.Range(0, 5), GetRandomSprite());
                        Debug.Log("✅ Fixed top-left dark square!");
                    }
                }
            }
        }
    }
    
    Sprite GetRandomSprite()
    {
        // Try to get sprites from GridSettings
        GridSettings settings = AssetDatabase.LoadAssetAtPath<GridSettings>("Assets/ScriptableObjects/LevelGridSettings.asset");
        if (settings != null && settings.tileSprites != null && settings.tileSprites.Length > 0)
        {
            return settings.tileSprites[Random.Range(0, settings.tileSprites.Length)];
        }
        
        return null;
    }
    
    void PerfectScreenFit()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;
        
        // Calculate perfect camera settings for your current grid
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent == null) return;
        
        // Get grid bounds
        Renderer[] renderers = gridParent.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;
        
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        
        // Position camera to center on grid
        Vector3 center = bounds.center;
        mainCamera.transform.position = new Vector3(center.x, center.y, -10f);
        
        // Set camera size to fit grid with some padding
        float padding = 1.5f;
        float requiredSize = Mathf.Max(bounds.size.x, bounds.size.y) / 2f + padding;
        mainCamera.orthographicSize = requiredSize;
        
        // Set a nice background color
        mainCamera.backgroundColor = new Color(0.85f, 0.85f, 0.95f, 1f); // Light blue-gray
        
        Debug.Log($"✅ Camera positioned perfectly: Center({center.x:F1}, {center.y:F1}), Size: {requiredSize:F1}");
    }
}
#endif
