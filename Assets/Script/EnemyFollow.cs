using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(AIDestinationSetter))]
public class EnemyFollow : MonoBehaviour
{
    private AIDestinationSetter destinationSetter;
    private Coroutine setTargetCoroutine;

    void Awake()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger entered, starting coroutine");
            
            // If a coroutine is already running, stop it first
            if (setTargetCoroutine != null)
                StopCoroutine(setTargetCoroutine);

            setTargetCoroutine = StartCoroutine(SetTargetAfterDelay(other.transform, 1f));
        }
    }

  

    private IEnumerator SetTargetAfterDelay(Transform target, float delay)
    {
        yield return new WaitForSeconds(delay);
        destinationSetter.target = target;
        Debug.Log("Target set after delay");
    }
}
