using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IRobotState
{
    public void Enter(RobotAI robot)
    {
        robot.agent.speed = robot.patrolSpeed;
        MoveToNextPoint(robot);
    }

    public void Update(RobotAI robot)
    {
        // プレイヤーが追跡範囲に入ったらChaseに移行
        if (robot.IsPlayerInChaseRange())
        {
            robot.SetState(new ChaseState());
            return;
        }

        // patrolPoints への移動
        if (!robot.agent.pathPending && robot.agent.remainingDistance < 0.2f)
        {
            MoveToNextPoint(robot);
        }
    }

    private void MoveToNextPoint(RobotAI robot)
    {
        if (robot.patrolPoints.Length == 0)
            return;

        robot.agent.destination = robot.patrolPoints[robot.currentPatrolIndex].position;
        robot.currentPatrolIndex = (robot.currentPatrolIndex + 1) % robot.patrolPoints.Length;
    }

    public void Exit(RobotAI robot) { }

    public string GetName() => "Patrol";
}
