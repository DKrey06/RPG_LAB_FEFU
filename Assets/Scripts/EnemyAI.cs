using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float movespeed = 2f;
    private int _currentPatrolIndex = 0;

    public float detectionRadius = 5f;
    private bool _isPlayerDetecetd = false;
    private Transform _player;

    private Rigidbody2D _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (_player == null) Debug.LogWarning("игрока нет");
    }


    void Update()
    {
        CheckForPlayer();

        if (_isPlayerDetecetd && _player != null)
        {
            MoveTowards(_player.position);
        }
        else
        {
            if (patrolPoints.Length > 0)
            {
                Vector3 targetPos = patrolPoints[_currentPatrolIndex].position;
                MoveTowards(targetPos);

                if (Vector2.Distance(transform.position, targetPos) < 0.1f)
                {
                    _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
                }
            }
        }

        void CheckForPlayer()
        {
            if (_player == null) return;
            float distanceToPlayer = Vector2.Distance(transform.position, _player.position);
            _isPlayerDetecetd = distanceToPlayer <= detectionRadius;
        }

        void MoveTowards(Vector3 targetPosition)
        {
            Vector2 direction = (targetPosition - transform.position).normalized;
            _rb.linearVelocity = direction * movespeed;
        }
    }
}
