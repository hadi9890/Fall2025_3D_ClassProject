using System.Linq;
using _SOs;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class SmartEnemy : MonoBehaviour
{
    private enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    private EnemyState _currState = EnemyState.Patrol;
    public EnemyData enemyData;
    [CanBeNull] public Transform projectileSpawn;

    private NavMeshAgent _agent;
    private Animator _anim;
    private Transform _player;

    public Transform patrolPointsParent;
    private Transform[] _patrolPoints;
    private int _currentPatrolIndex;

    private bool _alreadyAttacked;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (enemyData == null)
        {
            Debug.LogError($"EnemyData not assigned to {name}");
            enabled = false;
            return;
        }

        _agent.speed = enemyData.walkSpeed;

        if (patrolPointsParent != null)
        {
            _patrolPoints = patrolPointsParent.GetComponentsInChildren<Transform>().Where(t => t != patrolPointsParent)
                .ToArray();

            if (_patrolPoints.Length > 0)
            {
                SetClosestPatrolPoint();
            }
        }
    }

    private void Update()
    {
        PlayerInSight();
        PlayerInAttackRange();

        switch (_currState)
        {
            case EnemyState.Patrol:
                // Patrol();
                if (PlayerInSight())
                {
                    _currState = EnemyState.Chase;
                }
                break;
            case EnemyState.Chase:
                // Chase();
                if (PlayerInAttackRange())
                {
                    _currState = EnemyState.Attack;
                }
                else if (!PlayerInSight())
                {
                    _currState = EnemyState.Patrol;
                }
                break;
            case EnemyState.Attack:
                // Attack();
                if (!PlayerInAttackRange() && PlayerInSight())
                {
                    _currState = EnemyState.Chase;
                }
                else if (!PlayerInSight())
                {
                    _currState = EnemyState.Patrol;
                }
                break;
        }
    }

    #region Enemy Checks

    private bool PlayerInSight()
    {
        var directionToPlayer = (_player.position - transform.position).normalized;
        var distance = Vector3.Distance(transform.position, _player.position);
        var angle = Vector3.Angle(transform.forward, directionToPlayer);

        return distance <= enemyData.sightRange && angle <= enemyData.sightAngle / 2;
    }

    private bool PlayerInAttackRange()
    {
        var directionToPlayer = (_player.position - transform.position).normalized;
        var distance = Vector3.Distance(transform.position, _player.position);
        var angle = Vector3.Angle(transform.forward, directionToPlayer);

        return distance <= enemyData.attackRange && angle <= enemyData.sightAngle / 2;
    }

    #endregion

    private void SetClosestPatrolPoint()
    {
        var closestDistance = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < _patrolPoints.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, _patrolPoints[i].position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        _currentPatrolIndex = closestIndex;
        _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
    }

    private void GoToNextPatrolPoint()
    {
        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
        _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1,0,0,0.5f);

        var forward = transform.forward;
        var halfAngle = enemyData.sightAngle / 2f;

        var rightBoundary = Quaternion.Euler(0, halfAngle, 0) * forward;
        var leftBoundary = Quaternion.Euler(0, -halfAngle, 0) * forward;
        
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * enemyData.sightRange);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * enemyData.sightRange);
    }
}