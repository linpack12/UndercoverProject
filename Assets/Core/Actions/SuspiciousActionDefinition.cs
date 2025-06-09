
using UnityEngine;

[CreateAssetMenu(fileName = "new hata carbonara", menuName = "Action Type")]
public class ActionData : ScriptableObject
{
    public string actionName;
    [Range(0f, 1f)] public float suspiciousLevel = 0.1f; 
}
