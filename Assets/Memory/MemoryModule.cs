using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TraitModule))]
public class MemoryModule : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float confidenceDecayPerMemory = 0.05f;
    [SerializeField] private float passiveDecayInterval = 2f; 
    [SerializeField] private float gossipThreshold = 0.5f; 
    [SerializeField] private float gossipRadius = 5f;

    [SerializeField] private float suspicionThreshold = 0.3f;
    [SerializeField] private float suspicionDecayRate = 0.02f;

    private readonly List<MemoryEntry> _memories = new();
    private readonly HashSet<GossipKey> _gossiped = new();
    private Dictionary<(int, string), float> suspicionScores = new();

    public delegate void MemoryUpdated(MemoryEntry updated);
    public event Action<MemoryEntry> OnMemoryAdded;

    private TraitModule traitModule;
    private NPCTraitContext context;

    private void Awake()
    {
        traitModule = GetComponent<TraitModule>();
        context = traitModule.Context;
    }

    private void Start()
    {
        InvokeRepeating(nameof(PassiveDecay), passiveDecayInterval, passiveDecayInterval);
    }

    public void Remember(GameObject observed, string action)
    {
        int id = observed.GetInstanceID();
        _ = (id, action);
        RegisterSuspicion(observed, action);

        //Check if already observed 
        if(_memories.Exists(m => m.ObservedInstanceId == id && m.ActionType == action))
        {
            return; 
        }

        float decay = confidenceDecayPerMemory * context.ConfidenceDecayMultiplier;

        for (int i = 0; i < _memories.Count; i++)
        {
            var m = _memories[i];
            m.Confidence = Mathf.Clamp01(m.Confidence - decay);
            _memories[i] = m;
            OnMemoryAdded?.Invoke(m);
        }
        var newMemory = new MemoryEntry(observed, action, 1f);
        _memories.Add(newMemory);
        OnMemoryAdded?.Invoke(newMemory);

        Debug.Log($"[{name}] Remembered: {action} by {observed.name}. Total memories: {_memories.Count}");

        TryGossip(observed, new GossipKey(id, action));
    }

    private void RegisterSuspicion(GameObject observed, string action)
    {
        var key = (observed.GetInstanceID(), action);

        if (!suspicionScores.ContainsKey(key))
        {
            suspicionScores[key] = 0f;
        }

        suspicionScores[key] = suspicionScores[key] * 1.5f + 0.1f;
        Debug.Log($"[{name}] Suspicion score for {observed.name} doing {action}: {suspicionScores[key]:F2}");

        if (suspicionScores[key] >= suspicionThreshold)
        {
            Debug.LogWarning($"[{name}] ALERT: Suspicion threshold passed for {observed.name} doing {action}!");
            Vector3 playerPos = observed.transform.position;

            //Alert Guards
            var alert = new SuspicionAlertEvent(
                source: gameObject,
                susNPC: gameObject,
                actionType: action,
                location: playerPos
            );
            GameEventBus<SuspicionAlertEvent>.Raise(alert);
            //Reset after alert
            suspicionScores[key] = 0f;
        }
    }

    private void TryGossip(GameObject observed, GossipKey key)
    {
        if (_gossiped.Contains(key)) { return; }

        float gossipScore = 0.5f * context.GossipChanceMultiplier;

        //Relation modifier 
        int relation = GetComponent<RelationshipModule>()?.GetRelation(observed) ?? 0;
        relation += context.RelationDeltaModifier;

        float finalScore = gossipScore + (Mathf.Clamp01((50f - relation) / 50f));

        Debug.Log($"[{name}] Gossip score = {finalScore:F2}, Threshold = {gossipThreshold}");

        if (finalScore >= gossipThreshold)
        {
            _gossiped.Add(key);
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, gossipRadius);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out MemoryModule receiver))
                {
                    receiver.ReceiveGossip(key, observed);
                }
            }
        }
    }

    public void ReceiveGossip(GossipKey key, GameObject observed)
    {
        if (_memories.Exists(m => m.ObservedInstanceId == key.ObservedId && m.ActionType == key.Action))
        {
            return;
        }
        _memories.Add(new MemoryEntry(observed, key.Action, 0.5f, true));
        Debug.Log($"[{name}] Received gossip about {observed.name} doing {key.Action}");
    }

    private void PassiveDecay()
    {
        float decay = confidenceDecayPerMemory * context.ConfidenceDecayMultiplier;

        for (int i = 0; i < _memories.Count; i++)
        {
            var m = _memories[i];
            m.Confidence = Mathf.Clamp01(m.Confidence - decay);
            _memories[i] = m;
            OnMemoryAdded?.Invoke(m);
        }

        DecaySuspicion();
    }

    private void DecaySuspicion()
    {
        var keys = suspicionScores.Keys.ToList();
        foreach (var key in keys)
        {
            suspicionScores[key] -= suspicionDecayRate;
            if (suspicionScores[key] <= 0)
            {
                suspicionScores.Remove(key);
            }
        }
    }

    public void EraseIfUncertain(float threshold = 0.3f)
    {
        _memories.RemoveAll(m => m.Confidence < threshold);
    }

    public void WeakenMemory(GameObject observed, string action, float amount)
    {
        int id = GetInstanceID();
        for (int i = 0; i < _memories.Count; i++)
        {
            var m = _memories[id]; 
            if (m.ObservedInstanceId == id && m.ActionType == action)
            {
                m.Confidence = Mathf.Clamp01(m.Confidence - amount);
                _memories[id] = m;
                Debug.Log($"[{name}] Weakened memory of {observed.name}: {m.Confidence:F2}");
                break;
            }
        }
    }
    public bool Believes(GameObject observed, string action, float minConfidence = 0.5f)
    {
        int id = observed.GetInstanceID();
        return _memories.Exists(m => m.ObservedInstanceId == id && m.ActionType == action && m.Confidence >= minConfidence);
    }

    public void SpreadMemoryToNearbyNPCs(GameObject observed, string action)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, gossipRadius); 
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            var memoryModule = hit.GetComponent<MemoryModule>();
            if (memoryModule != null)
            {
                memoryModule.Remember(observed, action);
                Debug.Log($"[{name}] gossiped to [{hit.name}] about {observed.name} doing {action}");
            }
        }
    }

    public MemoryEntry? GetLatestMemory()
    {
        if (_memories.Count == 0) { return null; }
        return _memories[^1];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, gossipRadius);
    }
}
