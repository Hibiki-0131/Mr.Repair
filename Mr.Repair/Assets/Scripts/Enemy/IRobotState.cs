using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ロボットの状態を表すインターフェース
public interface IRobotState
{
    void Enter(RobotAI robot);
    void Update(RobotAI robot);
    void Exit(RobotAI robot);
    string GetName(); // 状態名を返す
}
