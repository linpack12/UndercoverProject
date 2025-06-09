using System.Collections;
using UnityEngine;

public class SuspiciousActionComponent : MonoBehaviour, IPerceivable
{
    public bool IsSuspicious => active;
    public string ActionType => currentAction?.actionName;
    public GameObject Owner => gameObject;


    private bool active; 
    private ActionData currentAction;

    public void MarkSuspicious(ActionData action)
    {
        currentAction = action;
        active = true;
        StartCoroutine(ClearAfterSeconds(1f));
    }

    private IEnumerator ClearAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        active = false;
    }
}
