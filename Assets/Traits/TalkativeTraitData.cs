using UnityEngine;

[CreateAssetMenu(fileName = "TalkativeTrait", menuName = "Traits/Talkative")]
public class TalkativeTraitData : TraitDefinition
{
    [SerializeField] private float gossipScore = 0.3f;

    public override void ApplyTo(NPCTraitContext context)
    {
        context.GossipChanceMultiplier += gossipScore;
    }
}
