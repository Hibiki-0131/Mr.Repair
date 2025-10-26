using UnityEngine;

public class ChaseState : IRobotState
{
    public void Enter(RobotAI robot)
    {
        robot.agent.speed = robot.chaseSpeed;
    }

    public void Update(RobotAI robot)
    {
        robot.agent.SetDestination(robot.player.position);

        if (robot.IsPlayerInAttackRange())
        {
            robot.SetState(new AttackState());
            return;
        }

        if (!robot.IsPlayerInChaseRange())
        {
            robot.SetState(new PatrolState());
        }
    }

    public void Exit(RobotAI robot) { }

    public string GetName() => "Chase";
}
