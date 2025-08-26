using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class SimpleGridFix : EditorWindow
{
    [MenuItem("Tools/Simple Grid Fix")]
    public static void ShowWindow()
    {
        GetWindow<SimpleGridFix>("Simple Fix");
    }
    
    void OnGUI()
    {
        GUILayout.Label("🔧 Simple Grid Fix", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🎯 CENTER GRID"))
        {
            CenterGrid();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📷 FIX CAMERA"))
        {
            FixCamera();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🎨 ADD BACKGROUNDS"))
        {
            AddBackgrounds();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("✨ DO ALL THREE"))
        {
            DoAllFixes();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("Click buttons one by one to avoid Unity sync issues", EditorStyles.helpBox);
    }
    
    void DoAllFixes()
    {
        Debug.Log("=== APPLYING ALL FIXES ===");
        CenterGrid();
        FixCamera();
        AddBackgrounds();
        Debug.Log("✅ ALL FIXES COMPLETE!");
    }
    
    void CenterGrid()
    {
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent == null)
        {
            Debug.LogWarning("GridParent not found!");
            return;
        }
        
        // Center for 8x8 grid with 0.9 spacing
        gridParent.transform.position = new Vector3(-3.15f, 3.15f, 0f);
        Debug.Log("✅ Grid centered");
    }
    
    void FixCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            Debug.LogWarning("Main Camera not found!");
            return;
        }
        
        camera.transform.position = new Vector3(0, 0, -10f);
        camera.orthographicSize = 4.8f;
        camera.backgroundColor = new Color(0.9f, 0.94f, 0.98f, 1f);
        Debug.Log("✅ Camera fixed");
    }
    
    void AddBackgrounds()
    {
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        
        foreach (Tile tile in tiles)
        {
            if (tile == null) continue;
            
            // Remove existing background
            Transform bg = tile.transform.Find("TileBackground");
            if (bg != null) DestroyImmediate(bg.gameObject);
            
            // Create simple background
            GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
            background.name = "TileBackground";
            background.transform.SetParent(tile.transform);
            background.transform.localPosition = Vector3.zero;
            background.transform.localScale = Vector3.one * 1.2f;
            
            // Remove collider
            Collider collider = background.GetComponent<Collider>();
            if (collider != null) DestroyImmediate(collider);
            
            // Set up renderer
            Renderer renderer = background.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Sprites/Default"));
                renderer.material.color = new Color(0.3f, 0.4f, 0.6f, 0.8f);
                renderer.sortingOrder = -5;
            }
            
            // Ensure tile is in front
            if (tile.spriteRenderer != null)
            {
                tile.spriteRenderer.sortingOrder = 0;
            }
        }
        
        Debug.Log($"✅ Added backgrounds to {tiles.Length} tiles");
    }
}
#endif
