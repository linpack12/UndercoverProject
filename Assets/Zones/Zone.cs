using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(Collider2D))]
public class Zone : MonoBehaviour
{
    public int zoneId;
    public List<string> allowedRoles = new();


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<ZonablePlayer>(out var player))
        {
            player.OnZoneEntered(this); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<ZonablePlayer>(out var player))
        {
            player.OnZoneExited(this);
        }
    }
    
    public Vector3 GetRandomPositionInZone(float padding = 0.5f)
    {
        var bounds = GetComponent<Collider2D>().bounds;
        float x = Random.Range(bounds.min.x + padding, bounds.max.x - padding);
        float y = Random.Range(bounds.min.y + padding, bounds.max.y - padding);
        return new Vector3(x, y, 0);
    }
}
