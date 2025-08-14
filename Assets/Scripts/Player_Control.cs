using UnityEngine;
using UnityEngine.EventSystems; // Required for UI check

public class Player_Control : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float leftLimit = -5f;
    public float rightLimit = 5f;
    private Game_System gameSystem;
    private Camera mainCamera;
    private float targetX;

    void Start()
    {
        mainCamera = Camera.main;
        targetX = transform.position.x;

        gameSystem = Game_System.Instance;
        if (gameSystem == null)
        {
            Debug.LogWarning("No Game_System found in the scene.");
        }
    }

    void Update()
    {
        if (IsPointerOverUI())
            return;

        bool isHolding = Input.GetMouseButton(0) || Input.touchCount > 0;

        if (isHolding)
        {
            Vector3 inputPosition;

            if (Input.touchCount > 0)
                inputPosition = Input.GetTouch(0).position;
            else
                inputPosition = Input.mousePosition;

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(inputPosition);
            targetX = Mathf.Clamp(worldPosition.x, leftLimit, rightLimit);
        }

        float newX = Mathf.Lerp(transform.position.x, targetX, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        if ((Input.GetMouseButtonUp(0) || IsTouchReleased()) && !IsPointerOverUI())
        {
            if (gameSystem != null)
                gameSystem.Spawn();
            else
                Debug.LogWarning("GameSystem reference not set in Player_Control.");
        }
    }

    private bool IsTouchReleased()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Ended)
                return true;
        }
        return false;
    }

    private bool IsPointerOverUI()
    {
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            return EventSystem.current.IsPointerOverGameObject(); // For mouse
        }
        else if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId); // For touch
        }

        return false;
    }
}
