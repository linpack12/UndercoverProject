using UnityEngine;

public class SuspicionAlertEvent : GameEvent
{
    public GameObject SusNPC;
    public string ActionType;
    public Vector3 Location;

    public SuspicionAlertEvent(GameObject source, GameObject susNPC, string actionType, Vector3 location) : base(source) 
    {
        SusNPC = susNPC;
        ActionType = actionType;
        Location = location;
    }
}
