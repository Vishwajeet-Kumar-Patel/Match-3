using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text scoreText;
    public Text movesText;
    public Text levelText;
    public Button shuffleButton;
    public Button hintButton;
    
    [Header("Game Settings")]
    public int movesRemaining = 30;
    public int currentLevel = 1;
    
    private Match3Manager match3Manager;
    
    void Start()
    {
        match3Manager = Match3Manager.Instance;
        
        if (shuffleButton != null)
            shuffleButton.onClick.AddListener(ShuffleGrid);
            
        if (hintButton != null)
            hintButton.onClick.AddListener(ShowHint);
            
        UpdateUI();
    }
    
    void Update()
    {
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (match3Manager != null)
        {
            if (scoreText != null)
                scoreText.text = "Score: " + match3Manager.score;
        }
        
        if (movesText != null)
            movesText.text = "Moves: " + movesRemaining;
            
        if (levelText != null)
            levelText.text = "Level: " + currentLevel;
    }
    
    public void OnMoveMade()
    {
        movesRemaining--;
        if (movesRemaining <= 0)
        {
            GameOver();
        }
    }
    
    void GameOver()
    {
        Debug.Log("Game Over! Final Score: " + match3Manager.score);
        // Add game over logic here
    }
    
    void ShuffleGrid()
    {
        // Implement grid shuffle logic
        Debug.Log("Shuffle requested");
    }
    
    void ShowHint()
    {
        if (match3Manager != null)
        {
            bool hasMoves = match3Manager.HasPossibleMoves();
            if (!hasMoves)
            {
                Debug.Log("No possible moves - shuffle needed!");
            }
            else
            {
                Debug.Log("Hint: Look for possible matches!");
            }
        }
    }
}
