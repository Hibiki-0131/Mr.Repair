using UnityEngine;
using UnityEngine.AI;

public class RobotAI : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Ranges")]
    public float chaseRange = 10f;   // 追跡範囲
    public float attackRange = 2f;   // 攻撃範囲（追跡範囲より小さい）

    [Header("Speeds")]
    public float chaseSpeed = 3f;
    public float patrolSpeed = 1.5f;

    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    [HideInInspector] public int currentPatrolIndex = 0;

    private IRobotState currentState;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;

        SetState(new PatrolState()); // 初期状態はPatrol
    }

    private void Update()
    {
        currentState?.Update(this);

        // デバッグ用に現在の状態を表示
        if (currentState != null)
            Debug.Log("Current State: " + currentState.GetName());
    }

    public void SetState(IRobotState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    // Player が chaseRange 内にいるか
    public bool IsPlayerInChaseRange()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= chaseRange;
    }

    // Player が attackRange 内にいるか
    public bool IsPlayerInAttackRange()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= attackRange;
    }


    private void OnDrawGizmosSelected()
    {
        // 追跡範囲（青）
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // 攻撃範囲（赤）
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
