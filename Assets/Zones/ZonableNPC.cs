using UnityEngine;

public class ZonableNPC : MonoBehaviour, IZonable
{
    private Zone _currentZone;
    public Zone CurrentZone => _currentZone; 

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
