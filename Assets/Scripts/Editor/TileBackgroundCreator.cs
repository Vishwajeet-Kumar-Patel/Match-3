using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class TileBackgroundCreator : EditorWindow
{
    [MenuItem("Tools/Create Tile Backgrounds")]
    public static void ShowWindow()
    {
        GetWindow<TileBackgroundCreator>("Tile Background Creator");
    }
    
    private Color tileBackgroundColor = new Color(0.2f, 0.2f, 0.4f, 0.8f);
    private Color tileBorderColor = new Color(0.1f, 0.1f, 0.3f, 1f);
    private float backgroundScale = 1.1f;
    
    void OnGUI()
    {
        GUILayout.Label("🎨 Tile Background Creator", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        tileBackgroundColor = EditorGUILayout.ColorField("Background Color", tileBackgroundColor);
        tileBorderColor = EditorGUILayout.ColorField("Border Color", tileBorderColor);
        backgroundScale = EditorGUILayout.Slider("Background Scale", backgroundScale, 0.8f, 1.5f);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🎯 Create Background Sprites"))
        {
            CreateBackgroundSprites();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🔧 Add Backgrounds to Existing Tiles"))
        {
            AddBackgroundsToExistingTiles();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("This creates tile backgrounds that show behind your elements", EditorStyles.helpBox);
    }
    
    void CreateBackgroundSprites()
    {
        // Create a folder for backgrounds if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Sprites/Backgrounds"))
        {
            AssetDatabase.CreateFolder("Assets/Sprites", "Backgrounds");
        }
        
        // Create background sprite
        Texture2D backgroundTexture = CreateBackgroundTexture();
        
        // Save as asset
        string path = "Assets/Sprites/Backgrounds/TileBackground.png";
        System.IO.File.WriteAllBytes(path, backgroundTexture.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        
        // Configure texture import settings
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 64f;
            importer.filterMode = FilterMode.Point;
            
            // Unity 2025 - Set pivot to center using Vector2
            importer.spritePivot = new Vector2(0.5f, 0.5f);
            
            importer.SaveAndReimport();
        }
        
        Debug.Log("✅ Tile background sprite created at: " + path);
    }
    
    Texture2D CreateBackgroundTexture()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                
                // Create border effect
                bool isBorder = x < 2 || x >= size - 2 || y < 2 || y >= size - 2;
                
                if (isBorder)
                {
                    colors[index] = tileBorderColor;
                }
                else
                {
                    colors[index] = tileBackgroundColor;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return texture;
    }
    
    void AddBackgroundsToExistingTiles()
    {
        // Find all tiles in the scene
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        
        // Load the background sprite
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Backgrounds/TileBackground.png");
        if (backgroundSprite == null)
        {
            Debug.LogWarning("❌ Background sprite not found! Create it first.");
            return;
        }
        
        int count = 0;
        foreach (Tile tile in tiles)
        {
            // Check if background already exists
            Transform existingBG = tile.transform.Find("TileBackground");
            if (existingBG == null)
            {
                // Create background GameObject
                GameObject background = new GameObject("TileBackground");
                background.transform.SetParent(tile.transform);
                background.transform.localPosition = Vector3.zero;
                background.transform.localScale = Vector3.one * backgroundScale;
                
                // Add SpriteRenderer
                SpriteRenderer bgRenderer = background.AddComponent<SpriteRenderer>();
                bgRenderer.sprite = backgroundSprite;
                bgRenderer.color = tileBackgroundColor;
                bgRenderer.sortingOrder = -1; // Behind the main tile
                
                // Ensure main tile is in front
                if (tile.spriteRenderer != null)
                {
                    tile.spriteRenderer.sortingOrder = 0;
                }
                
                count++;
            }
        }
        
        Debug.Log($"✅ Added backgrounds to {count} tiles");
    }
}
#endif
