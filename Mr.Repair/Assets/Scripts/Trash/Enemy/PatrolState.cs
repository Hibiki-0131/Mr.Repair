using UnityEngine;

public class PatrolState : IRobotState
{
    public void Enter(RobotAI robot)
    {
        robot.agent.speed = robot.patrolSpeed;
        MoveToNextPoint(robot);
    }

    public void Update(RobotAI robot)
    {
        if (robot.IsPlayerInChaseRange())
        {
            robot.SetState(new ChaseState());
            return;
        }

        if (!robot.agent.pathPending && robot.agent.remainingDistance < 0.2f)
        {
            MoveToNextPoint(robot);
        }
    }

    private void MoveToNextPoint(RobotAI robot)
    {
        if (robot.currentRoute == null || robot.currentRoute.Length == 0)
            return;

        robot.agent.destination = robot.currentRoute[robot.currentPatrolIndex].position;
        robot.currentPatrolIndex = (robot.currentPatrolIndex + 1) % robot.currentRoute.Length;
    }

    public void Exit(RobotAI robot) { }

    public string GetName() => "Patrol";
}
