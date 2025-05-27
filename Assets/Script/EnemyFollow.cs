using UnityEngine;
using Pathfinding;
using System.Collections;
using Unity.Cinemachine;

[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(AILerp))]
[RequireComponent(typeof(Animator))]
public class EnemyFollow : MonoBehaviour
{
    private AIDestinationSetter destinationSetter;
    private AILerp aiLerp;
    private Animator animator;
    private Coroutine setTargetCoroutine;
    public Animator playerAnimator;
    public AudioSource audioSource;
    public AudioClip kickSound;
    public float waitForSecond=2;
    public float enemySpeed;


    void Awake()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiLerp = GetComponent<AILerp>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        aiLerp.speed = enemySpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerAnimator=other.gameObject.GetComponent<Animator>();
            if (setTargetCoroutine != null)
                StopCoroutine(setTargetCoroutine);

            setTargetCoroutine = StartCoroutine(SetTargetAfterDelay(other.transform, waitForSecond));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Trigger Stay: " + collision.name);
            var player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.reduceHealth();
            }
            else
            {
                Debug.LogWarning("PlayerMovement not found!");
            }
        }
    }

    private IEnumerator SetTargetAfterDelay(Transform target, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("IsMoving", true);
        destinationSetter.target = target;
    }

    void Update()
    {
       

        if (destinationSetter.target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, destinationSetter.target.position);

            if (distanceToTarget <= 0.2f)
            {
                aiLerp.canMove = false; // Stop moving
                RotateTowardsMovement(0);
                animator.SetBool("IsAttack", true);
                animator.SetBool("IsMoving", false);
                playerAnimator.SetBool("IsHurted", true);

                if (!audioSource.isPlaying)
                {
                    audioSource.clip = kickSound;
                    audioSource.loop = false;
                    audioSource.Play();
                }

            }
            else
            {
                aiLerp.canMove = true; // Resume moving
                RotateTowardsMovement(90);
                animator.SetBool("IsAttack", false);
                animator.SetBool("IsMoving", true);
                playerAnimator.SetBool("IsHurted", false);

             
                if (audioSource.isPlaying)
                    audioSource.Stop();
            }
        }
    }


    private void RotateTowardsMovement(float offset=90)
    {
        Vector2 velocity = aiLerp.velocity;

        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - offset);
        }
    }
}
