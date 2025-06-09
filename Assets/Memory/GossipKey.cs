using System;

[Serializable]
public struct GossipKey 
{
    public int ObservedId;
    public string Action;

    public GossipKey(int observedId, string action)
    {
        ObservedId = observedId;
        Action = action;
    }

    public override bool Equals(object obj)
    {
        return obj is GossipKey key && ObservedId == key.ObservedId && Action == key.Action;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ObservedId, Action);
    }
}
