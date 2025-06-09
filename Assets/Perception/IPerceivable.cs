using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPerceivable
{
    bool IsSuspicious { get; }
    string ActionType { get; }
    GameObject Owner { get; }
}
