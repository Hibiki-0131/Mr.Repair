using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(BlockSettlementSensor))]
public class PushableBlock : MonoBehaviour
{
    [SerializeField] private float gravityMultiplier = 5f;

    private Rigidbody rb;

    /// <summary>
    /// すでに地形として確定したか
    /// </summary>
    public bool IsSettled { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;

        IsSettled = false;
    }

    private void FixedUpdate()
    {
        if (rb.isKinematic)
            return;

        rb.AddForce(
            Physics.gravity * gravityMultiplier,
            ForceMode.Acceleration
        );
    }

    //  OnCollisionEnter 完全削除
    // 衝突処理は BlockSettlementSensor に一元化済み

    // ================================
    // Settlement API
    // ================================

    public void Freeze()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    public void Unfreeze()
    {
        rb.isKinematic = false;
        IsSettled = false;
    }

    public void MarkSettled()
    {
        IsSettled = true;
    }

    public void SetOwner(RoomBuilder builder)
    {
        // 互換用ダミー
    }
}
