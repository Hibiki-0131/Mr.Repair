using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public void Jump()
    {
        Debug.Log("Jump!");
        // Rigidbodyを使うなら AddForce などでジャンプ実装
    }

    // 今後ここに攻撃やインタラクト処理を追加できる
}

