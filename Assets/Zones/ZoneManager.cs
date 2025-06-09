using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

    private readonly HashSet<Zone> _activeZones = new();
    private readonly List<Zone> _allZones = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; 
        }

        Instance = this;

        //Gather all the zones in the current scene
        _allZones.AddRange(FindObjectsOfType<Zone>());
    }

    public void RegisterActiveZone(Zone zone)
    {
        _activeZones.Add(zone);
    }

    public void UnregisterActiveZone(Zone zone)
    {
        _activeZones.Remove(zone);
    }

    public bool IsZoneActive(Zone zone) => _activeZones.Contains(zone);

    public IReadOnlyCollection<Zone> GetActiveZones() => _activeZones;
}
