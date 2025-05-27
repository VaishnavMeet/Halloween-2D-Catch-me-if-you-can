using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // Degrees per second
    public float rotationTriggerDistance = 1f; // Distance before reaching target to start rotating

    private Vector2 moveDirection = Vector2.zero;
    private bool isMoving = false;

    private Rigidbody2D rb;

    private Vector2 targetPosition;

    private Vector2 touchStart;
    private Vector2 touchEnd;
    private float minSwipeDistance = 50f;

    public LayerMask wallLayer;

    private Quaternion targetRotation;
    private bool rotationStarted = false;

    private Animator animator;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        targetPosition = rb.position;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (!isMoving)
        {
            if (!audioSource.isPlaying)
                        audioSource.enabled = false;
            DetectSwipe();
        }
        else
        {
            animator.SetBool("IsMoving", true); 
            MoveTowardsTarget();
        }
    }

    void DetectSwipe()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            touchStart = Input.mousePosition;
        if (Input.GetMouseButtonUp(0))
        {
            touchEnd = Input.mousePosition;
            HandleSwipe();
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                touchStart = touch.position;
            if (touch.phase == TouchPhase.Ended)
            {
                touchEnd = touch.position;
                HandleSwipe();
            }
        }
#endif
    }

    void HandleSwipe()
    {
        Vector2 swipe = touchEnd - touchStart;
        if (swipe.magnitude < minSwipeDistance) return;

        swipe.Normalize();

        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
            moveDirection = swipe.x > 0 ? Vector2.right : Vector2.left;
        else
            moveDirection = swipe.y > 0 ? Vector2.up : Vector2.down;

        // Set target rotation based on direction
        if (moveDirection == Vector2.up)
            targetRotation = Quaternion.Euler(0, 0, 180);
        else if (moveDirection == Vector2.down)
            targetRotation = Quaternion.Euler(0, 0, 0);
        else if (moveDirection == Vector2.left)
            targetRotation = Quaternion.Euler(0, 0, -90);
        else if (moveDirection == Vector2.right)
            targetRotation = Quaternion.Euler(0, 0, 90);

        float maxDistance = 100f;
        float safeDistance = 0.1725f;

        RaycastHit2D hit = Physics2D.Raycast(rb.position, moveDirection, maxDistance, wallLayer);

        if (hit.collider != null)
        {
            float distanceToWall = hit.distance;
            float moveDistance = Mathf.Max(0, distanceToWall - safeDistance);
            targetPosition = rb.position + moveDirection * moveDistance;
        }
        else
        {
            targetPosition = rb.position + moveDirection * maxDistance;
        }

        isMoving = true;
        rotationStarted = false;
    }

    void MoveTowardsTarget()
    {
        // Move position smoothly
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime));

        float distanceLeft = Vector2.Distance(rb.position, targetPosition);

        // Start rotating if close enough
        if (distanceLeft <= rotationTriggerDistance)
        {

            animator.SetBool("IsMoving", false);
            rotationStarted = true;
        }

        if (rotationStarted)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            audioSource.enabled = true;
        }

        // Stop when close to target
        if (distanceLeft < 0.01f)
        {
            rb.position = targetPosition;
            transform.rotation = targetRotation;
            isMoving = false;
        }
    }
}
