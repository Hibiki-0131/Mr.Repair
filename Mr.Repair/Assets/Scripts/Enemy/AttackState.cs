using UnityEngine;

public class AttackState : IRobotState
{
    private float attackCooldown = 1.5f;
    private float timer;

    public void Enter(RobotAI robot)
    {
        robot.agent.isStopped = true;
        timer = 0f;
    }

    public void Update(RobotAI robot)
    {
        timer += Time.deltaTime;

        if (Vector3.Distance(robot.transform.position, robot.player.position) > robot.attackRange)
        {
            robot.agent.isStopped = false;
            robot.SetState(new ChaseState());
            return;
        }

        if (timer >= attackCooldown)
        {
            Debug.Log("Robot attacks Player!");
            timer = 0f;
        }
    }

    public void Exit(RobotAI robot)
    {
        robot.agent.isStopped = false;
    }

    public string GetName() => "Attack";
}
