using System.Collections.Generic;
using UnityEngine;

public class TraitModule : MonoBehaviour
{
    [SerializeField] private List<TraitDefinition> traits = new();

    private NPCTraitContext _context = new();
    public NPCTraitContext Context => _context;

    private void Awake()
    {
        foreach (var trait in traits)
        {
            trait.ApplyTo(_context);
        }
        Debug.Log($"[{name}] Applied Traits: {string.Join(", ", traits.ConvertAll(t => t.name))}");
    }

    public void AddTrait(TraitDefinition trait)
    {
        traits.Add(trait);
        trait.ApplyTo(_context);
    }
}
