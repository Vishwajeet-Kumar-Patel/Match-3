using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class ScreenFitAndBackgroundFix : EditorWindow
{
    [MenuItem("Tools/Screen Fit + Background Fix")]
    public static void ShowWindow()
    {
        GetWindow<ScreenFitAndBackgroundFix>("Screen Fit Fix");
    }
    
    void OnGUI()
    {
        GUILayout.Label("📱 Screen Fit + Background Fix", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🎯 FIX SCREEN FIT + ADD BACKGROUNDS"))
        {
            FixScreenFitAndAddBackgrounds();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📏 Center Grid Only"))
        {
            CenterGridInScreen();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🎨 Add Backgrounds Only"))
        {
            AddTileBackgrounds();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📷 Optimize Camera"))
        {
            OptimizeCameraForGrid();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("This will:", EditorStyles.helpBox);
        GUILayout.Label("• Move grid to center of screen", EditorStyles.helpBox);
        GUILayout.Label("• Ensure all tiles are visible", EditorStyles.helpBox);
        GUILayout.Label("• Add beautiful tile backgrounds", EditorStyles.helpBox);
        GUILayout.Label("• Perfect camera positioning", EditorStyles.helpBox);
    }
    
    void FixScreenFitAndAddBackgrounds()
    {
        Debug.Log("=== FIXING SCREEN FIT & ADDING BACKGROUNDS ===");
        
        // 1. Center the grid perfectly
        CenterGridInScreen();
        
        // 2. Optimize camera to show full grid
        OptimizeCameraForGrid();
        
        // 3. Add beautiful tile backgrounds
        AddTileBackgrounds();
        
        Debug.Log("✅ SCREEN FIT & BACKGROUNDS COMPLETE!");
        Debug.Log("🎮 All tiles should now be visible with backgrounds!");
    }
    
    void CenterGridInScreen()
    {
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent == null)
        {
            Debug.LogWarning("❌ GridParent not found!");
            return;
        }
        
        // Get current spacing from Match3Manager
        float spacing = GetCurrentSpacing();
        
        // Calculate the total grid size
        float gridWidth = 7 * spacing;   // 7 gaps between 8 tiles
        float gridHeight = 7 * spacing;  // 7 gaps between 8 tiles
        
        // Position to center the grid at (0, 0)
        Vector3 centerPosition = new Vector3(-gridWidth / 2f, gridHeight / 2f, 0f);
        gridParent.transform.position = centerPosition;
        
        Debug.Log($"✅ Grid centered at {centerPosition} (grid size: {gridWidth:F1} x {gridHeight:F1})");
    }
    
    float GetCurrentSpacing()
    {
        GameObject managerObj = GameObject.Find("Match3Manager");
        if (managerObj != null)
        {
            Match3Manager manager = managerObj.GetComponent<Match3Manager>();
            if (manager != null)
            {
                var spacingField = typeof(Match3Manager).GetField("tileSpacing", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (spacingField != null)
                {
                    return (float)spacingField.GetValue(manager);
                }
            }
        }
        return 0.9f; // Default fallback
    }
    
    void OptimizeCameraForGrid()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("❌ Main Camera not found!");
            return;
        }
        
        // Get grid dimensions
        float spacing = GetCurrentSpacing();
        float gridWidth = 7 * spacing;
        float gridHeight = 7 * spacing;
        
        // Position camera at center
        mainCamera.transform.position = new Vector3(0, 0, -10f);
        
        // Calculate required camera size to fit the grid with padding
        float padding = 1.0f; // Extra space around grid
        float requiredSize = Mathf.Max(gridWidth, gridHeight) / 2f + padding;
        
        // Clamp to reasonable values
        requiredSize = Mathf.Clamp(requiredSize, 3.5f, 6.0f);
        
        mainCamera.orthographicSize = requiredSize;
        mainCamera.backgroundColor = new Color(0.88f, 0.92f, 0.98f, 1f); // Light blue-gray
        
        Debug.Log($"✅ Camera optimized: Size {requiredSize:F1} to fit grid ({gridWidth:F1} x {gridHeight:F1})");
    }
    
    void AddTileBackgrounds()
    {
        Debug.Log("=== ADDING BEAUTIFUL TILE BACKGROUNDS ===");
        
        // Create background sprite
        Sprite backgroundSprite = CreateTileBackgroundSprite();
        
        // Add backgrounds to all tiles
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
            
            // Create new background
            GameObject background = new GameObject("TileBackground");
            background.transform.SetParent(tile.transform);
            background.transform.localPosition = Vector3.zero;
            background.transform.localScale = Vector3.one * 1.25f; // Slightly larger than tile
            
            // Add sprite renderer
            SpriteRenderer bgRenderer = background.AddComponent<SpriteRenderer>();
            bgRenderer.sprite = backgroundSprite;
            bgRenderer.color = new Color(0.25f, 0.35f, 0.55f, 0.85f); // Nice blue with transparency
            bgRenderer.sortingOrder = -5; // Behind the tile
            
            // Ensure tile sprite is in front
            if (tile.spriteRenderer != null)
            {
                tile.spriteRenderer.sortingOrder = 0;
            }
            
            count++;
        }
        
        Debug.Log($"✅ Added beautiful backgrounds to {count} tiles!");
    }
    
    Sprite CreateTileBackgroundSprite()
    {
        int size = 100;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        
        // Colors for the background
        Color centerColor = Color.white;
        Color borderColor = new Color(0.8f, 0.8f, 0.9f, 1f);
        Color edgeColor = new Color(0.6f, 0.6f, 0.8f, 1f);
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float maxRadius = size / 2f;
        int borderWidth = 4;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                
                // Create rounded rectangle with nice borders
                if (distance > maxRadius - 2)
                {
                    colors[index] = Color.clear; // Transparent edges for rounded corners
                }
                else if (x < borderWidth || x >= size - borderWidth || 
                        y < borderWidth || y >= size - borderWidth)
                {
                    colors[index] = borderColor; // Border
                }
                else if (x < borderWidth * 2 || x >= size - borderWidth * 2 || 
                        y < borderWidth * 2 || y >= size - borderWidth * 2)
                {
                    colors[index] = edgeColor; // Inner border
                }
                else
                {
                    // Center with subtle gradient
                    float gradientFactor = 1f - (distance / maxRadius) * 0.1f;
                    colors[index] = new Color(
                        centerColor.r * gradientFactor,
                        centerColor.g * gradientFactor,
                        centerColor.b * gradientFactor,
                        centerColor.a
                    );
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    void TestGridBounds()
    {
        Debug.Log("=== TESTING GRID BOUNDS ===");
        
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        if (tiles.Length == 0)
        {
            Debug.LogWarning("❌ No tiles found!");
            return;
        }
        
        // Find bounds of all tiles
        Vector3 minPos = tiles[0].transform.position;
        Vector3 maxPos = tiles[0].transform.position;
        
        foreach (Tile tile in tiles)
        {
            Vector3 pos = tile.transform.position;
            minPos = Vector3.Min(minPos, pos);
            maxPos = Vector3.Max(maxPos, pos);
        }
        
        Vector3 gridSize = maxPos - minPos;
        Vector3 gridCenter = (minPos + maxPos) / 2f;
        
        Debug.Log($"Grid bounds: Min({minPos.x:F1}, {minPos.y:F1}) Max({maxPos.x:F1}, {maxPos.y:F1})");
        Debug.Log($"Grid size: {gridSize.x:F1} x {gridSize.y:F1}");
        Debug.Log($"Grid center: ({gridCenter.x:F1}, {gridCenter.y:F1})");
        
        // Check camera bounds
        Camera camera = Camera.main;
        if (camera != null)
        {
            float camHeight = camera.orthographicSize * 2f;
            float camWidth = camHeight * camera.aspect;
            
            Debug.Log($"Camera view: {camWidth:F1} x {camHeight:F1}");
            
            if (gridSize.x > camWidth || gridSize.y > camHeight)
            {
                Debug.LogWarning("⚠️ Grid is larger than camera view!");
                float requiredSize = Mathf.Max(gridSize.x / camera.aspect, gridSize.y) / 2f + 0.5f;
                Debug.Log($"💡 Suggested camera size: {requiredSize:F1}");
            }
            else
            {
                Debug.Log("✅ Grid fits within camera view");
            }
        }
    }
}
#endif
