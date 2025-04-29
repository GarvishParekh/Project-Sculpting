using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectRotation : MonoBehaviour
{
    public float rotationSpeed = 0.2f;
    public float swipeThreshold = 5f; // Minimum pixels to consider as a swipe

    private Vector2 lastTouchPosition;
    private bool isDragging = false;

    void Update()
    {
        if (Input.touchCount > 0) // Mobile
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - lastTouchPosition;
                if (delta.magnitude > swipeThreshold)
                {
                    RotateObject(delta);
                    lastTouchPosition = touch.position;
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        else if (Input.GetMouseButtonDown(0)) // Editor / PC
        {
            lastTouchPosition = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastTouchPosition;
            if (delta.magnitude > swipeThreshold)
            {
                RotateObject(delta);
                lastTouchPosition = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void RotateObject(Vector2 swipeDelta)
    {
        float rotationY = -swipeDelta.x * rotationSpeed;
        float rotationX = swipeDelta.y * rotationSpeed;

        transform.Rotate(Vector3.up, rotationY, Space.World);
        transform.Rotate(Vector3.right, rotationX, Space.World);
    }

    public void _ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
