using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class DarkSquareRemover : EditorWindow
{
    [MenuItem("Tools/Remove Dark Square")]
    public static void ShowWindow()
    {
        GetWindow<DarkSquareRemover>("Remove Dark Square");
    }
    
    void OnGUI()
    {
        GUILayout.Label("🔧 Dark Square Remover", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🎯 Find & Fix Dark Square"))
        {
            FindAndFixDarkSquare();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🔄 Regenerate Problematic Tiles"))
        {
            RegenerateProblematicTiles();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("This will find and fix the dark square in top-left", EditorStyles.helpBox);
    }
    
    void FindAndFixDarkSquare()
    {
        Debug.Log("=== SEARCHING FOR DARK SQUARE ===");
        
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        
        foreach (Tile tile in tiles)
        {
            SpriteRenderer sr = tile.spriteRenderer;
            if (sr != null && sr.sprite != null)
            {
                // Check for dark or problematic sprites
                string spriteName = sr.sprite.name.ToLower();
                
                if (spriteName.Contains("dark") || spriteName.Contains("black") || 
                    sr.color.r < 0.3f && sr.color.g < 0.3f && sr.color.b < 0.3f)
                {
                    Debug.Log($"Found dark tile: {tile.name} at ({tile.row}, {tile.column}) with sprite: {spriteName}");
                    
                    // Replace with a bright sprite
                    ReplaceTileWithBrightSprite(tile);
                }
            }
        }
        
        Debug.Log("✅ Dark square search complete!");
    }
    
    void ReplaceTileWithBrightSprite(Tile tile)
    {
        // Load grid settings to get available sprites
        GridSettings settings = AssetDatabase.LoadAssetAtPath<GridSettings>("Assets/ScriptableObjects/LevelGridSettings.asset");
        
        if (settings != null && settings.tileSprites != null && settings.tileSprites.Length > 0)
        {
            // Choose a bright, colorful sprite (avoid the first one if it's dark)
            Sprite newSprite = null;
            
            // Try to find a leaf, sun, or rainbow sprite (bright ones)
            foreach (Sprite sprite in settings.tileSprites)
            {
                string name = sprite.name.ToLower();
                if (name.Contains("leaf") || name.Contains("sun") || name.Contains("rainbow"))
                {
                    newSprite = sprite;
                    break;
                }
            }
            
            // If no bright sprite found, use any except the first
            if (newSprite == null && settings.tileSprites.Length > 1)
            {
                newSprite = settings.tileSprites[1]; // Use second sprite
            }
            
            if (newSprite != null)
            {
                tile.SetTileType(1, newSprite); // Set to type 1 (usually safe)
                tile.spriteRenderer.color = Color.white; // Ensure full brightness
                
                Debug.Log($"✅ Replaced dark tile with: {newSprite.name}");
            }
        }
    }
    
    void RegenerateProblematicTiles()
    {
        Debug.Log("=== REGENERATING PROBLEMATIC TILES ===");
        
        // Find Match3Manager
        Match3Manager manager = FindFirstObjectByType<Match3Manager>();
        if (manager == null)
        {
            Debug.LogWarning("❌ Match3Manager not found!");
            return;
        }
        
        // Clear and regenerate just the problematic areas
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        
        // Remove tiles in top-left corner that might be problematic
        foreach (Tile tile in tiles)
        {
            if ((tile.row == 0 && tile.column == 0) || // Top-left corner
                (tile.row == 0 && tile.column == 1) || // Next to it
                (tile.row == 1 && tile.column == 0))   // Below it
            {
                Debug.Log($"Removing potentially problematic tile at ({tile.row}, {tile.column})");
                DestroyImmediate(tile.gameObject);
            }
        }
        
        Debug.Log("✅ Problematic tiles removed. Press PLAY to regenerate the grid!");
    }
}
#endif
