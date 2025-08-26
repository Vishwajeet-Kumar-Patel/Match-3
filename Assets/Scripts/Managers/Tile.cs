using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Tile Data")]
    public int tileType;
    public int row;
    public int column;
    
    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer backgroundRenderer;
    
    [Header("Animation")]
    public float moveSpeed = 10f;
    
    private Vector3 targetPosition;
    private bool isMoving = false;
    
    public bool IsMoving => isMoving;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Set up background if it exists
        SetupBackground();

        targetPosition = transform.position;
        
        EnsureOverlayAndSorting();
    }


    public void EnsureOverlayAndSorting()
{
    // Main piece renderer
    if (spriteRenderer == null)
        spriteRenderer = GetComponent<SpriteRenderer>();
    if (spriteRenderer != null)
    {
        spriteRenderer.sortingLayerName = "gameplay";
        spriteRenderer.sortingOrder = 0;
    }

    // Overlay (child) renderer – shown above everything else
    if (backgroundRenderer == null)
    {
        // Try to find an existing child first
        Transform child = transform.Find("TileOverlay");
        if (child == null)
        {
            child = new GameObject("TileOverlay").transform;
            child.SetParent(transform, false);
            child.localPosition = Vector3.zero;
        }
        backgroundRenderer = child.GetComponent<SpriteRenderer>();
        if (backgroundRenderer == null)
            backgroundRenderer = child.gameObject.AddComponent<SpriteRenderer>();

        // Optional: give it a subtle tint if you haven't set a sprite yet
        backgroundRenderer.color = new Color(1f, 1f, 1f, 0.15f);
        // You can assign a proper overlay sprite in the Inspector later:
        // backgroundRenderer.sprite = overlaySprite;
    }

    // <<< The lines you asked about go here >>>
    // For the piece
    if (spriteRenderer != null)
    {
        spriteRenderer.sortingLayerName = "Pieces";
        spriteRenderer.sortingOrder = 0;
    }

    // For the grid overlay (on top)
    if (backgroundRenderer != null)
    {
        backgroundRenderer.sortingLayerName = "GridOverlay";
        backgroundRenderer.sortingOrder = 1000;
    }
}
    
    void SetupBackground()
{
    if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    // Pieces below
    spriteRenderer.sortingLayerName = "Pieces";
    spriteRenderer.sortingOrder = 0;

    // Find or create overlay child
    var child = transform.Find("TileOverlay");
    if (child == null)
    {
        child = new GameObject("TileOverlay").transform;
        child.SetParent(transform, false);
        child.localPosition = Vector3.zero;
    }
    backgroundRenderer = child.GetComponent<SpriteRenderer>();
    if (backgroundRenderer == null) backgroundRenderer = child.gameObject.AddComponent<SpriteRenderer>();

    // Assign your overlay sprite in the Inspector (e.g., a rounded square with some transparency)
    // backgroundRenderer.sprite = yourOverlaySprite;
    backgroundRenderer.color = new Color(1f, 1f, 1f, 0.15f); // subtle tint
    backgroundRenderer.sortingLayerName = "GridOverlay";
    backgroundRenderer.sortingOrder = 1000;

    // Make sure it visually matches your cell size (assuming 1 unit tiles):
    child.localScale = Vector3.one * 1.0f; // or Match3Manager.Instance.tileSpacing if you expose it static
}
    
    void Update()
    {
        // Smooth movement animation
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }
    
    public void SetTileType(int type, Sprite sprite)
    {
        tileType = type;
        if (spriteRenderer != null)
            spriteRenderer.sprite = sprite;
    }
    
    public void SetGridPosition(int r, int c)
    {
        row = r;
        column = c;
        name = $"Tile_{row}_{column}";
    }
    
    public void MoveTo(Vector3 newPosition)
    {
        targetPosition = newPosition;
        isMoving = true;
    }
    
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
        targetPosition = position;
        isMoving = false;
    }
    
    void OnMouseDown()
    {
        // Only respond to clicks if not moving and game is active
        if (!isMoving && Match3Manager.Instance != null)
        {
            Match3Manager.Instance.OnTileClicked(this);
        }
    }
    
    void OnMouseEnter()
    {
        // Add hover effect if desired
        if (spriteRenderer != null && !isMoving)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.gray, 0.2f);
        }
    }
    
    void OnMouseExit()
    {
        // Remove hover effect
        if (spriteRenderer != null && !isMoving)
        {
            spriteRenderer.color = Color.white;
        }
    }
}
