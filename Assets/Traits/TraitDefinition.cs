using UnityEngine;

public abstract class TraitDefinition : ScriptableObject
{
    public abstract void ApplyTo(NPCTraitContext context);
}
