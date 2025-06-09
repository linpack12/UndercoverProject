using System.Collections.Generic;
using UnityEngine;

public class RelationshipModule : MonoBehaviour
{
    private Dictionary<GameObject, int> _relations = new();

    [SerializeField] private int defaultRelation = 50; 
    [SerializeField] public int firendlyRelationThreshold = 70; 
    [SerializeField] public int hostileRelationThreshold = 30; 

    public void AdjustRelation(GameObject target, int delta)
    {
        if (!_relations.ContainsKey(target))
        {
            _relations[target] = defaultRelation;   
        }
        // Clamp 01 
        _relations[target] = Mathf.Clamp(_relations[target] + delta, 0, 100); 
    }

    public int GetRelation(GameObject target)
    {
        return _relations.ContainsKey(target) ? _relations[target] : defaultRelation;
    }

    public bool IsFriendly(GameObject target) => GetRelation(target) >= 70;
    public bool IsHostile(GameObject target) => GetRelation(target) <= 30; 
}
