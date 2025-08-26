using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class QuickTileFix : EditorWindow
{
    [MenuItem("Tools/Quick Tile Fix")]
    public static void ShowWindow()
    {
        GetWindow<QuickTileFix>("Quick Tile Fix");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Quick Tile Fix", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Fix Missing Script and Create TilePrefab"))
        {
            FixMissingScriptAndCreateTile();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("This will:", EditorStyles.helpBox);
        GUILayout.Label("1. Create TilePrefab from your existing prefabs", EditorStyles.helpBox);
        GUILayout.Label("2. Create GridParent if missing", EditorStyles.helpBox);
        GUILayout.Label("3. Setup basic scene structure", EditorStyles.helpBox);
    }
    
    void FixMissingScriptAndCreateTile()
    {
        Debug.Log("=== QUICK FIX STARTED ===");
        
        // Create TilePrefab from existing prefabs
        CreateTilePrefabFromExisting();
        
        // Create GridParent if missing
        CreateGridParent();
        
        // Create basic managers if missing
        CreateBasicManagers();
        
        Debug.Log("=== QUICK FIX COMPLETE ===");
        Debug.Log("Now:");
        Debug.Log("1. Remove missing script component manually");
        Debug.Log("2. Add Match3Manager script to the GameObject");
        Debug.Log("3. Assign TilePrefab and GridParent to Match3Manager");
        Debug.Log("4. Press Play!");
    }
    
    void CreateTilePrefabFromExisting()
    {
        // Try to find existing prefabs
        string[] prefabNames = { "leaf", "rainbow", "rock", "sun", "tile" };
        Sprite firstSprite = null;
        
        // Find the first available sprite
        foreach (string prefabName in prefabNames)
        {
            string path = $"Assets/Prefabs/{prefabName}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    firstSprite = sr.sprite;
                    break;
                }
            }
        }
        
        // Create TilePrefab
        GameObject tilePrefab = new GameObject("TilePrefab");
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = tilePrefab.AddComponent<SpriteRenderer>();
        if (firstSprite != null)
        {
            spriteRenderer.sprite = firstSprite;
        }
        
        // Add BoxCollider2D
        BoxCollider2D collider = tilePrefab.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        
        // Add Tile script
        tilePrefab.AddComponent<Tile>();
        
        // Save as prefab
        string prefabPath = "Assets/Prefabs/TilePrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(tilePrefab, prefabPath);
        DestroyImmediate(tilePrefab);
        
        Debug.Log("✅ Created TilePrefab!");
        
        // Select the prefab so user can see it
        GameObject savedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Selection.activeObject = savedPrefab;
    }
    
    void CreateGridParent()
    {
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent == null)
        {
            gridParent = new GameObject("GridParent");
            gridParent.transform.position = Vector3.zero;
            Debug.Log("✅ Created GridParent!");
        }
        else
        {
            Debug.Log("✅ GridParent already exists!");
        }
    }
    
    void CreateBasicManagers()
    {
        // Create GameManagers parent if it doesn't exist
        GameObject managersParent = GameObject.Find("GameManagers");
        if (managersParent == null)
        {
            managersParent = new GameObject("GameManagers");
            Debug.Log("✅ Created GameManagers!");
        }
        
        // Create essential managers
        string[] managerNames = { "Match3Manager", "UIManager", "InputManager" };
        
        foreach (string managerName in managerNames)
        {
            GameObject manager = GameObject.Find(managerName);
            if (manager == null)
            {
                manager = new GameObject(managerName);
                manager.transform.SetParent(managersParent.transform);
                Debug.Log($"✅ Created {managerName}!");
            }
        }
    }
}
#endif
