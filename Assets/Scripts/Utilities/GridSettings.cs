using UnityEngine;

[CreateAssetMenu(fileName = "GridSettings", menuName = "Match3/Grid Settings")]
public class GridSettings : ScriptableObject
{
    [Header("Grid Dimensions")]
    public int rows = 8;
    public int columns = 8;
    
    [Header("Tile Settings")]
    public int tileTypeCount = 5;
    public float tileSpacing = 1f;
    
    [Header("Tile Visuals")]
    public Sprite[] tileSprites;
    
    [Header("Animation Settings")]
    public float moveSpeed = 10f;
    public float fallSpeed = 8f;
    public float swapSpeed = 6f;
    
    [Header("Game Rules")]
    public int minimumMatchLength = 3;
    public bool allowDiagonalMatches = false;
    
    [Header("Special Tiles")]
    public bool enableBombTiles = true;
    public bool enableStripedTiles = true;
    public bool enableColorBombs = true;
    
    [Header("Scoring")]
    public int baseScore = 10;
    public float comboMultiplier = 1.5f;
    public int[] matchBonuses = {0, 0, 0, 50, 100, 200}; // Index = match length
}
