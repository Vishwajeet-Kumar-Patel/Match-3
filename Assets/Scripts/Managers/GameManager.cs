using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    LevelComplete
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game State")]
    public GameState currentState = GameState.Playing;
    
    [Header("Level Settings")]
    public int targetScore = 1000;
    public int movesLimit = 30;
    public float timeLimit = 300f; // 5 minutes
    
    [Header("Managers")]
    public Match3Manager match3Manager;
    public UIManager uiManager;
    public AudioManager audioManager;
    
    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject levelCompletePanel;
    
    private float gameTime;
    private int currentMoves;
    
    public float GameTime => gameTime;
    public int CurrentMoves => currentMoves;
    public int MovesRemaining => movesLimit - currentMoves;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        StartGame();
    }
    
    void Update()
    {
        if (currentState == GameState.Playing)
        {
            gameTime += Time.deltaTime;
            
            // Check win/lose conditions
            CheckGameConditions();
        }
        
        // Handle input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
                PauseGame();
            else if (currentState == GameState.Paused)
                ResumeGame();
        }
    }
    
    public void StartGame()
    {
        currentState = GameState.Playing;
        gameTime = 0f;
        currentMoves = 0;
        
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        
        Time.timeScale = 1f;
    }
    
    public void OnMoveMade()
    {
        if (currentState != GameState.Playing) return;
        
        currentMoves++;
        
        if (uiManager != null)
            uiManager.OnMoveMade();
    }
    
    void CheckGameConditions()
    {
        // Check if player won
        if (match3Manager != null && match3Manager.score >= targetScore)
        {
            LevelComplete();
            return;
        }
        
        // Check if player lost (no moves or time)
        if (currentMoves >= movesLimit)
        {
            GameOver("No moves remaining!");
            return;
        }
        
        if (gameTime >= timeLimit)
        {
            GameOver("Time's up!");
            return;
        }
        
        // Check if no possible moves
        if (match3Manager != null && !match3Manager.HasPossibleMoves())
        {
            // Auto-shuffle or game over
            GameOver("No possible moves!");
        }
    }
    
    public void GameOver(string reason = "")
    {
        currentState = GameState.GameOver;
        
        if (audioManager != null)
            audioManager.PlayGameOverSound();
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        Debug.Log($"Game Over: {reason}");
        Time.timeScale = 0f;
    }
    
    public void LevelComplete()
    {
        currentState = GameState.LevelComplete;
        
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
        
        Debug.Log("Level Complete!");
        Time.timeScale = 0f;
    }
    
    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            
            if (pausePanel != null)
                pausePanel.SetActive(true);
            
            Time.timeScale = 0f;
        }
    }
    
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            
            if (pausePanel != null)
                pausePanel.SetActive(false);
            
            Time.timeScale = 1f;
        }
    }
    
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Assuming you have a main menu scene
    }
    
    public void NextLevel()
    {
        // Implement level progression logic
        targetScore += 500;
        movesLimit += 5;
        StartGame();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
