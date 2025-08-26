using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class TileSpriteGenerator : EditorWindow
{
    private int spriteSize = 256;
    
    // Updated to match your existing prefabs
    private string[] existingPrefabNames = new string[]
    {
        "leaf",
        "rainbow", 
        "rock",
        "sun",
        "tile"
    };
    
    [MenuItem("Tools/Generate Tile Sprites")]
    public static void ShowWindow()
    {
        GetWindow<TileSpriteGenerator>("Tile Sprite Generator");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Tile Sprite Generator", EditorStyles.boldLabel);
        
        spriteSize = EditorGUILayout.IntField("Sprite Size:", spriteSize);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create Tile Prefab from Existing"))
        {
            CreateTilePrefabFromExisting();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Generate Additional Tile Sprites"))
        {
            GenerateAdditionalSprites();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("Existing prefabs found:", EditorStyles.boldLabel);
        foreach (string prefabName in existingPrefabNames)
        {
            string path = $"Assets/Prefabs/{prefabName}.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            {
                GUILayout.Label($"✓ {prefabName}", EditorStyles.helpBox);
            }
            else
            {
                GUILayout.Label($"✗ {prefabName} (not found)", EditorStyles.helpBox);
            }
        }
        
        GUILayout.Space(10);
        GUILayout.Label("This will create a TilePrefab using your existing prefab sprites.", EditorStyles.helpBox);
    }
    
    void CreateTilePrefabFromExisting()
    {
        // Check if we have existing prefabs
        GameObject[] existingPrefabs = new GameObject[existingPrefabNames.Length];
        Sprite[] tileSprites = new Sprite[existingPrefabNames.Length];
        
        for (int i = 0; i < existingPrefabNames.Length; i++)
        {
            string path = $"Assets/Prefabs/{existingPrefabNames[i]}.prefab";
            existingPrefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (existingPrefabs[i] != null)
            {
                SpriteRenderer sr = existingPrefabs[i].GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    tileSprites[i] = sr.sprite;
                }
            }
        }
        
        // Create tile prefab
        GameObject tilePrefab = new GameObject("TilePrefab");
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = tilePrefab.AddComponent<SpriteRenderer>();
        if (tileSprites[0] != null)
        {
            spriteRenderer.sprite = tileSprites[0]; // Use first sprite as default
        }
        
        // Add BoxCollider2D
        BoxCollider2D collider = tilePrefab.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        
        // Add Tile script
        tilePrefab.AddComponent<Tile>();
        
        // Save as prefab
        string prefabPath = "Assets/Prefabs/TilePrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(tilePrefab, prefabPath);
        
        // Clean up
        DestroyImmediate(tilePrefab);
        
        Debug.Log("TilePrefab created successfully using existing sprites!");
        
        // Create or update GridSettings
        CreateGridSettings(tileSprites);
    }
    
    void CreateGridSettings(Sprite[] sprites)
    {
        // Create ScriptableObjects directory if it doesn't exist
        string settingsPath = "Assets/ScriptableObjects";
        if (!AssetDatabase.IsValidFolder(settingsPath))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        }
        
        // Create GridSettings
        GridSettings gridSettings = ScriptableObject.CreateInstance<GridSettings>();
        gridSettings.rows = 8;
        gridSettings.columns = 8;
        gridSettings.tileTypeCount = sprites.Length;
        gridSettings.tileSprites = sprites;
        gridSettings.tileSpacing = 1f;
        gridSettings.moveSpeed = 10f;
        gridSettings.fallSpeed = 8f;
        gridSettings.swapSpeed = 6f;
        
        string assetPath = "Assets/ScriptableObjects/LevelGridSettings.asset";
        AssetDatabase.CreateAsset(gridSettings, assetPath);
        AssetDatabase.SaveAssets();
        
        Debug.Log("GridSettings created with your existing sprites!");
    }
    
    void GenerateAdditionalSprites()
    {
        // Create additional colored sprites if needed
        Color[] additionalColors = new Color[]
        {
            Color.magenta,
            Color.cyan,
            new Color(1f, 0.5f, 0f) // Orange
        };
        
        string[] additionalNames = new string[]
        {
            "tile_magenta",
            "tile_cyan", 
            "tile_orange"
        };
        
        // Create Sprites directory if it doesn't exist
        string spritePath = "Assets/Sprites";
        if (!AssetDatabase.IsValidFolder(spritePath))
        {
            AssetDatabase.CreateFolder("Assets", "Sprites");
        }
        
        for (int i = 0; i < additionalColors.Length; i++)
        {
            CreateTileSprite(additionalColors[i], additionalNames[i]);
        }
        
        AssetDatabase.Refresh();
        Debug.Log("Generated additional tile sprites!");
    }
    
    void CreateTileSprite(Color color, string name)
    {
        // Create texture
        Texture2D texture = new Texture2D(spriteSize, spriteSize);
        
        // Fill with solid color
        Color[] pixels = new Color[spriteSize * spriteSize];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        // Add border for better visibility
        AddBorder(pixels, spriteSize, Color.black, 8);
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Save as PNG
        byte[] pngData = texture.EncodeToPNG();
        string filePath = $"Assets/Sprites/{name}.png";
        File.WriteAllBytes(filePath, pngData);
        
        // Clean up
        DestroyImmediate(texture);
        
        // Import and configure
        AssetDatabase.ImportAsset(filePath);
        ConfigureSprite(filePath);
    }
    
    void AddBorder(Color[] pixels, int size, Color borderColor, int borderWidth)
    {
        // Top and bottom borders
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < borderWidth; y++)
            {
                pixels[y * size + x] = borderColor; // Top
                pixels[(size - 1 - y) * size + x] = borderColor; // Bottom
            }
        }
        
        // Left and right borders  
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < borderWidth; x++)
            {
                pixels[y * size + x] = borderColor; // Left
                pixels[y * size + (size - 1 - x)] = borderColor; // Right
            }
        }
    }
    
    void ConfigureSprite(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritePixelsPerUnit = 100;
            
            AssetDatabase.ImportAsset(assetPath);
        }
    }
}
#endif
