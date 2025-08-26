using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    
    [Header("Input Settings")]
    public bool useTouch = true;
    public bool useMouse = true;
    public float swipeThreshold = 50f;
    
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isTouching = false;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Handle mouse input
        if (useMouse)
        {
            HandleMouseInput();
        }
        
        // Handle touch input
        if (useTouch && Input.touchCount > 0)
        {
            HandleTouchInput();
        }
    }
    
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            HandleClick(mousePos);
        }
    }
    
    void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0);
        
        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = Camera.main.ScreenToWorldPoint(touch.position);
                isTouching = true;
                break;
                
            case TouchPhase.Ended:
                if (isTouching)
                {
                    touchEndPos = Camera.main.ScreenToWorldPoint(touch.position);
                    HandleSwipe();
                }
                isTouching = false;
                break;
                
            case TouchPhase.Canceled:
                isTouching = false;
                break;
        }
    }
    
    void HandleClick(Vector2 worldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        
        if (hit.collider != null)
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null && Match3Manager.Instance != null)
            {
                Match3Manager.Instance.OnTileClicked(tile);
            }
        }
    }
    
    void HandleSwipe()
    {
        Vector2 swipeDirection = touchEndPos - touchStartPos;
        
        if (swipeDirection.magnitude > swipeThreshold)
        {
            // Determine swipe direction
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                // Horizontal swipe
                if (swipeDirection.x > 0)
                    Debug.Log("Swipe Right");
                else
                    Debug.Log("Swipe Left");
            }
            else
            {
                // Vertical swipe
                if (swipeDirection.y > 0)
                    Debug.Log("Swipe Up");
                else
                    Debug.Log("Swipe Down");
            }
        }
        else
        {
            // Treat as tap
            HandleClick(touchStartPos);
        }
    }
}
