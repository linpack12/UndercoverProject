using System;
using UnityEngine;

public class ZonableNPC : MonoBehaviour, IZonable
{
    public event Action<Zone> OnZoneAssigned;

    private Zone _currentZone;
    public Zone CurrentZone
    {
        get => _currentZone;
        set
        {
            _currentZone = value;
            OnZoneAssigned?.Invoke(_currentZone);
        }
    }

    public void OnZoneEntered(Zone zone)
    {
        _currentZone = zone; 
    }

    public void OnZoneExited(Zone zone)
    {
        if (_currentZone == zone)
        {
            _currentZone = null;
        }
    }
}
