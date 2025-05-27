using UnityEngine;
using Pathfinding;
using System.Collections;

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
    void Awake()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiLerp = GetComponent<AILerp>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger entered, starting coroutine");
            playerAnimator=other.gameObject.GetComponent<Animator>();
            if (setTargetCoroutine != null)
                StopCoroutine(setTargetCoroutine);

            setTargetCoroutine = StartCoroutine(SetTargetAfterDelay(other.transform, 1f));
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

            if (distanceToTarget <= 0.3f)
            {
                aiLerp.canMove = false; // Stop moving
                RotateTowardsMovement(0);
                animator.SetBool("IsAttack", true);
                animator.SetBool("IsMoving", false);
                playerAnimator.SetBool("IsHurted", true);
            }
            else
            {
                aiLerp.canMove = true; // Resume moving
                RotateTowardsMovement(90);
                animator.SetBool("IsAttack", false);
                animator.SetBool("IsMoving", true);
                playerAnimator.SetBool("IsHurted", false);
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
