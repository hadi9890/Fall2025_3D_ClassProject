using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Lecture08.Enemies
{
    public class FollowEnemy : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private Animator _anim;

        private Transform _target;
        private bool _hasStoppedMoving;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            ChasePlayer();
        }

        private void ChasePlayer()
        {
            if (_target)
            {
                _agent.SetDestination(_target.position);
            }
            
            _anim.SetFloat("Speed", 1, 0.2f, Time.deltaTime);

            float distanceToTarget = Vector3.Distance(transform.position, _target.position);

            if (distanceToTarget <= _agent.stoppingDistance)
            {
                _anim.SetFloat("Speed", 0);

                if (!_hasStoppedMoving)
                {
                    _hasStoppedMoving = true;
                    // TODO: Add Combat Logic
                }
                transform.LookAt(_target);
            }
            else
            {
                if (_hasStoppedMoving)
                {
                    _hasStoppedMoving = false;
                }
            }
        }
    }
}
