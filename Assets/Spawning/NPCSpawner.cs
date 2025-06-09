using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [System.Serializable]
    public class RolePrefab
    {
        public string role;
        public GameObject prefab;
    }

    public List<RolePrefab> rolePrefabs;
    public List<Zone> zones;
    public int npcCount = 10;

    private Dictionary<string, GameObject> rolePrefabMap = new();
    public List<TraitDefinition> availableTraits;
    private List<Vector3> usedPositions = new();

    //Cache for zones per role 
    private Dictionary<string, List<Zone>> roleZoneCache = new();
    private HashSet<Zone> usedGuardZones = new();

    private void Awake()
    {
        LoadTraitDefinitions();
        BuildRolePrefabMap();
        CacheRoleZones();
        SpawnAllNPCs();
    }

    private void LoadTraitDefinitions()
    {
        availableTraits = new List<TraitDefinition>(Resources.LoadAll<TraitDefinition>("Traits"));
        Debug.Log($"Loaded {availableTraits.Count} trait definitions from Resources/Traits");
    }

    private void BuildRolePrefabMap()
    {
        foreach (var rp in rolePrefabs)
        {
            rolePrefabMap.TryAdd(rp.role, rp.prefab);
        }
    }

    private void CacheRoleZones()
    {
        foreach (var rp in rolePrefabs)
        {
            var prefabIsGuard = rp.prefab.GetComponent<GuardAI>() != null;
            if (prefabIsGuard)
            {
                //Guards can only be assigned guard zones
                roleZoneCache[rp.role] = zones.OfType<GuardZone>().Where(z => z != null && z.allowedRoles != null && z.allowedRoles.Contains(rp.role)).Cast<Zone>().ToList();
            }
            else
            {
                //Normal NPC get default zones
                roleZoneCache[rp.role] = zones.Where(z => z != null && z.allowedRoles != null && !(z is GuardZone) && z.allowedRoles.Contains(rp.role)).ToList();
            }
        }
    }

    private void SpawnAllNPCs()
    {
        for (int i = 0; i < npcCount; i++)
        {
            SpawnNPC();
        }
    }

    private void SpawnNPC()
    {
        string role = GetRandomRole();
        if (!rolePrefabMap.TryGetValue(role, out GameObject prefab)) { return; }
        if (!roleZoneCache.TryGetValue(role, out List<Zone> validZones) || validZones.Count == 0) { return; }

        bool isGuard = prefab.GetComponent<GuardAI>() != null;
        if (isGuard)
        {
            Debug.Log($"Trying to spawn guard for role: {role}");
            var availableGuardZones = validZones.Cast<GuardZone>().Where(z => z.CurrentGuard == null).ToList();
            if (availableGuardZones.Count == 0)
            {
                return;
            }

            GuardZone zone = availableGuardZones[Random.Range(0, availableGuardZones.Count)];
            Vector3 spawnPos = GetUniquePosition(zone);

            GameObject npc = Instantiate(prefab, spawnPos, Quaternion.identity);
            npc.name = $"{role}_{Random.Range(1000, 9999)}";
            var zonable = npc.GetComponent<ZonableNPC>();
            if (zonable != null)
            {
                zonable.CurrentZone = zone;
            }
            zone.CurrentGuard = npc;
        }
        else
        {
            Zone zone = validZones[Random.Range(0, validZones.Count)];
            Vector3 spawnPos = GetUniquePosition(zone);

            GameObject npc = Instantiate(prefab, spawnPos, Quaternion.identity);
            npc.name = $"{role}_{Random.Range(1000, 9999)}";

            AssignRandomTraits(npc);
        }
    }

    private string GetRandomRole()
    {
        return rolePrefabs[Random.Range(0, rolePrefabs.Count)].role;
    }

    private Vector3 GetUniquePosition(Zone zone)
    {
        const int maxTries = 10;
        const float minDistance = 0.05f; 

        for (int i = 0; i < maxTries; i++)
        {
            Vector3 pos = zone.GetRandomPositionInZone();
            if (!usedPositions.Exists(p => Vector3.Distance(p, pos) < minDistance))
            {
                usedPositions.Add(pos);
                return pos;
            }
        }
        return zone.GetRandomPositionInZone();
    }

    private void AssignRandomTraits(GameObject npc)
    {
        var traitModule = npc.GetComponent<TraitModule>();
        if (traitModule == null ||availableTraits.Count == 0) { return; }

        int traitCount = Mathf.Clamp(Random.Range(1, 4), 1, availableTraits.Count);

        //Fisher yates shuffle, should make this a method for later
        var shuffledIndices = Enumerable.Range(0, availableTraits.Count).ToList();

        for (int i = 0; i < traitCount; i++)
        {
            int randomIndex = Random.Range(i, shuffledIndices.Count);
            (shuffledIndices[i], shuffledIndices[randomIndex]) = (shuffledIndices[randomIndex], shuffledIndices[i]);

            TraitDefinition trait = availableTraits[shuffledIndices[i]];
            trait.ApplyTo(traitModule.Context);
            Debug.Log($"{npc.name} received trait: {trait.name}");
        }
    }
}
