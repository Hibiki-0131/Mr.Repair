using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAnimation))]
public class PlayerPresentationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerAnimation gameplayAnimation;

    [Header("Camera")]
    [SerializeField] private string goalCameraID = "GoalCamera";

    [Header("Goal Move")]
    [SerializeField] private float goalMoveSpeed = 3f;   // ★追加

    public bool IsLocked { get; private set; }

    private System.Action onGoalAnimationCompleted;

    private void Reset()
    {
        controller = GetComponent<PlayerController>();
        movement = GetComponent<PlayerMovement>();
        gameplayAnimation = GetComponent<PlayerAnimation>();
    }

    // =========================================================
    // Public API
    // =========================================================

    /// <summary>
    /// 旧API（互換維持）
    /// </summary>
    public void PlayGoal(System.Action onCompleted = null)
    {
        PlayGoal(transform.position, onCompleted);
    }

    /// <summary>
    /// ★新API：ゴール座標指定版
    /// </summary>
    public void PlayGoal(Vector3 goalPosition, System.Action onCompleted = null)
    {
        Debug.Log("<color=cyan>[Presentation] >>> PlayGoal(with position)</color>");

        if (IsLocked)
        {
            Debug.Log("<color=cyan>[Presentation] Already Locked → Ignore</color>");
            return;
        }

        onGoalAnimationCompleted = onCompleted;

        StartCoroutine(GoalRoutine(goalPosition));
    }

    // =========================================================
    // Routines
    // =========================================================

    private IEnumerator GoalRoutine(Vector3 goalPos)
    {
        Debug.Log("<color=cyan>[Presentation] GoalRoutine START</color>");

        BeginLock();

        //-----------------------------------
        // ★ ここが重要
        // Rigidbody/Controllerを無効にしてから座標セット
        //-----------------------------------
        transform.position = goalPos;

        yield return null; // 1frame待つ（物理安定化）

        //-----------------------------------
        // カメラ切替
        //-----------------------------------
        if (CameraManager.Instance != null)
            CameraManager.Instance.SwitchToCamera(goalCameraID);

        //-----------------------------------
        // アニメ再生
        //-----------------------------------
        gameplayAnimation.PlayGoal();
    }


    // =========================================================
    // Animation Event
    // =========================================================

    /// <summary>
    /// Goalアニメ終了イベントから呼ぶ
    /// </summary>
    public void OnGoalAnimationEnd()
    {
        Debug.Log("<color=yellow>[Presentation] Goal Animation End Event</color>");

        //EndLock();

        onGoalAnimationCompleted?.Invoke();
        onGoalAnimationCompleted = null;
    }

    // =========================================================
    // Lock Control
    // =========================================================

    private void BeginLock()
    {
        Debug.Log("<color=cyan>[Presentation] BeginLock()</color>");

        IsLocked = true;

        if (controller) controller.enabled = false;
        if (movement) movement.enabled = false;
        if (gameplayAnimation) gameplayAnimation.enabled = true;
    }

    private void EndLock()
    {
        Debug.Log("<color=cyan>[Presentation] EndLock()</color>");

        if (controller) controller.enabled = true;
        if (movement) movement.enabled = true;
        if (gameplayAnimation) gameplayAnimation.enabled = true;

        IsLocked = false;
    }

    public void LockControl(float duration)
    {
        StartCoroutine(LockRoutine(duration));
    }

    private IEnumerator LockRoutine(float duration)
    {
        BeginLock();
        yield return new WaitForSeconds(duration);
        EndLock();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            Debug.Log($"<color=cyan>[Presentation] Debug → Locked:{IsLocked}</color>");
        }
    }
#endif
}
