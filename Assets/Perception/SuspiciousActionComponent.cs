
using System.Collections;
using UnityEngine;

public class SuspiciousActionComponent : MonoBehaviour, IPerceivable
{
    public bool IsSuspicious => _active;
    public string ActionType => _currentAction;
    public GameObject Owner => gameObject;

    private bool _active; 
    private string _currentAction;

    public void MarkSuspicious(string a)
    {
        _currentAction = a;
        _active = true;
        StartCoroutine(ClearAfterSeconds(1f));
    }

    private IEnumerator ClearAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _active = false;
    }
}
