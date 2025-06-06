using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0, 360)]
    public float horizontalAngle;
    [Range(0, 180)]
    public float verticalAngle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length > 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Горизонтальний кут
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 flatDirection = Vector3.ProjectOnPlane(directionToTarget, Vector3.up).normalized;
            float horizontalDiff = Vector3.Angle(flatForward, flatDirection);

            // Вертикальний кут
            Vector3 upForward = Vector3.ProjectOnPlane(transform.forward, transform.right).normalized;
            Vector3 upDirection = Vector3.ProjectOnPlane(directionToTarget, transform.right).normalized;
            float verticalDiff = Vector3.Angle(upForward, upDirection);

            if (horizontalDiff < horizontalAngle / 2 && verticalDiff < verticalAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    return;
                }
            }
        }

        canSeePlayer = false;
    }
}
