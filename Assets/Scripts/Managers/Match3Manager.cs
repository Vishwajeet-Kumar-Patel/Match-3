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
    [Tooltip("Gem prefab (must have SpriteRenderer and Tile script).")]
    public GameObject tilePrefab;
    [Tooltip("Parent transform for all gem tiles. Board will be centered on this transform.")]
    public Transform gridParent;

    [Header("Per-cell Background (Dark Plate Behind Gems)")]
    [Tooltip("Flat dark square prefab with SpriteRenderer. Renders behind gems, in front of scenic background.")]
    public GameObject tileBackgroundPrefab;
    [Tooltip("Optional parent for dark plates. If null, plates will be parented to gridParent.")]
    public Transform backgroundParent;
    [Tooltip("Sorting layer used for both plates and gems.")]
    public string gameplaySortingLayer = "gameplay";
    [Tooltip("Order for per-cell plates (behind gems).")]
    public int bgOrderInLayer = 0;
    [Tooltip("Order for gems (above plates).")]
    public int gemOrderInLayer = 10;

    [Header("Runtime Grid Info")]
    public int rows = 8;
    public int columns = 8;
    public int tileTypeCount = 5;
    public Sprite[] tileSprites;

    [Header("Game Settings")]
    [Tooltip("Distance between adjacent cells in world units. Also used as cell size.")]
    public float tileSpacing = 1f;
    public float fallSpeed = 10f;
    public float swapSpeed = 8f;

    [Header("Visual Tuning")]
    [Range(0.6f, 1f)] public float tileFillPercent = 0.9f;   // how much of the cell the gem fills
    [Range(0.8f, 1.05f)] public float backgroundFillPercent = 1.0f; // how much of the cell the plate fills

    [Header("Score")]
    public int score = 0;
    public int comboMultiplier = 1;

    // ---- internals ----
    private Tile[,] grid;
    private Tile selectedTile;
    private Tile targetTile;
    private bool isProcessing = false;

    // layout cache
    private Vector3 gridOrigin; // top-left cell center
    private float gemScale = 1f;
    private float plateScale = 1f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        LoadGridSettings();
        ComputeLayout();
        GenerateInitialGrid();
    }

    // --------------------------------------------------
    // Layout / sizing
    // --------------------------------------------------
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

    void ComputeLayout()
    {
        if (gridParent == null)
        {
            var go = new GameObject("GridParent");
            gridParent = go.transform;
            gridParent.position = Vector3.zero;
            Debug.LogWarning("[Match3] gridParent not assigned; created one at world origin.");
        }

        if (backgroundParent == null)
            backgroundParent = gridParent;

        // Center the grid around gridParent.position
        float width  = (columns - 1) * tileSpacing;
        float height = (rows    - 1) * tileSpacing;
        gridOrigin = gridParent.position + new Vector3(-width * 0.5f, height * 0.5f, 0f);

        // Compute instance scales from sprite bounds so things fit nicely per cell
        if (tilePrefab != null)
        {
            var sr = tilePrefab.GetComponent<SpriteRenderer>();
            if (sr && sr.sprite)
            {
                float maxSide = Mathf.Max(sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);
                if (maxSide > 0f)
                    gemScale = (tileSpacing * tileFillPercent) / maxSide;
            }
        }

        if (tileBackgroundPrefab != null)
        {
            var bs = tileBackgroundPrefab.GetComponent<SpriteRenderer>();
            if (bs && bs.sprite)
            {
                float maxSide = Mathf.Max(bs.sprite.bounds.size.x, bs.sprite.bounds.size.y);
                if (maxSide > 0f)
                    plateScale = (tileSpacing * backgroundFillPercent) / maxSide;
            }
        }
    }

    Vector3 GridToWorld(int row, int col)
    {
        // rows grow downward
        float x = gridOrigin.x + col * tileSpacing;
        float y = gridOrigin.y - row * tileSpacing;
        return new Vector3(x, y, 0f);
    }

    // --------------------------------------------------
    // Grid creation
    // --------------------------------------------------
    void GenerateInitialGrid()
    {
        // Clear old tiles if any
        if (backgroundParent != null)
        {
            for (int i = backgroundParent.childCount - 1; i >= 0; i--)
                DestroyImmediate(backgroundParent.GetChild(i).gameObject);
        }
        if (gridParent != null)
        {
            for (int i = gridParent.childCount - 1; i >= 0; i--)
                DestroyImmediate(gridParent.GetChild(i).gameObject);
        }

        // Generate grid without initial matches
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = GridToWorld(row, col);

                // 1) Per-cell background plate (behind gems, above scenic background)
                if (tileBackgroundPrefab)
                {
                    GameObject plate = Instantiate(tileBackgroundPrefab, pos, Quaternion.identity, backgroundParent);
                    plate.transform.localScale = Vector3.one * plateScale;

                    var bsr = plate.GetComponent<SpriteRenderer>();
                    if (bsr)
                    {
                        bsr.sortingLayerName = gameplaySortingLayer; // e.g., "gameplay"
                        bsr.sortingOrder     = bgOrderInLayer;       // 0
                    }
                }

                // 2) Gem
                CreateTileAt(row, col, GetRandomTileType(row, col), pos);
            }
        }

        // Re-roll any initial matches
        while (HasMatches())
        {
            for (int row = 0; row < rows; row++)
            for (int col = 0; col < columns; col++)
            {
                if (IsPartOfMatch(row, col))
                {
                    int t = GetRandomTileType(row, col);
                    grid[row, col].SetTileType(t, tileSprites[t]);
                }
            }
        }
    }

    void CreateTileAt(int row, int col, int tileType) =>
        CreateTileAt(row, col, tileType, GridToWorld(row, col));

    void CreateTileAt(int row, int col, int tileType, Vector3 position)
    {
        GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, gridParent);
        tileObj.transform.localScale = Vector3.one * gemScale;

        // Ensure sorting for gems
        var sr = tileObj.GetComponent<SpriteRenderer>();
        if (sr)
        {
            sr.sortingLayerName = gameplaySortingLayer; // e.g., "gameplay"
            sr.sortingOrder     = gemOrderInLayer;      // e.g., 10
        }

        Tile tile = tileObj.GetComponent<Tile>();
        if (tile == null) tile = tileObj.AddComponent<Tile>();

        tile.SetGridPosition(row, col);
        tile.SetTileType(tileType, tileSprites[tileType]);
        tile.SetPosition(position);

        grid[row, col] = tile;
    }

    int GetRandomTileType(int row, int col)
    {
        List<int> validTypes = new List<int>();
        for (int i = 0; i < tileTypeCount; i++) validTypes.Add(i);

        // Prevent immediate horizontal match
        if (col >= 2 && grid[row, col - 1] != null && grid[row, col - 2] != null)
        {
            if (grid[row, col - 1].tileType == grid[row, col - 2].tileType)
                validTypes.Remove(grid[row, col - 1].tileType);
        }

        // Prevent immediate vertical match
        if (row >= 2 && grid[row - 1, col] != null && grid[row - 2, col] != null)
        {
            if (grid[row - 1, col].tileType == grid[row - 2, col].tileType)
                validTypes.Remove(grid[row - 1, col].tileType);
        }

        return validTypes[Random.Range(0, validTypes.Count)];
    }

    // --------------------------------------------------
    // Input & swap (unchanged logic except positions use GridToWorld)
    // --------------------------------------------------
    public void OnTileClicked(Tile clickedTile)
    {
        if (isProcessing) return;

        if (selectedTile == null)
        {
            selectedTile = clickedTile;
            HighlightTile(selectedTile, true);
        }
        else if (selectedTile == clickedTile)
        {
            HighlightTile(selectedTile, false);
            selectedTile = null;
        }
        else if (AreAdjacent(selectedTile, clickedTile))
        {
            targetTile = clickedTile;
            StartCoroutine(SwapTiles(selectedTile, targetTile));
        }
        else
        {
            HighlightTile(selectedTile, false);
            selectedTile = clickedTile;
            HighlightTile(selectedTile, true);
        }
    }

    void HighlightTile(Tile tile, bool highlight)
    {
        if (tile != null && tile.spriteRenderer != null)
            tile.spriteRenderer.color = highlight ? Color.yellow : Color.white;
    }

    bool AreAdjacent(Tile tile1, Tile tile2)
    {
        int rowDiff = Mathf.Abs(tile1.row - tile2.row);
        int colDiff = Mathf.Abs(tile1.column - tile2.column);
        return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
    }

    public bool TrySwapTiles(Tile tile1, Tile tile2)
    {
        if (isProcessing) return false;
        if (tile1 == null || tile2 == null) return false;
        if (!AreAdjacent(tile1, tile2)) return false;

        if (selectedTile != null) HighlightTile(selectedTile, false);

        selectedTile = tile1;
        targetTile   = tile2;
        StartCoroutine(SwapTiles(tile1, tile2));
        return true;
    }

    IEnumerator SwapTiles(Tile tile1, Tile tile2)
    {
        isProcessing = true;
        HighlightTile(selectedTile, false);

        // Swap positions in grid array
        grid[tile1.row, tile1.column] = tile2;
        grid[tile2.row, tile2.column] = tile1;

        // Swap grid coordinates
        int tempRow = tile1.row;
        int tempCol = tile1.column;
        tile1.SetGridPosition(tile2.row, tile2.column);
        tile2.SetGridPosition(tempRow, tempCol);

        // Animate swap using world positions from our centered layout
        Vector3 tile1Target = GridToWorld(tile1.row, tile1.column);
        Vector3 tile2Target = GridToWorld(tile2.row, tile2.column);

        tile1.MoveTo(tile1Target);
        tile2.MoveTo(tile2Target);

        yield return new WaitUntil(() => !tile1.IsMoving && !tile2.IsMoving);

        if (HasMatches())
        {
            yield return StartCoroutine(ProcessMatches());
        }
        else
        {
            // Invalid move; swap back
            grid[tile1.row, tile1.column] = tile2;
            grid[tile2.row, tile2.column] = tile1;

            int tempRow2 = tile1.row;
            int tempCol2 = tile1.column;
            tile1.SetGridPosition(tile2.row, tile2.column);
            tile2.SetGridPosition(tempRow2, tempCol2);

            tile1Target = GridToWorld(tile1.row, tile1.column);
            tile2Target = GridToWorld(tile2.row, tile2.column);

            tile1.MoveTo(tile1Target);
            tile2.MoveTo(tile2Target);

            yield return new WaitUntil(() => !tile1.IsMoving && !tile2.IsMoving);
        }

        selectedTile = null;
        targetTile = null;
        isProcessing = false;
    }

    // --------------------------------------------------
    // Match / resolve (unchanged logic; positions come from GridToWorld)
    // --------------------------------------------------
    bool HasMatches()
    {
        for (int row = 0; row < rows; row++)
        for (int col = 0; col < columns; col++)
            if (IsPartOfMatch(row, col)) return true;
        return false;
    }

    bool IsPartOfMatch(int row, int col)
    {
        if (grid[row, col] == null) return false;
        int tileType = grid[row, col].tileType;

        int horizontalCount = 1;
        for (int i = col - 1; i >= 0; i--) { if (grid[row, i] != null && grid[row, i].tileType == tileType) horizontalCount++; else break; }
        for (int i = col + 1; i < columns; i++) { if (grid[row, i] != null && grid[row, i].tileType == tileType) horizontalCount++; else break; }
        if (horizontalCount >= 3) return true;

        int verticalCount = 1;
        for (int i = row - 1; i >= 0; i--) { if (grid[i, col] != null && grid[i, col].tileType == tileType) verticalCount++; else break; }
        for (int i = row + 1; i < rows; i++) { if (grid[i, col] != null && grid[i, col].tileType == tileType) verticalCount++; else break; }
        return verticalCount >= 3;
    }

    IEnumerator ProcessMatches()
    {
        bool foundMatches;
        do
        {
            foundMatches = false;
            List<Tile> tilesToRemove = new List<Tile>();

            for (int row = 0; row < rows; row++)
            for (int col = 0; col < columns; col++)
            {
                if (IsPartOfMatch(row, col))
                {
                    tilesToRemove.Add(grid[row, col]);
                    foundMatches = true;
                }
            }

            if (foundMatches)
            {
                foreach (Tile tile in tilesToRemove)
                {
                    AddScore(10 * comboMultiplier);
                    grid[tile.row, tile.column] = null;
                    tile.gameObject.SetActive(false);
                }

                comboMultiplier++;
                yield return StartCoroutine(ApplyGravity());
                yield return StartCoroutine(FillEmptySpaces());
            }

        } while (foundMatches);

        comboMultiplier = 1;
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
                        for (int aboveRow = row - 1; aboveRow >= 0; aboveRow--)
                        {
                            if (grid[aboveRow, col] != null)
                            {
                                grid[row, col] = grid[aboveRow, col];
                                grid[aboveRow, col] = null;

                                grid[row, col].SetGridPosition(row, col);
                                Vector3 newPosition = GridToWorld(row, col);
                                grid[row, col].MoveTo(newPosition);

                                moved = true;
                                break;
                            }
                        }
                    }
                }
            }

            yield return new WaitUntil(() =>
            {
                for (int r = 0; r < rows; r++)
                for (int c = 0; c < columns; c++)
                    if (grid[r, c] != null && grid[r, c].IsMoving) return false;
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
                    int tileType = Random.Range(0, tileTypeCount);
                    Vector3 startPosition = GridToWorld(-1, col); // one cell above
                    Vector3 targetPosition = GridToWorld(row, col);

                    GameObject tileObj = Instantiate(tilePrefab, startPosition, Quaternion.identity, gridParent);
                    tileObj.transform.localScale = Vector3.one * gemScale;

                    var sr = tileObj.GetComponent<SpriteRenderer>();
                    if (sr)
                    {
                        sr.sortingLayerName = gameplaySortingLayer;
                        sr.sortingOrder     = gemOrderInLayer;
                    }

                    Tile tile = tileObj.GetComponent<Tile>();
                    if (tile == null) tile = tileObj.AddComponent<Tile>();

                    tile.SetGridPosition(row, col);
                    tile.SetTileType(tileType, tileSprites[tileType]);
                    tile.MoveTo(targetPosition);

                    grid[row, col] = tile;
                }
            }
        }

        yield return new WaitUntil(() =>
        {
            for (int r = 0; r < rows; r++)
            for (int c = 0; c < columns; c++)
                if (grid[r, c] != null && grid[r, c].IsMoving) return false;
            return true;
        });
    }

    void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score: {score}");
    }

    public bool HasPossibleMoves()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int[] dRow = { -1, 1, 0, 0 };
                int[] dCol = { 0, 0, -1, 1 };

                for (int i = 0; i < 4; i++)
                {
                    int newRow = row + dRow[i];
                    int newCol = col + dCol[i];

                    if (IsValidPosition(newRow, newCol))
                    {
                        SwapTilesInGrid(row, col, newRow, newCol);
                        bool wouldMatch = IsPartOfMatch(row, col) || IsPartOfMatch(newRow, newCol);
                        SwapTilesInGrid(row, col, newRow, newCol);
                        if (wouldMatch) return true;
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

        if (grid[row1, col1] != null) grid[row1, col1].SetGridPosition(row1, col1);
        if (grid[row2, col2] != null) grid[row2, col2].SetGridPosition(row2, col2);
    }

    bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < columns;
    }
}
