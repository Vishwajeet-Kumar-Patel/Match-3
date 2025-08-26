using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class QuickGridFix : EditorWindow
{
    [MenuItem("Tools/Quick Grid Fix")]
    public static void ShowWindow()
    {
        GetWindow<QuickGridFix>("Quick Grid Fix");
    }
    
    void OnGUI()
    {
        GUILayout.Label("🎯 Quick Match3 Grid Fix", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🔧 Fix Grid Position"))
        {
            FixGridPosition();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📷 Fix Camera"))
        {
            FixCamera();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🔄 Clear and Regenerate"))
        {
            ClearGrid();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("✨ Complete Fix"))
        {
            CompleteFix();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("This fixes the grid positioning issue from your screenshot!", EditorStyles.helpBox);
    }
    
    void FixGridPosition()
    {
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
        {
            // Perfect positioning for centered 8x8 grid
            gridParent.transform.position = new Vector3(-0.35f, 0.35f, 0);
            Debug.Log("✅ GridParent positioned at (-0.35, 0.35, 0)");
        }
        else
        {
            Debug.LogWarning("❌ GridParent not found!");
        }
    }
    
    void FixCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(2.8f, -2.8f, -10f);
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 4.5f;
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.3f);
            
            Debug.Log("✅ Camera fixed: Position (2.8, -2.8, -10), Size 4.5");
        }
        else
        {
            Debug.LogWarning("❌ Main Camera not found!");
        }
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
    }
    
    void CompleteFix()
    {
        Debug.Log("=== APPLYING COMPLETE FIX ===");
        
        FixGridPosition();
        FixCamera();
        ClearGrid();
        
        // Also check if Match3Manager exists and is properly configured
        GameObject match3Manager = GameObject.Find("Match3Manager");
        if (match3Manager != null)
        {
            Debug.Log("✅ Match3Manager found - ready to generate grid");
        }
        else
        {
            Debug.LogWarning("❌ Match3Manager not found - make sure it's in the scene");
        }
        
        Debug.Log("=== FIX COMPLETE ===");
        Debug.Log("🎮 Press PLAY to see the properly positioned 8x8 grid!");
        Debug.Log("The grid should now be centered and properly sized!");
    }
}
#endif
