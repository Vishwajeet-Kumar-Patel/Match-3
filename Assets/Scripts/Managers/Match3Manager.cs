using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Match3Manager : MonoBehaviour
{
    public static Match3Manager Instance;

    [Header("Grid Settings")]
    public bool useScriptableSettings = true;
    public GridSettings gridSettings;

    [Header("Prefabs & References")]
    public GameObject tilePrefab;
    public Transform gridParent;

    [Header("Runtime Grid Info")]
    public int rows = 8;
    public int columns = 8;
    public int tileTypeCount = 5;
    public Sprite[] tileSprites;

    [Header("Game Settings")]
    public float tileSpacing = 1f;
    public float fallSpeed = 10f;
    public float swapSpeed = 8f;

    [Header("Score")]
    public int score = 0;
    public int comboMultiplier = 1;

    private Tile[,] grid;
    private Tile selectedTile;
    private Tile targetTile;
    private bool isProcessing = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        LoadGridSettings();
        GenerateInitialGrid();
    }

    void LoadGridSettings()
    {
        if (useScriptableSettings && gridSettings != null)
        {
            rows = gridSettings.rows;
            columns = gridSettings.columns;
            tileTypeCount = gridSettings.tileTypeCount;
            tileSprites = gridSettings.tileSprites;
        }

        grid = new Tile[rows, columns];
    }

    void GenerateInitialGrid()
    {
        // Clear old tiles if any
        if (gridParent != null)
        {
            for (int i = gridParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(gridParent.GetChild(i).gameObject);
            }
        }

        // Generate grid without initial matches
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                CreateTileAt(row, col, GetRandomTileType(row, col));
            }
        }

        // Check for initial matches and regenerate if needed
        while (HasMatches())
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (IsPartOfMatch(row, col))
                    {
                        grid[row, col].SetTileType(GetRandomTileType(row, col),
                            tileSprites[grid[row, col].tileType]);
                    }
                }
            }
        }
    }

    void CreateTileAt(int row, int col, int tileType)
    {
        Vector3 position = new Vector3(col * tileSpacing, -row * tileSpacing, 0);
        GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, gridParent);

        Tile tile = tileObj.GetComponent<Tile>();
        if (tile == null)
            tile = tileObj.AddComponent<Tile>();

        tile.SetGridPosition(row, col);
        tile.SetTileType(tileType, tileSprites[tileType]);
        tile.SetPosition(position);

        grid[row, col] = tile;
    }

    int GetRandomTileType(int row, int col)
    {
        List<int> validTypes = new List<int>();

        for (int i = 0; i < tileTypeCount; i++)
        {
            validTypes.Add(i);
        }

        // Remove types that would create horizontal matches
        if (col >= 2)
        {
            if (grid[row, col - 1] != null && grid[row, col - 2] != null)
            {
                if (grid[row, col - 1].tileType == grid[row, col - 2].tileType)
                {
                    validTypes.Remove(grid[row, col - 1].tileType);
                }
            }
        }

        // Remove types that would create vertical matches
        if (row >= 2)
        {
            if (grid[row - 1, col] != null && grid[row - 2, col] != null)
            {
                if (grid[row - 1, col].tileType == grid[row - 2, col].tileType)
                {
                    validTypes.Remove(grid[row - 1, col].tileType);
                }
            }
        }

        return validTypes[Random.Range(0, validTypes.Count)];
    }

    public void OnTileClicked(Tile clickedTile)
    {
        if (isProcessing) return;

        if (selectedTile == null)
        {
            // First tile selection
            selectedTile = clickedTile;
            HighlightTile(selectedTile, true);
        }
        else if (selectedTile == clickedTile)
        {
            // Deselect current tile
            HighlightTile(selectedTile, false);
            selectedTile = null;
        }
        else if (AreAdjacent(selectedTile, clickedTile))
        {
            // Valid swap
            targetTile = clickedTile;
            StartCoroutine(SwapTiles(selectedTile, targetTile));
        }
        else
        {
            // Select new tile
            HighlightTile(selectedTile, false);
            selectedTile = clickedTile;
            HighlightTile(selectedTile, true);
        }
    }

    void HighlightTile(Tile tile, bool highlight)
    {
        if (tile != null && tile.spriteRenderer != null)
        {
            tile.spriteRenderer.color = highlight ? Color.yellow : Color.white;
        }
    }

    bool AreAdjacent(Tile tile1, Tile tile2)
    {
        int rowDiff = Mathf.Abs(tile1.row - tile2.row);
        int colDiff = Mathf.Abs(tile1.column - tile2.column);

        return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
    }

    // Mobile-friendly method for direct tile swapping
    public bool TrySwapTiles(Tile tile1, Tile tile2)
    {
        if (isProcessing) return false;
        if (tile1 == null || tile2 == null) return false;
        if (!AreAdjacent(tile1, tile2)) return false;

        // Clear any previous selection
        if (selectedTile != null)
        {
            HighlightTile(selectedTile, false);
        }

        // Perform the swap
        selectedTile = tile1;
        targetTile = tile2;
        StartCoroutine(SwapTiles(tile1, tile2));

        return true;
    }

    IEnumerator SwapTiles(Tile tile1, Tile tile2)
    {
        isProcessing = true;
        HighlightTile(selectedTile, false);

        // Play swap sound
        Debug.Log("Tile swap!");

        // Swap positions in grid array
        grid[tile1.row, tile1.column] = tile2;
        grid[tile2.row, tile2.column] = tile1;

        // Swap grid coordinates
        int tempRow = tile1.row;
        int tempCol = tile1.column;
        tile1.SetGridPosition(tile2.row, tile2.column);
        tile2.SetGridPosition(tempRow, tempCol);

        // Animate swap
        Vector3 tile1Target = new Vector3(tile1.column * tileSpacing, -tile1.row * tileSpacing, 0);
        Vector3 tile2Target = new Vector3(tile2.column * tileSpacing, -tile2.row * tileSpacing, 0);

        tile1.MoveTo(tile1Target);
        tile2.MoveTo(tile2Target);

        // Wait for animation to complete
        yield return new WaitUntil(() => !tile1.IsMoving && !tile2.IsMoving);

        // Check for matches
        bool hasMatches = HasMatches();

        if (hasMatches)
        {
            // Valid move - notify about move and process matches
            Debug.Log("Valid move made!");

            yield return StartCoroutine(ProcessMatches());
        }
        else
        {
            // Invalid move - play sound and swap back
            Debug.Log("Invalid move!");

            grid[tile1.row, tile1.column] = tile2;
            grid[tile2.row, tile2.column] = tile1;

            int tempRow2 = tile1.row;
            int tempCol2 = tile1.column;
            tile1.SetGridPosition(tile2.row, tile2.column);
            tile2.SetGridPosition(tempRow2, tempCol2);

            tile1Target = new Vector3(tile1.column * tileSpacing, -tile1.row * tileSpacing, 0);
            tile2Target = new Vector3(tile2.column * tileSpacing, -tile2.row * tileSpacing, 0);

            tile1.MoveTo(tile1Target);
            tile2.MoveTo(tile2Target);

            yield return new WaitUntil(() => !tile1.IsMoving && !tile2.IsMoving);
        }

        selectedTile = null;
        targetTile = null;
        isProcessing = false;
    }

    bool HasMatches()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (IsPartOfMatch(row, col))
                    return true;
            }
        }
        return false;
    }

    bool IsPartOfMatch(int row, int col)
    {
        if (grid[row, col] == null) return false;

        int tileType = grid[row, col].tileType;

        // Check horizontal match
        int horizontalCount = 1;

        // Count left
        for (int i = col - 1; i >= 0; i--)
        {
            if (grid[row, i] != null && grid[row, i].tileType == tileType)
                horizontalCount++;
            else
                break;
        }

        // Count right
        for (int i = col + 1; i < columns; i++)
        {
            if (grid[row, i] != null && grid[row, i].tileType == tileType)
                horizontalCount++;
            else
                break;
        }

        if (horizontalCount >= 3) return true;

        // Check vertical match
        int verticalCount = 1;

        // Count up
        for (int i = row - 1; i >= 0; i--)
        {
            if (grid[i, col] != null && grid[i, col].tileType == tileType)
                verticalCount++;
            else
                break;
        }

        // Count down
        for (int i = row + 1; i < rows; i++)
        {
            if (grid[i, col] != null && grid[i, col].tileType == tileType)
                verticalCount++;
            else
                break;
        }

        return verticalCount >= 3;
    }

    IEnumerator ProcessMatches()
    {
        bool foundMatches;

        do
        {
            foundMatches = false;
            List<Tile> tilesToRemove = new List<Tile>();

            // Find all matching tiles
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (IsPartOfMatch(row, col))
                    {
                        tilesToRemove.Add(grid[row, col]);
                        foundMatches = true;
                    }
                }
            }

            if (foundMatches)
            {
                // Remove matched tiles
                foreach (Tile tile in tilesToRemove)
                {
                    AddScore(10 * comboMultiplier);

                    // Play effects
                    Vector3 effectPos = tile.transform.position;
                    Color tileColor = tile.spriteRenderer != null ? tile.spriteRenderer.color : Color.white;

                    grid[tile.row, tile.column] = null;
                    tile.gameObject.SetActive(false);
                }

                // Play match sound
                if (comboMultiplier > 1)
                    Debug.Log("Combo sound!");
                else
                    Debug.Log("Match sound!");

                comboMultiplier++;

                // Apply gravity
                yield return StartCoroutine(ApplyGravity());

                // Fill empty spaces
                yield return StartCoroutine(FillEmptySpaces());
            }

        } while (foundMatches);

        comboMultiplier = 1; // Reset combo multiplier
    }

    IEnumerator ApplyGravity()
    {
        bool moved = true;

        while (moved)
        {
            moved = false;

            for (int col = 0; col < columns; col++)
            {
                for (int row = rows - 1; row >= 0; row--)
                {
                    if (grid[row, col] == null)
                    {
                        // Find tile above to fall down
                        for (int aboveRow = row - 1; aboveRow >= 0; aboveRow--)
                        {
                            if (grid[aboveRow, col] != null)
                            {
                                // Move tile down
                                grid[row, col] = grid[aboveRow, col];
                                grid[aboveRow, col] = null;

                                grid[row, col].SetGridPosition(row, col);
                                Vector3 newPosition = new Vector3(col * tileSpacing, -row * tileSpacing, 0);
                                grid[row, col].MoveTo(newPosition);

                                moved = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Wait for all tiles to finish moving
            yield return new WaitUntil(() =>
            {
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        if (grid[row, col] != null && grid[row, col].IsMoving)
                            return false;
                    }
                }
                return true;
            });
        }
    }

    IEnumerator FillEmptySpaces()
    {
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (grid[row, col] == null)
                {
                    // Create new tile above the grid and let it fall
                    int tileType = Random.Range(0, tileTypeCount);
                    Vector3 startPosition = new Vector3(col * tileSpacing, rows * tileSpacing, 0);

                    GameObject tileObj = Instantiate(tilePrefab, startPosition, Quaternion.identity, gridParent);
                    Tile tile = tileObj.GetComponent<Tile>();
                    if (tile == null)
                        tile = tileObj.AddComponent<Tile>();

                    tile.SetGridPosition(row, col);
                    tile.SetTileType(tileType, tileSprites[tileType]);

                    Vector3 targetPosition = new Vector3(col * tileSpacing, -row * tileSpacing, 0);
                    tile.MoveTo(targetPosition);

                    grid[row, col] = tile;
                }
            }
        }

        // Wait for all new tiles to finish falling
        yield return new WaitUntil(() =>
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (grid[row, col] != null && grid[row, col].IsMoving)
                        return false;
                }
            }
            return true;
        });
    }

    void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score: {score}");
    }

    // Helper method to get hint for player
    public bool HasPossibleMoves()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Check all adjacent tiles
                int[] dRow = { -1, 1, 0, 0 };
                int[] dCol = { 0, 0, -1, 1 };

                for (int i = 0; i < 4; i++)
                {
                    int newRow = row + dRow[i];
                    int newCol = col + dCol[i];

                    if (IsValidPosition(newRow, newCol))
                    {
                        // Simulate swap
                        SwapTilesInGrid(row, col, newRow, newCol);

                        bool wouldMatch = IsPartOfMatch(row, col) || IsPartOfMatch(newRow, newCol);

                        // Swap back
                        SwapTilesInGrid(row, col, newRow, newCol);

                        if (wouldMatch)
                            return true;
                    }
                }
            }
        }
        return false;
    }

    void SwapTilesInGrid(int row1, int col1, int row2, int col2)
    {
        Tile temp = grid[row1, col1];
        grid[row1, col1] = grid[row2, col2];
        grid[row2, col2] = temp;

        if (grid[row1, col1] != null)
            grid[row1, col1].SetGridPosition(row1, col1);
        if (grid[row2, col2] != null)
            grid[row2, col2].SetGridPosition(row2, col2);
    }

    bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < columns;
    }
}
