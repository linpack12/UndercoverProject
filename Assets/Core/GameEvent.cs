using UnityEngine;

public abstract class GameEvent
{
    public GameObject Source;
    public float Timestamp; 

    protected GameEvent(GameObject source)
    {
        Source = source;
        Timestamp = Time.time; 
    }
}
