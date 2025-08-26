using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class GridSizeFixer : EditorWindow
{
    private float cameraSize = 6f;
    private float tileSpacing = 0.8f;
    private Vector3 tileScale = new Vector3(0.8f, 0.8f, 1f);
    
    [MenuItem("Tools/Fix Grid Size")]
    public static void ShowWindow()
    {
        GetWindow<GridSizeFixer>("Fix Grid Size");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Fix Oversized Grid", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        cameraSize = EditorGUILayout.Slider("Camera Size", cameraSize, 4f, 10f);
        tileSpacing = EditorGUILayout.Slider("Tile Spacing", tileSpacing, 0.5f, 1.5f);
        tileScale = EditorGUILayout.Vector3Field("Tile Scale", tileScale);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Apply Quick Fix"))
        {
            ApplyQuickFix();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Reset Camera Only"))
        {
            FixCamera();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Clear Grid and Regenerate"))
        {
            ClearAndRegenerate();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("Current Issues:", EditorStyles.helpBox);
        GUILayout.Label("• Tiles are too big for 8x8 grid", EditorStyles.helpBox);
        GUILayout.Label("• Camera might be too close", EditorStyles.helpBox);
        GUILayout.Label("• Tile spacing might be too large", EditorStyles.helpBox);
    }
    
    void ApplyQuickFix()
    {
        Debug.Log("=== APPLYING GRID SIZE FIX ===");
        
        // Fix camera
        FixCamera();
        
        // Fix tile spacing in Match3Manager
        FixTileSpacing();
        
        // Fix tile prefab scale
        FixTilePrefabScale();
        
        // Clear and regenerate grid
        ClearAndRegenerate();
        
        Debug.Log("✅ Grid size fix applied!");
        Debug.Log("Press Play to see the properly sized 8x8 grid!");
    }
    
    void FixCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(3.5f, -3.5f, -10f);
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = cameraSize;
            
            Debug.Log($"✅ Camera fixed: Size {cameraSize}, Position (3.5, -3.5, -10)");
        }
        else
        {
            Debug.LogError("Main Camera not found!");
        }
    }
    
    void FixTileSpacing()
    {
        // Find Match3Manager and update tile spacing
        GameObject match3ManagerObj = GameObject.Find("Match3Manager");
        if (match3ManagerObj != null)
        {
            Match3Manager manager = match3ManagerObj.GetComponent<Match3Manager>();
            if (manager != null)
            {
                // Use reflection to set the tile spacing since it might be private
                var field = typeof(Match3Manager).GetField("tileSpacing", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(manager, tileSpacing);
                    Debug.Log($"✅ Tile spacing set to {tileSpacing}");
                }
            }
        }
    }
    
    void FixTilePrefabScale()
    {
        // Find and scale the TilePrefab
        string prefabPath = "Assets/Prefabs/TilePrefab.prefab";
        GameObject tilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (tilePrefab != null)
        {
            // Load prefab for editing
            GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
            prefabInstance.transform.localScale = tileScale;
            
            // Save changes
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabInstance);
            
            Debug.Log($"✅ TilePrefab scale set to {tileScale}");
        }
        else
        {
            Debug.LogWarning("TilePrefab not found at Assets/Prefabs/TilePrefab.prefab");
        }
    }
    
    void ClearAndRegenerate()
    {
        // Clear existing tiles
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
        {
            int childCount = gridParent.transform.childCount;
            
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(gridParent.transform.GetChild(i).gameObject);
            }
            
            Debug.Log($"✅ Cleared {childCount} old tiles");
        }
        
        Debug.Log("Grid cleared. Press Play to regenerate with new settings!");
    }
}
#endif
