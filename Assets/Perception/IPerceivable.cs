using UnityEngine;

public interface IPerceivable
{
    bool IsSuspicious { get; }
    string ActionType { get; }
    GameObject Owner { get; }
}
