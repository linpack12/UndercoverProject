using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(AIPath), typeof(Seeker), typeof(AIDestinationSetter))]
public class GuardAI : MonoBehaviour
{
    private enum State { Patrolling, Investigating, Chasing, Returning }

    //Player detection
    [SerializeField] private float visionAngle = 45f;
    [SerializeField] private float visionDistance = 5f;
    [SerializeField] private float arrestDistance = 1f;

    private Transform targetPlayer;
    private float timeSinceSeen;

    private State _currentState = State.Patrolling;
    private AIPath _aiPath;
    private AIDestinationSetter _destinationSetter;

    private Vector3[] _patrolRoute;
    private int _currentPatrolIndex = 0;
    private Vector3 _investigationTarget;
    private float _searchTimer;
    private const float MaxSearchTime = 5f;

    public bool IsBusy => _currentState == State.Chasing || _currentState == State.Investigating;

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
                ScanForPlayer();
                if (Vector3.Distance(transform.position, _investigationTarget) < 1.5f)
                {
                    _searchTimer += Time.deltaTime;
                    if (_searchTimer > MaxSearchTime)
                    {
                        ReturnToPatrol();
                    }
                }
                break;

            case State.Chasing:
                if (targetPlayer == null)
                {
                    ReturnToPatrol();
                    break;
                }
                _destinationSetter.target = targetPlayer;
                if (Vector3.Distance(transform.position, targetPlayer.position) < arrestDistance)
                {
                    ArrestPlayer();
                }
                else
                {
                    ScanForPlayer();
                    if (timeSinceSeen > MaxSearchTime)
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

    private void ScanForPlayer()
    {
        var player = PlayerIdentity.LocalPlayer?.transform;
        if (player == null) { return; }

        var dir = player.position - transform.position;
        if (dir.magnitude > visionDistance)
        {
            timeSinceSeen += Time.deltaTime;
            return; 
        }

        if(Vector3.Angle(transform.right, dir) > visionAngle)
        {
            timeSinceSeen += Time.deltaTime;
            return;
        }
        //No obstacle/wall away
        if (Physics2D.Linecast(transform.position, player.position, LayerMask.GetMask("wall")))
        {
            timeSinceSeen += Time.deltaTime;
            return;
        }

        //Seen player
        targetPlayer = player;
        _currentState = State.Chasing;
        timeSinceSeen = 0f;
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

    private void ArrestPlayer()
    {
        if (targetPlayer.TryGetComponent<PlayerTestController>(out var controller))
        {
            controller.enabled = false;
        }

        if (targetPlayer.TryGetComponent<SpriteRenderer>(out var renderer))
        {
            renderer.color = Color.red;
        }

        Debug.Log("Player arrested!");
        ReturnToPatrol();
    }

    private Transform CreateTempTarget(Vector3 pos)
    {
        GameObject go = new GameObject("TempTarget");
        go.transform.position = pos;
        Destroy(go, MaxSearchTime + 1f);
        return go.transform;
    }
}
