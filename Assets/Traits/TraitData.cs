using UnityEngine;

public abstract class TraitData : ScriptableObject
{
    public string traitName;

    public virtual void ModifyMemory(ref float confidenceDecay) { }
    public virtual void ModifyRelation(GameObject source, ref int delta) { }
    public virtual void ModifyGossip(ref float gossipChance) { }
}
