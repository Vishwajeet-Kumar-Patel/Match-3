using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

#if UNITY_EDITOR
public class PrefabToTileConverter : EditorWindow
{
    [MenuItem("Tools/Convert Existing Prefabs to Match3")]
    public static void ShowWindow()
    {
        GetWindow<PrefabToTileConverter>("Prefab to Tile Converter");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Convert Existing Prefabs", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Convert Existing Prefabs to Tiles"))
        {
            ConvertPrefabsToTiles();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create TilePrefab from Existing"))
        {
            CreateTilePrefabFromExisting();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Setup Complete Match3 Scene"))
        {
            SetupCompleteScene();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("This will:", EditorStyles.helpBox);
        GUILayout.Label("1. Convert your existing prefabs (leaf, rainbow, rock, sun, tile)", EditorStyles.helpBox);
        GUILayout.Label("2. Add Tile scripts and colliders if missing", EditorStyles.helpBox);
        GUILayout.Label("3. Create a new TilePrefab for the grid", EditorStyles.helpBox);
        GUILayout.Label("4. Setup GridSettings with your sprites", EditorStyles.helpBox);
    }
    
    void ConvertPrefabsToTiles()
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
                SpriteRenderer sr = prefabInstance.GetComponent<SpriteRenderer>();
                if (sr == null)
                {
                    sr = prefabInstance.AddComponent<SpriteRenderer>();
                    Debug.Log($"Added SpriteRenderer to {prefabName}");
                }
                
                // Ensure it has BoxCollider2D
                BoxCollider2D collider = prefabInstance.GetComponent<BoxCollider2D>();
                if (collider == null)
                {
                    collider = prefabInstance.AddComponent<BoxCollider2D>();
                    collider.size = Vector2.one;
                    Debug.Log($"Added BoxCollider2D to {prefabName}");
                }
                
                // Ensure it has Tile script
                Tile tileScript = prefabInstance.GetComponent<Tile>();
                if (tileScript == null)
                {
                    prefabInstance.AddComponent<Tile>();
                    Debug.Log($"Added Tile script to {prefabName}");
                }
                
                // Save changes
                PrefabUtility.SaveAsPrefabAsset(prefabInstance, path);
                PrefabUtility.UnloadPrefabContents(prefabInstance);
                
                Debug.Log($"Converted {prefabName} to tile-compatible prefab");
            }
            else
            {
                Debug.LogWarning($"Prefab not found: {path}");
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Finished converting existing prefabs!");
    }
    
    void CreateTilePrefabFromExisting()
    {
        string[] prefabNames = { "leaf", "rainbow", "rock", "sun", "tile" };
        Sprite[] sprites = new Sprite[prefabNames.Length];
        
        // Collect sprites from existing prefabs
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
        
        // Add components
        SpriteRenderer spriteRenderer = tilePrefab.AddComponent<SpriteRenderer>();
        if (sprites[0] != null)
        {
            spriteRenderer.sprite = sprites[0]; // Use first sprite as default
        }
        
        BoxCollider2D collider = tilePrefab.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        
        Tile tileScript = tilePrefab.AddComponent<Tile>();
        tileScript.moveSpeed = 10f;
        
        // Save as prefab
        string prefabPath = "Assets/Prefabs/TilePrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(tilePrefab, prefabPath);
        DestroyImmediate(tilePrefab);
        
        Debug.Log("Created TilePrefab from existing sprites!");
        
        // Create GridSettings
        CreateGridSettings(sprites);
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
        gridSettings.baseScore = 10;
        gridSettings.comboMultiplier = 1.5f;
        
        string assetPath = "Assets/ScriptableObjects/LevelGridSettings.asset";
        AssetDatabase.CreateAsset(gridSettings, assetPath);
        AssetDatabase.SaveAssets();
        
        Selection.activeObject = gridSettings; // Select it in the inspector
        
        Debug.Log("Created GridSettings with your existing sprites!");
    }
    
    void SetupCompleteScene()
    {
        // First convert prefabs
        ConvertPrefabsToTiles();
        
        // Create tile prefab and grid settings
        CreateTilePrefabFromExisting();
        
        // Setup scene using the existing Match3SceneSetup
        Match3SceneSetup sceneSetup = EditorWindow.GetWindow<Match3SceneSetup>();
        if (sceneSetup != null)
        {
            sceneSetup.Close();
        }
        
        // Create scene objects
        CreateManagers();
        SetupCamera();
        CreateGridParent();
        SetupUI();
        
        Debug.Log("=== COMPLETE MATCH3 SETUP FINISHED ===");
        Debug.Log("Next steps:");
        Debug.Log("1. Assign scripts to manager GameObjects");
        Debug.Log("2. Configure Match3Manager with TilePrefab and GridSettings");
        Debug.Log("3. Configure UIManager with UI text elements");
        Debug.Log("4. Press Play to test!");
    }
    
    void CreateManagers()
    {
        // Create GameManagers parent
        GameObject managersParent = GameObject.Find("GameManagers");
        if (managersParent == null)
        {
            managersParent = new GameObject("GameManagers");
        }
        
        // Create individual managers
        string[] managerNames = {
            "GameManager",
            "Match3Manager", 
            "AudioManager",
            "UIManager",
            "EffectsManager",
            "InputManager"
        };
        
        foreach (string managerName in managerNames)
        {
            GameObject manager = GameObject.Find(managerName);
            if (manager == null)
            {
                manager = new GameObject(managerName);
                manager.transform.SetParent(managersParent.transform);
            }
        }
        
        // Add AudioSource components to AudioManager
        GameObject audioManager = GameObject.Find("AudioManager");
        if (audioManager != null && audioManager.GetComponents<AudioSource>().Length == 0)
        {
            AudioSource musicSource = audioManager.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = 0.7f;
            
            AudioSource sfxSource = audioManager.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = 1.0f;
        }
        
        Debug.Log("Managers created successfully!");
    }
    
    void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(3.5f, -3.5f, -10f);
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 6f;
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.2f);
            
            Debug.Log("Camera configured for 8x8 grid!");
        }
    }
    
    void CreateGridParent()
    {
        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent == null)
        {
            gridParent = new GameObject("GridParent");
            gridParent.transform.position = Vector3.zero;
            Debug.Log("GridParent created!");
        }
    }
    
    void SetupUI()
    {
        // Check if Canvas already exists
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            // Create Canvas
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Create EventSystem if it doesn't exist
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Create UI Text elements
        CreateUIText(canvas.transform, "ScoreText", "Score: 0", TextAnchor.UpperLeft, new Vector2(-800, 450));
        CreateUIText(canvas.transform, "MovesText", "Moves: 30", TextAnchor.UpperRight, new Vector2(800, 450));
        CreateUIText(canvas.transform, "LevelText", "Level: 1", TextAnchor.UpperCenter, new Vector2(0, 450));
        
        Debug.Log("UI Canvas and elements created!");
    }
    
    void CreateUIText(Transform parent, string name, string text, TextAnchor alignment, Vector2 position)
    {
        // Check if text already exists
        Transform existing = parent.Find(name);
        if (existing != null) return;
        
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(300, 50);
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 36;
        textComponent.color = Color.white;
        textComponent.alignment = alignment;
    }
}
#endif
