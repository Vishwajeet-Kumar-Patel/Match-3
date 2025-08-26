using UnityEngine;
using UnityEditor;
using System.Collections;

#if UNITY_EDITOR
public class SafeScreenFitFix : EditorWindow
{
    [MenuItem("Tools/Safe Screen Fit Fix")]
    public static void ShowWindow()
    {
        GetWindow<SafeScreenFitFix>("Safe Screen Fix");
    }
    
    private bool isProcessing = false;
    
    void OnGUI()
    {
        GUILayout.Label("🔧 Safe Screen Fit Fix", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        EditorGUI.BeginDisabledGroup(isProcessing);
        
        if (GUILayout.Button("🎯 SAFE FIX (Recommended)"))
        {
            SafeFix();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📏 Center Grid Only"))
        {
            SafeCenterGrid();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🎨 Add Backgrounds Only"))
        {
            SafeAddBackgrounds();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📷 Fix Camera Only"))
        {
            SafeFixCamera();
        }
        
        EditorGUI.EndDisabledGroup();
        
        if (isProcessing)
        {
            GUILayout.Label("Processing... Please wait", EditorStyles.helpBox);
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("Safe Fix will:", EditorStyles.helpBox);
        GUILayout.Label("• Center grid in screen", EditorStyles.helpBox);
        GUILayout.Label("• Add tile backgrounds", EditorStyles.helpBox);
        GUILayout.Label("• Fix camera view", EditorStyles.helpBox);
        GUILayout.Label("• No Unity assertion errors", EditorStyles.helpBox);
    }
    
    void SafeFix()
    {
        if (isProcessing) return;
        
        EditorCoroutine.start(SafeFixCoroutine());
    }
    
    System.Collections.IEnumerator SafeFixCoroutine()
    {
        isProcessing = true;
        Repaint();
        
        try
        {
            Debug.Log("=== SAFE SCREEN FIT FIX ===");
            
            // Step 1: Center grid
            yield return null;
            SafeCenterGrid();
            
            // Step 2: Fix camera
            yield return null;
            SafeFixCamera();
            
            // Step 3: Add backgrounds
            yield return null;
            SafeAddBackgrounds();
            
            Debug.Log("✅ SAFE FIX COMPLETE!");
        }
        finally
        {
            isProcessing = false;
            Repaint();
        }
    }
    
    void SafeCenterGrid()
    {
        try
        {
            GameObject gridParent = GameObject.Find("GridParent");
            if (gridParent == null)
            {
                Debug.LogWarning("❌ GridParent not found!");
                return;
            }
            
            // Simple, safe centering
            float spacing = 0.9f; // Use known safe value
            float gridSize = 7 * spacing; // 7 gaps for 8x8 grid
            
            Vector3 centerPos = new Vector3(-gridSize / 2f, gridSize / 2f, 0f);
            gridParent.transform.position = centerPos;
            
            Debug.Log($"✅ Grid safely centered at {centerPos}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error centering grid: {e.Message}");
        }
    }
    
    void SafeFixCamera()
    {
        try
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("❌ Main Camera not found!");
                return;
            }
            
            // Safe camera settings
            mainCamera.transform.position = new Vector3(0, 0, -10f);
            mainCamera.orthographicSize = 4.8f; // Safe size for 8x8 grid
            mainCamera.backgroundColor = new Color(0.9f, 0.94f, 0.98f, 1f);
            
            Debug.Log("✅ Camera safely configured");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error fixing camera: {e.Message}");
        }
    }
    
    void SafeAddBackgrounds()
    {
        try
        {
            Debug.Log("=== ADDING BACKGROUNDS SAFELY ===");
            
            // Find all tiles
            Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
            if (tiles.Length == 0)
            {
                Debug.LogWarning("❌ No tiles found!");
                return;
            }
            
            // Create simple background texture
            Texture2D bgTexture = CreateSimpleBackground();
            Sprite bgSprite = Sprite.Create(bgTexture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
            
            int count = 0;
            foreach (Tile tile in tiles)
            {
                if (tile == null) continue;
                
                try
                {
                    // Remove existing background safely
                    Transform existingBG = tile.transform.Find("TileBackground");
                    if (existingBG != null)
                    {
                        DestroyImmediate(existingBG.gameObject);
                    }
                    
                    // Create new background
                    GameObject background = new GameObject("TileBackground");
                    background.transform.SetParent(tile.transform);
                    background.transform.localPosition = Vector3.zero;
                    background.transform.localScale = Vector3.one * 1.2f;
                    
                    // Add renderer
                    SpriteRenderer bgRenderer = background.AddComponent<SpriteRenderer>();
                    bgRenderer.sprite = bgSprite;
                    bgRenderer.color = new Color(0.3f, 0.4f, 0.6f, 0.8f);
                    bgRenderer.sortingOrder = -5;
                    
                    // Ensure tile is in front
                    if (tile.spriteRenderer != null)
                    {
                        tile.spriteRenderer.sortingOrder = 0;
                    }
                    
                    count++;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Failed to add background to tile: {e.Message}");
                }
            }
            
            Debug.Log($"✅ Safely added backgrounds to {count} tiles!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error adding backgrounds: {e.Message}");
        }
    }
    
    Texture2D CreateSimpleBackground()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        
        Color bgColor = Color.white;
        Color borderColor = new Color(0.8f, 0.8f, 0.9f, 1f);
        
        for (int i = 0; i < colors.Length; i++)
        {
            int x = i % size;
            int y = i / size;
            
            bool isBorder = x < 3 || x >= size - 3 || y < 3 || y >= size - 3;
            colors[i] = isBorder ? borderColor : bgColor;
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return texture;
    }
    
    void OnDestroy()
    {
        EditorCoroutine.stopAll();
    }
}

// Simple coroutine handler for editor
public class EditorCoroutine
{
    public static EditorCoroutine start(System.Collections.IEnumerator _routine)
    {
        EditorCoroutine coroutine = new EditorCoroutine(_routine);
        coroutine.start();
        return coroutine;
    }
    
    readonly System.Collections.IEnumerator routine;
    EditorCoroutine(System.Collections.IEnumerator _routine)
    {
        routine = _routine;
    }
    
    void start()
    {
        EditorApplication.update += update;
    }
    
    public void stop()
    {
        EditorApplication.update -= update;
    }
    
    void update()
    {
        if (!routine.MoveNext())
        {
            stop();
        }
    }
    
    public static void stopAll()
    {
        EditorApplication.update = null;
    }
}
#endif
