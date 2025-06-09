using UnityEngine;

public class ObservationEvent : GameEvent
{
    public GameObject Observer;
    public GameObject Observed;
    public string ActionType; 

    public ObservationEvent(GameObject observer, GameObject observed, string actionType) : base(observer)
    {
        Observer = observer;
        Observed = observed;
        ActionType = actionType;
    }
}
