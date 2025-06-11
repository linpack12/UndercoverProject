using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class AlertListener : MonoBehaviour
{
    [SerializeField] private float alertRadius = 5f;
    [SerializeField] private float offsetRadius = 1f;

    private void OnEnable()
    {
        GameEventBus<SuspicionAlertEvent>.OnGameEvent += OnSuspicionAlert;
    }

    private void OnDisable()
    {
        GameEventBus<SuspicionAlertEvent>.OnGameEvent -= OnSuspicionAlert;
    }

    private void OnSuspicionAlert(SuspicionAlertEvent e)
    {
        //Get all guards in the radius
        int layerMask = 1 << LayerMask.NameToLayer("Guard");
        Collider2D[] hits = Physics2D.OverlapCircleAll(e.Location, alertRadius, layerMask);

        var guards = new List<GuardAI>();
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out GuardAI guard) && !guard.IsBusy)
            {
                guards.Add(guard);
            }
        }

        if (guards.Count == 0) { return; }
        //Sort based on distance to alert location
        guards.OrderBy(g => Vector3.Distance(g.transform.position, e.Location)).ToList();
        guards[0].Investigate(e.Location);
        Debug.Log($"[Alert] {guards[0].name} (primary) investigating {e.Location}");

        //Give other guards if any a offset to the alert location
        for (int i = 1; i < guards.Count; i++)
        {
            float angle = (360f / (guards.Count - 1)) * (i - 1);
            var offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f) * offsetRadius;
            var targetPos = e.Location + offset;
            guards[i].Investigate(targetPos);
            Debug.Log($"[Alert] {guards[i].name} backup investigating {targetPos}");
        }
    }

    /// <summary>
    /// Draw alert radius
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }
}
