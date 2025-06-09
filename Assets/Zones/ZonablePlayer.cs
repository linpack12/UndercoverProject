using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonablePlayer : MonoBehaviour, IZonable
{
    private HashSet<Zone> _currentZones = new();
    private PlayerIdentity _identity;

    private void Awake()
    {
        _identity = GetComponent<PlayerIdentity>();
    }

    public void OnZoneEntered(Zone zone)
    {
        _currentZones.Add(zone);
        ZoneManager.Instance?.RegisterActiveZone(zone);

        Debug.Log($"Player {_identity?.PlayerId} entered zone {zone.zoneId}");
    }

    public void OnZoneExited(Zone zone)
    {
        _currentZones.Remove(zone);
        if (!_currentZones.Contains(zone)) 
        {
            ZoneManager.Instance?.UnregisterActiveZone(zone);

            Debug.Log($"Player {_identity?.PlayerId} exited zone {zone.zoneId}");
        }
    }
}
