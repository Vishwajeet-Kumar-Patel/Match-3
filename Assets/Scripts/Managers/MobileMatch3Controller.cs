using UnityEngine;

public class MobileMatch3Controller : MonoBehaviour
{
    [Header("Mobile Touch Settings")]
    public float touchSensitivity = 1.0f;
    public float swipeThreshold = 50f;
    public bool enableHapticFeedback = true;
    public bool enableSwipeToMove = true;
    
    private Vector2 touchStartPos;
    private bool isTouching = false;
    private Tile lastTouchedTile = null;
    
    void Update()
    {
        HandleMobileInput();
    }
    
    void HandleMobileInput()
    {
        // Handle touch input for mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isTouching = true;
                    HandleTouchStart(touch.position);
                    break;
                    
                case TouchPhase.Moved:
                    if (isTouching && enableSwipeToMove)
                    {
                        HandleTouchMove(touch.position);
                    }
                    break;
                    
                case TouchPhase.Ended:
                    if (isTouching)
                    {
                        HandleTouchEnd(touch.position);
                    }
                    isTouching = false;
                    lastTouchedTile = null;
                    break;
            }
        }
        
        // Fallback to mouse for testing in editor
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isTouching = true;
            HandleTouchStart(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && isTouching)
        {
            HandleTouchEnd(Input.mousePosition);
            isTouching = false;
            lastTouchedTile = null;
        }
        #endif
    }
    
    void HandleTouchStart(Vector2 screenPosition)
    {
        // Convert screen position to world position
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
        
        // Find tile at this position
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPos);
        if (hitCollider != null)
        {
            Tile tile = hitCollider.GetComponent<Tile>();
            if (tile != null)
            {
                lastTouchedTile = tile;
                
                // Provide haptic feedback
                if (enableHapticFeedback)
                {
                    Handheld.Vibrate();
                }
            }
        }
    }
    
    void HandleTouchMove(Vector2 currentPosition)
    {
        if (lastTouchedTile == null) return;
        
        Vector2 swipeDirection = currentPosition - touchStartPos;
        
        if (swipeDirection.magnitude > swipeThreshold)
        {
            // Determine swipe direction
            Vector2 normalizedDirection = swipeDirection.normalized;
            
            // Convert to grid direction
            int deltaRow = 0;
            int deltaCol = 0;
            
            if (Mathf.Abs(normalizedDirection.x) > Mathf.Abs(normalizedDirection.y))
            {
                // Horizontal swipe
                deltaCol = normalizedDirection.x > 0 ? 1 : -1;
            }
            else
            {
                // Vertical swipe
                deltaRow = normalizedDirection.y > 0 ? 1 : -1;
            }
            
            // Try to swap with adjacent tile
            AttemptTileSwap(lastTouchedTile, deltaRow, deltaCol);
            
            // Reset to prevent multiple swipes
            touchStartPos = currentPosition;
        }
    }
    
    void HandleTouchEnd(Vector2 touchEndPos)
    {
        if (lastTouchedTile == null) return;
        
        Vector2 swipeDirection = touchEndPos - touchStartPos;
        
        // If it was a tap (minimal movement), just select the tile
        if (swipeDirection.magnitude <= swipeThreshold)
        {
            // Handle tap selection
            if (Match3Manager.Instance != null)
            {
                Match3Manager.Instance.OnTileClicked(lastTouchedTile);
            }
        }
    }
    
    void AttemptTileSwap(Tile tile, int deltaRow, int deltaCol)
    {
        if (Match3Manager.Instance == null) return;
        
        int targetRow = tile.row + deltaRow;
        int targetCol = tile.column + deltaCol;
        
        // Check bounds
        if (targetRow >= 0 && targetRow < 8 && targetCol >= 0 && targetCol < 8)
        {
            // Find target tile
            Tile[] allTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
            foreach (Tile targetTile in allTiles)
            {
                if (targetTile.row == targetRow && targetTile.column == targetCol)
                {
                    // Attempt swap through Match3Manager
                    Match3Manager.Instance.TrySwapTiles(tile, targetTile);
                    
                    if (enableHapticFeedback)
                    {
                        Handheld.Vibrate();
                    }
                    break;
                }
            }
        }
    }
}
