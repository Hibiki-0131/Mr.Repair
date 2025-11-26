using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PushableBlock : MonoBehaviour
{
    [Header("重力設定")]
    [SerializeField] private float gravityMultiplier = 5f; // 標準重力の5倍で素早く落下

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // カスタム重力を適用するので無効化
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        // 重力を5倍にして下方向に常に加える
        rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
    }
}
