using UnityEngine;

[CreateAssetMenu(menuName = "Traits/ForgetfulTrait")]
public class ForgetfulTraitData : TraitDefinition
{
    [Range(0f, 2f)] public float decayMultiplier = 1.5f;

    public override void ApplyTo(NPCTraitContext context)
    {
        context.ConfidenceDecayMultiplier *= decayMultiplier;
    }
}
