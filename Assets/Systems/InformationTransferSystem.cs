using System.Collections;
using UnityEngine;

public class InformationTransferSystem : MonoBehaviour
{
    private void OnEnable()
    {
        GameEventBus<ObservationEvent>.OnGameEvent += HandleGameEvent;
    }

    private void OnDisable()
    {
        GameEventBus<ObservationEvent>.OnGameEvent -= HandleGameEvent;
    }

    private void HandleGameEvent(ObservationEvent e)
    {
        if (e is ObservationEvent observation)
        {
            var npc = observation.Observer.GetComponent<RelationshipModule>();
            var memory = observation.Observer.GetComponent<MemoryModule>();

            if (npc == null || memory == null) return;

            memory.Remember(observation.Observed, observation.ActionType);

            int relation = npc.GetRelation(observation.Observed);
            if (relation < npc.firendlyRelationThreshold)
            {
                Debug.Log($"{observation.Observer.name} spreads a rumour about {observation.Observed.name} (relation: {relation}");
                memory?.Remember(observation.Observed, observation.ActionType);
            }
        }
    }
}
