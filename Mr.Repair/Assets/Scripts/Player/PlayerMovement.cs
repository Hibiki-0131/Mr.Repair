using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float partsSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    [Header("Collider Size")]
    [SerializeField] private Vector3 normalColliderSize = new Vector3(0.51f, 0.82f, 0.52f);
    [SerializeField] private Vector3 partsColliderSize = new Vector3(0.51f, 0.43f,0.52f);
    [SerializeField] private Vector3 normalColliderCenter = new Vector3(0.004f, -0.11f, 0.2f);
    [SerializeField] private Vector3 partsColliderCenter = new Vector3(0.004f, -0.3045f, 0.2f);

    private Rigidbody rb;
    private BoxCollider boxCollider;
    private Vector2 moveInput;
    private bool isPartsMode = false;
    private bool isInNarrowSpace = false;

    public bool IsPartsMode => isPartsMode;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Collider初期値を明示的に設定
        boxCollider.size = normalColliderSize;
        boxCollider.center = normalColliderCenter;
    }


    private void LateUpdate()
    {
        // カメラ更新（CameraManager対応）
        if (CameraManager.Instance != null)
        {
            Camera activeCam = CameraManager.Instance.GetActiveCamera();
            if (activeCam != null && cameraTransform != activeCam.transform)
                cameraTransform = activeCam.transform;
        }

        // Collider強制維持（Animatorや他の要因で戻される対策）
        if (isPartsMode)
        {
            boxCollider.size = partsColliderSize;
            boxCollider.center = partsColliderCenter;
        }
        else
        {
            boxCollider.size = normalColliderSize;
            boxCollider.center = normalColliderCenter;
        }
    }


    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    // 部品化開始
    public void BeginPartsMode()
    {
        if (!isPartsMode)
        {
            isPartsMode = true;
            UpdateColliderSize(true);
        }
    }

    // 部品化終了（狭い場所ではキャンセルされる）
    public void TryEndPartsMode()
    {
        if (!isInNarrowSpace)
        {
            isPartsMode = false;
            UpdateColliderSize(false);
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (cameraTransform == null)
            return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * 100 * Time.fixedDeltaTime);
        }

        float speed = isPartsMode ? partsSpeed : normalSpeed;
        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
    }

    // Colliderサイズ変更
    private void UpdateColliderSize(bool toParts)
    {
        if (toParts)
        {
            boxCollider.size = partsColliderSize;
            boxCollider.center = partsColliderCenter;
        }
        else
        {
            boxCollider.size = normalColliderSize;
            boxCollider.center = normalColliderCenter;
        }
    }

    // 狭い空間の出入り検出（例：NarrowSpaceタグのCubeに触れている間）
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NarrowSpace"))
            isInNarrowSpace = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NarrowSpace"))
            isInNarrowSpace = false;
    }
}
