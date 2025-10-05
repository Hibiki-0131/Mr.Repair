using UnityEngine;
using UnityEngine.AI;

public class RobotAI : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public Transform player;
    public RouteManager routeManager;

    [Header("Ranges")]
    public float chaseRange = 10f;
    public float attackRange = 2f;

    [Header("Speeds")]
    public float chaseSpeed = 3f;
    public float patrolSpeed = 1.5f;

    [HideInInspector] public Transform[] currentRoute;
    [HideInInspector] public int currentPatrolIndex = 0;

    private IRobotState currentState;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player")?.transform;

        if (routeManager == null)
        {
            Debug.LogError($"{name}: RouteManager が未設定です！RobotAI にドラッグしてください。");
            return;
        }

        // 全ルートを取得
        var allRoutes = routeManager.GetAllRoutes();
        if (allRoutes == null || allRoutes.Length == 0)
        {
            Debug.LogError($"{name}: RouteManager にルートが登録されていません！");
            return;
        }

        // ランダムにルート選択
        int routeIndex = Random.Range(0, allRoutes.Length);
        currentRoute = allRoutes[routeIndex];

        if (currentRoute == null || currentRoute.Length == 0)
        {
            Debug.LogError($"{name}: 選択したルートに巡回ポイントがありません！（RouteIndex = {routeIndex}）");
            return;
        }

        // --- デバッグ出力 ---
        string routeName = routeManager.routeParents != null && routeManager.routeParents.Length > routeIndex
            ? routeManager.routeParents[routeIndex].name
            : "(名前不明)";
        string points = string.Join(", ", System.Array.ConvertAll(currentRoute, p => p.name));

        Debug.Log($"{name}: RouteManager から「{routeName}」を選択しました。({currentRoute.Length}ポイント)\n→経路: [{points}]");

        // 初期状態を設定
        SetState(new PatrolState());
    }

    private void Update()
    {
        currentState?.Update(this);

        if (currentState != null)
            Debug.Log($"Current State ({name}): {currentState.GetName()}");
    }

    public void SetState(IRobotState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    public bool IsPlayerInChaseRange()
    {
        if (player == null) return false;
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= chaseRange;
    }

    public bool IsPlayerInAttackRange()
    {
        if (player == null) return false;
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= attackRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
