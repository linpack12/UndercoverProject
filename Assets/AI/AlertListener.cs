using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertListener : MonoBehaviour
{
    [SerializeField] private float alertRadius = 5f;

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
        int layerMask = 1 << LayerMask.NameToLayer("Guard");
        Collider2D[] hits = Physics2D.OverlapCircleAll(e.Location, alertRadius, layerMask);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out GuardAI guard))
            {
                guard.Investigate(e.Location);
            }
        }
    }
}
