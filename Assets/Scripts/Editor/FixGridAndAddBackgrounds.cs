using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class FixGridAndAddBackgrounds : EditorWindow
{
    [MenuItem("Tools/Fix Grid + Add Backgrounds")]
    public static void ShowWindow()
    {
        GetWindow<FixGridAndAddBackgrounds>("Fix Grid & BG");
    }
    
    void OnGUI()
    {
        GUILayout.Label("🔧 Fix Grid & Add Backgrounds", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🎯 COMPLETE FIX (Recommended)"))
        {
            CompleteFix();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📏 Fix Size & Position Only"))
        {
            FixSizeAndPosition();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🎨 Add Tile Backgrounds Only"))
        {
            AddTileBackgrounds();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🔄 Clear & Regenerate Grid"))
        {
            ClearGrid();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("Complete Fix will:", EditorStyles.helpBox);
        GUILayout.Label("• Make tiles proper size (0.8 scale)", EditorStyles.helpBox);
        GUILayout.Label("• Center grid on screen", EditorStyles.helpBox);
        GUILayout.Label("• Add beautiful tile backgrounds", EditorStyles.helpBox);
        GUILayout.Label("• Set perfect camera view", EditorStyles.helpBox);
    }
    
    void CompleteFix()
    {
        Debug.Log("=== COMPLETE GRID FIX ===");
        
        // 1. Fix the tiny scale issue
        FixSizeAndPosition();
        
        // 2. Add beautiful backgrounds
        AddTileBackgrounds();
        
        Debug.Log("✅ COMPLETE FIX APPLIED!");
        Debug.Log("🎮 Your grid should now look perfect!");
    }
    
    void FixSizeAndPosition()
    {
        Debug.Log("=== FIXING SIZE & POSITION ===");
        
        // 1. Reset GridParent scale (fix mobile optimizer issue)
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
        {
            gridParent.transform.localScale = Vector3.one; // Normal scale
            Debug.Log("✅ Reset GridParent scale to normal");
        }
        
        // 2. Set tiles to proper size
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        foreach (Tile tile in tiles)
        {
            tile.transform.localScale = new Vector3(0.8f, 0.8f, 1f); // Good size
        }
        Debug.Log($"✅ Set {tiles.Length} tiles to proper size (0.8 scale)");
        
        // 3. Update Match3Manager spacing
        UpdateSpacing(0.9f);
        
        // 4. Fix camera
        FixCamera();
        
        // 5. Center grid
        CenterGrid();
        
        Debug.Log("✅ Size & position fixed!");
    }
    
    void UpdateSpacing(float spacing)
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
                    spacingField.SetValue(manager, spacing);
                    EditorUtility.SetDirty(manager);
                    Debug.Log($"✅ Spacing updated to {spacing}");
                }
            }
        }
    }
    
    void FixCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0, 0, -10f);
            mainCamera.orthographicSize = 4.5f;
            mainCamera.backgroundColor = new Color(0.85f, 0.9f, 0.95f, 1f);
            
            // Remove mobile controller if exists
            MobileCameraController mobile = mainCamera.GetComponent<MobileCameraController>();
            if (mobile != null)
            {
                DestroyImmediate(mobile);
            }
            
            Debug.Log("✅ Camera fixed and centered");
        }
    }
    
    void CenterGrid()
    {
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
        {
            // Center for 8x8 grid with 0.9 spacing
            float offset = (7 * 0.9f) / 2f; // 7 gaps between 8 tiles
            gridParent.transform.position = new Vector3(-offset, offset, 0f);
            Debug.Log($"✅ Grid centered at ({-offset:F1}, {offset:F1}, 0)");
        }
    }
    
    void AddTileBackgrounds()
    {
        Debug.Log("=== ADDING TILE BACKGROUNDS ===");
        
        // Create background texture
        Texture2D bgTexture = CreateBackgroundTexture();
        Sprite bgSprite = Sprite.Create(bgTexture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        
        // Add to all tiles
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
            
            // Create new background
            GameObject background = new GameObject("TileBackground");
            background.transform.SetParent(tile.transform);
            background.transform.localPosition = Vector3.zero;
            background.transform.localScale = Vector3.one * 1.3f; // Slightly larger
            
            // Add sprite renderer
            SpriteRenderer bgRenderer = background.AddComponent<SpriteRenderer>();
            bgRenderer.sprite = bgSprite;
            bgRenderer.color = new Color(0.2f, 0.3f, 0.5f, 0.8f); // Nice blue
            bgRenderer.sortingOrder = -10; // Behind everything
            
            // Ensure tile is in front
            if (tile.spriteRenderer != null)
            {
                tile.spriteRenderer.sortingOrder = 0;
            }
            
            count++;
        }
        
        Debug.Log($"✅ Added backgrounds to {count} tiles!");
    }
    
    Texture2D CreateBackgroundTexture()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        
        Color bgColor = Color.white;
        Color borderColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                
                // Create border
                bool isBorder = x < 3 || x >= size - 3 || y < 3 || y >= size - 3;
                
                if (isBorder)
                {
                    colors[index] = borderColor;
                }
                else
                {
                    colors[index] = bgColor;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return texture;
    }
    
    void ClearGrid()
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
        
        Debug.Log("🎮 Press PLAY to regenerate grid with proper settings!");
    }
}
#endif
