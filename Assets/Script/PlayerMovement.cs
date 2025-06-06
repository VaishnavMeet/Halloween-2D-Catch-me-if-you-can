using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // Degrees per second
    public float rotationTriggerDistance = 1f; // Distance before reaching target to start rotating
     int health = 15;
    public bool isHealthReducing=false;

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

    [Header("Health Bar")]
    public Image healthBarImage; // Drag the fill image here in the Inspector


    [Header("Audio Setup")]
    public AudioSource audioSource;
    public AudioClip keyCollectSound;
    public AudioClip bounceSound;

    GameObject gameManager;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        targetPosition = rb.position;
        targetRotation = transform.rotation;
        gameManager = GameObject.FindGameObjectWithTag("Manager");
    }

    public void reduceHealth()
    {
        if(!isHealthReducing) 
        StartCoroutine(reduceHelthByOne());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Win"))
        {
            StartCoroutine(HandleWin());
        }

        if (other.CompareTag("Bronze") || other.CompareTag("Sliver") || other.CompareTag("Gold"))
        {
            StartCoroutine(FadeAndDestroy(other.gameObject));
        }
    }

    private IEnumerator HandleWin()
    {
        yield return new WaitForSeconds(1f);
        gameManager.GetComponent<GameManager>().LoadNextLevel();
    }


    private IEnumerator reduceHelthByOne()
    {
        isHealthReducing = true;

        if (health <= 0)
        {
            Debug.Log("GameOver");
            yield return new WaitForSeconds(1f);
            gameManager.GetComponent<GameManager>().RestartLevel();
        }
        else
        {
            health--;
            float fill = health / 15f;
            if (healthBarImage != null)
                healthBarImage.fillAmount = fill;
        }

        yield return new WaitForSeconds(0.1f);
        isHealthReducing = false;
    }

    private IEnumerator FadeAndDestroy(GameObject obj)
    {
        obj.GetComponent<BoxCollider2D>().enabled= false;
        audioSource.PlayOneShot(keyCollectSound);
        SpriteRenderer spriteRenderer=obj.GetComponent<SpriteRenderer>();
        float duration = 0.5f;
        float t = 0f;
        Color originalColor = spriteRenderer.color;
        Vector3 startPos = obj.transform.position;
        Vector3 endPos = startPos + new Vector3(0f, 1f, 0f); // Move 1 unit upward

        while (t < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            obj.transform.position = Vector3.Lerp(startPos, endPos, t / duration); // Move up

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(obj);
    }

    void Update()
    {
        if (!isMoving)
        {
            
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
            
        }

        // Stop when close to target
        if (distanceLeft < 0.01f)
        {
            rb.position = targetPosition;
            transform.rotation = targetRotation;
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(bounceSound);
            isMoving = false;
        }
    }
}
