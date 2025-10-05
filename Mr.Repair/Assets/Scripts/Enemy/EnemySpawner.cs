using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private float spawnDelay = 2f;
    [SerializeField] private float spawnInterval = 10f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnRobot), spawnDelay, spawnInterval);
    }

    void SpawnRobot()
    {
        // プールから取得
        GameObject robot = enemyPool.GetFromPool();

        // ランダムスポーン位置に配置
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        robot.transform.position = point.position;
        robot.transform.rotation = point.rotation;

        // 必要であれば初期化処理
        RobotAI ai = robot.GetComponent<RobotAI>();
        ai.SetState(new PatrolState()); // 初期状態リセット
    }
}
