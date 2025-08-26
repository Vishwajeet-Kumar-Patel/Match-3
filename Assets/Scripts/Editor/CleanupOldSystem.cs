using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class CleanupOldSystem : EditorWindow
{
    [MenuItem("Tools/Cleanup Old Grid System")]
    public static void ShowWindow()
    {
        GetWindow<CleanupOldSystem>("Cleanup Old System");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Cleanup Old Grid System", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Remove Old GridManager Components"))
        {
            CleanupOldGridManager();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Clear Old Grid Tiles"))
        {
            ClearOldTiles();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Complete Cleanup and Setup New System"))
        {
            CompleteCleanupAndSetup();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("This will:", EditorStyles.helpBox);
        GUILayout.Label("1. Remove old GridManager components", EditorStyles.helpBox);
        GUILayout.Label("2. Clear existing tiles from GridParent", EditorStyles.helpBox);
        GUILayout.Label("3. Setup new Match3 system", EditorStyles.helpBox);
    }
    
    void CleanupOldGridManager()
    {
        // Find and remove old GridManager components by name
        MonoBehaviour[] allComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        int removedCount = 0;
        
        foreach (MonoBehaviour component in allComponents)
        {
            if (component.GetType().Name == "GridManager")
            {
                Debug.Log($"Removing GridManager component from {component.gameObject.name}");
                DestroyImmediate(component);
                removedCount++;
            }
        }
        
        if (removedCount == 0)
        {
            Debug.Log("No old GridManager components found.");
        }
        else
        {
            Debug.Log($"Removed {removedCount} old GridManager components.");
        }
    }
    
    void ClearOldTiles()
    {
        // Find GridParent and clear its children
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
        {
            int childCount = gridParent.transform.childCount;
            
            // Clear all children
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(gridParent.transform.GetChild(i).gameObject);
            }
            
            Debug.Log($"Cleared {childCount} old tiles from GridParent.");
        }
        else
        {
            Debug.Log("GridParent not found - will be created by setup tool.");
        }
    }
    
    void CompleteCleanupAndSetup()
    {
        Debug.Log("=== STARTING COMPLETE CLEANUP AND SETUP ===");
        
        // Step 1: Cleanup old system
        CleanupOldGridManager();
        ClearOldTiles();
        
        // Step 2: Run the prefab converter
        PrefabToTileConverter converter = GetWindow<PrefabToTileConverter>();
        converter.Close(); // Close the window but use its functionality
        
        // Convert existing prefabs
        ConvertExistingPrefabs();
        
        // Create new tile prefab and grid settings
        CreateNewTileSystem();
        
        Debug.Log("=== CLEANUP AND SETUP COMPLETE ===");
        Debug.Log("Next: Add scripts to manager GameObjects and configure references!");
    }
    
    void ConvertExistingPrefabs()
    {
        string[] prefabNames = { "leaf", "rainbow", "rock", "sun", "tile" };
        
        foreach (string prefabName in prefabNames)
        {
            string path = $"Assets/Prefabs/{prefabName}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                // Load prefab for editing
                GameObject prefabInstance = PrefabUtility.LoadPrefabContents(path);
                
                // Ensure it has SpriteRenderer
                if (prefabInstance.GetComponent<SpriteRenderer>() == null)
                {
                    prefabInstance.AddComponent<SpriteRenderer>();
                }
                
                // Ensure it has BoxCollider2D
                BoxCollider2D collider = prefabInstance.GetComponent<BoxCollider2D>();
                if (collider == null)
                {
                    collider = prefabInstance.AddComponent<BoxCollider2D>();
                    collider.size = Vector2.one;
                }
                
                // Ensure it has Tile script
                if (prefabInstance.GetComponent<Tile>() == null)
                {
                    prefabInstance.AddComponent<Tile>();
                }
                
                // Save changes
                PrefabUtility.SaveAsPrefabAsset(prefabInstance, path);
                PrefabUtility.UnloadPrefabContents(prefabInstance);
            }
        }
        
        Debug.Log("Converted existing prefabs to be tile-compatible.");
    }
    
    void CreateNewTileSystem()
    {
        // Collect sprites from existing prefabs
        string[] prefabNames = { "leaf", "rainbow", "rock", "sun", "tile" };
        Sprite[] sprites = new Sprite[prefabNames.Length];
        
        for (int i = 0; i < prefabNames.Length; i++)
        {
            string path = $"Assets/Prefabs/{prefabNames[i]}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    sprites[i] = sr.sprite;
                }
            }
        }
        
        // Create new TilePrefab
        GameObject tilePrefab = new GameObject("TilePrefab");
        
        SpriteRenderer spriteRenderer = tilePrefab.AddComponent<SpriteRenderer>();
        if (sprites[0] != null)
        {
            spriteRenderer.sprite = sprites[0];
        }
        
        BoxCollider2D collider = tilePrefab.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        
        tilePrefab.AddComponent<Tile>();
        
        // Save as prefab
        string prefabPath = "Assets/Prefabs/TilePrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(tilePrefab, prefabPath);
        DestroyImmediate(tilePrefab);
        
        // Create GridSettings
        CreateGridSettings(sprites);
        
        Debug.Log("Created new TilePrefab and GridSettings!");
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
        
        Selection.activeObject = gridSettings;
        
        Debug.Log("Created GridSettings with your sprites!");
    }
}
#endif
