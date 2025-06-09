using UnityEngine;

[CreateAssetMenu(menuName = "Traits/SusTrait")]
public class SusTraitData : TraitDefinition
{
    [SerializeField] private float confidenceBoost = 0.2f;

    public override void ApplyTo(NPCTraitContext context) 
    {
        context.ConfidenceDecayMultiplier += confidenceBoost;
    }
    
}
