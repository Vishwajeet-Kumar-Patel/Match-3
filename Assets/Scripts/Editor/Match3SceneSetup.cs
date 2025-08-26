using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

#if UNITY_EDITOR
public class Match3SceneSetup : EditorWindow
{
    [MenuItem("Tools/Setup Match3 Scene")]
    public static void ShowWindow()
    {
        GetWindow<Match3SceneSetup>("Match3 Scene Setup");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Match3 Scene Auto-Setup", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create Complete Scene Setup"))
        {
            SetupScene();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Setup Camera Only"))
        {
            SetupCamera();
        }
        
        if (GUILayout.Button("Create UI Only"))
        {
            SetupUI();
        }
        
        if (GUILayout.Button("Create Managers Only"))
        {
            SetupManagers();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("This will create the complete scene hierarchy\nfor your Match3 game automatically.", EditorStyles.helpBox);
    }
    
    void SetupScene()
    {
        SetupManagers();
        SetupCamera();
        SetupUI();
        CreateGridParent();
        
        Debug.Log("Match3 scene setup complete! Don't forget to:");
        Debug.Log("1. Assign scripts to manager GameObjects");
        Debug.Log("2. Create tile prefab");
        Debug.Log("3. Create GridSettings ScriptableObject"); 
        Debug.Log("4. Configure all manager references");
    }
    
    void SetupManagers()
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
        if (audioManager != null)
        {
            if (audioManager.GetComponent<AudioSource>() == null)
            {
                AudioSource musicSource = audioManager.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.volume = 0.7f;
                
                AudioSource sfxSource = audioManager.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.volume = 1.0f;
            }
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
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.2f); // Dark blue
            
            Debug.Log("Camera configured for 8x8 grid!");
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
}
#endif
