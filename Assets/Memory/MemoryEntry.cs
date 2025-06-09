using UnityEngine;

public struct MemoryEntry
{
    public int ObservedInstanceId;
    public string ActionType;
    public float Confidence;
    public bool WasGossiped;

    public MemoryEntry(GameObject observed, string actionType, float confidence = 1f, bool wasGossiped = false)
    {
        ObservedInstanceId = observed.GetInstanceID();
        ActionType = actionType;    
        Confidence = confidence;
        WasGossiped = wasGossiped;
    }
}
