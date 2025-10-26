using UnityEngine;

public class AttackState : IRobotState
{
    private float attackCooldown = 1f; // 攻撃周期を3秒に変更
    private float timer;

    public void Enter(RobotAI robot)
    {
        robot.agent.isStopped = true; // 攻撃中は停止
        timer = 0f;
    }

    public void Update(RobotAI robot)
    {
        // プレイヤーが攻撃範囲外に出たらChaseに戻る
        if (Vector3.Distance(robot.transform.position, robot.player.position) > robot.attackRange)
        {
            robot.agent.isStopped = false;
            robot.SetState(new ChaseState());
            return;
        }

        // 攻撃タイマーを加算
        timer += Time.deltaTime;

        // 攻撃可能か判定
        if (timer >= attackCooldown)
        {
            Attack(robot);
            timer = 0f; // タイマーリセット
        }
    }

    private void Attack(RobotAI robot)
    {
        // 攻撃処理をここに書く
        Debug.Log("Robot attacks Player!");

        // 例: プレイヤーにダメージを与える
        PlayerHealth playerHealth = robot.player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10); // 任意のダメージ量
        }

        // 攻撃中は停止状態を維持（agent.isStopped = true は Enter で設定済み）
    }

    public void Exit(RobotAI robot)
    {
        robot.agent.isStopped = false; // ステート退出時に再び移動可能に
    }

    public string GetName() => "Attack";
}
