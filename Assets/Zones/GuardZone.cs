using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class GuardZone : Zone
{
    public Transform[] patrolPoints;

    [HideInInspector]
    public GameObject CurrentGuard { get; set; }

    private void OnDrawGizmosSelected()
    {
        if (patrolPoints == null || patrolPoints.Length < 2)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        for (int i = 0; i < patrolPoints.Length - 1; i++)
        {
            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
        }

        Gizmos.color = Color.red;
        foreach (var point in patrolPoints)
        {
            Gizmos.DrawSphere(point.position, 0.1f);
        }
    }
}