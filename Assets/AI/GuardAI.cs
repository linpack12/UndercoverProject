using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AIPath), typeof(Seeker), typeof(AIDestinationSetter))]
public class GuardAI : MonoBehaviour
{
    private enum State { Patrolling, Investigating, Returning }

    private State _currentState = State.Patrolling;
    private AIPath _aiPath;
    private AIDestinationSetter _destinationSetter;

    private Vector3[] _patrolRoute;
    private int _currentPatrolIndex = 0;
    private Vector3 _investigationTarget;
    private float _searchTimer;
    private const float MaxSearchTime = 5f;

    private void Awake()
    {
        _aiPath = GetComponent<AIPath>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
    }

    private void Start()
    {
        var zonable = GetComponent<ZonableNPC>();
        if (zonable.CurrentZone != null)
        {
            InitializePatrolRoute(zonable.CurrentZone);
            GoToNextPatrolPoint();
        }
    }

    private void InitializePatrolRoute(Zone zoneBase)
    {
        var zone = zoneBase as GuardZone;
        if (zone == null ||zone.patrolPoints == null || zone.patrolPoints.Length == 0)
        {
            Debug.LogError("Guardzone missing patrol points");
            enabled = false;
            return;
        }
        _patrolRoute = new Vector3[zone.patrolPoints.Length];
        for (int i = 0; i < _patrolRoute.Length; i++)
        {
            _patrolRoute[i] = zone.patrolPoints[i].position;
        }
    }

    private void Update()
    {
        switch (_currentState)
        {
            case State.Patrolling:
                if (_aiPath.reachedDestination)
                {
                    GoToNextPatrolPoint();
                }
                break;
            case State.Investigating:
                if (Vector3.Distance(transform.position, _investigationTarget) < 1.5f)
                {
                    _searchTimer += Time.deltaTime;
                    if (_searchTimer > MaxSearchTime)
                    {
                        ReturnToPatrol();
                    }
                }
                break;
            case State.Returning:
                if (_aiPath.reachedDestination)
                {
                    _currentState = State.Patrolling;
                }
                break;
        }
    }

    public void Investigate(Vector3 location)
    {
        if (_currentState == State.Investigating)
        {
            return;
        }

        _investigationTarget = location + (Vector3)(Random.insideUnitCircle.normalized);
        _destinationSetter.target = CreateTempTarget(_investigationTarget);
        _searchTimer = 0f;
        _currentState = State.Investigating;
    }

    private void GoToNextPatrolPoint()
    {
        if (_patrolRoute.Length == 0)
        {
            return;
        }
        Debug.Log("Going to patrol point at: " + _patrolRoute[_currentPatrolIndex]);
        _destinationSetter.target = CreateTempTarget(_patrolRoute[_currentPatrolIndex]);
        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolRoute.Length;
    }

    private void ReturnToPatrol()
    {
        _destinationSetter.target = CreateTempTarget(_patrolRoute[_currentPatrolIndex]);
        _currentState = State.Returning;
    }

    private Transform CreateTempTarget(Vector3 pos)
    {
        GameObject go = new GameObject("TempTarget");
        go.transform.position = pos;
        Destroy(go, MaxSearchTime + 1f);
        return go.transform;
    }
}
