using System;
using System.Threading.Tasks;
using UnityEngine;

public class PerceptionModule : MonoBehaviour
{
    [SerializeField] private float viewRadius = 5f;
    [SerializeField] private LayerMask perceptionMask;

    private readonly Collider2D[] _results = new Collider2D[10];
    private ZonableNPC _zonable;

    private void Awake()
    {
        _zonable = GetComponent<ZonableNPC>();
    }

    public void Scan()
    {
        //Check zone status
        if (_zonable != null && _zonable.CurrentZone != null)
        {
            if (!ZoneManager.Instance.IsZoneActive(_zonable.CurrentZone))
            {
                return; //Skip percpetion
            }
        }

        int hits = Physics2D.OverlapCircleNonAlloc(transform.position, viewRadius, _results, perceptionMask);
        for (int i = 0; i < hits; i++)
        {
            var collider = _results[i];
            if(collider == null) continue;

            if(collider.TryGetComponent<IPerceivable>(out var perceivable) && perceivable.IsSuspicious)
            {
                RaiseObservationEvent(perceivable);
            }
        }
    }

    private void RaiseObservationEvent(IPerceivable target)
    {
        var e = new ObservationEvent(
            observer: gameObject,
            observed: target.Owner,
            actionType: target.ActionType
        );
        GameEventBus<ObservationEvent>.Raise(e); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}
