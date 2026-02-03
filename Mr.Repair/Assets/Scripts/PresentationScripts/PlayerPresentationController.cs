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

    public bool IsLocked { get; private set; }

    // =========================================================
    // Setup
    // =========================================================

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
    /// 一定時間プレイヤー操作をロックする（Start演出用）
    /// Animatorは触らない（DefaultState再生前提）
    /// </summary>
    public void LockControl(float duration)
    {
        if (IsLocked) return;

        Debug.Log($"[Presentation] LockControl {duration}s");

        StartCoroutine(LockRoutine(duration));
    }

    /// <summary>
    /// ゴール演出（カメラ＋ロック）
    /// GoalアニメはAnimator側Triggerで制御
    /// </summary>
    public void PlayGoal(float duration)
    {
        if (IsLocked) return;

        Debug.Log("[Presentation] PlayGoal");

        StartCoroutine(GoalRoutine(duration));
    }

    // =========================================================
    // Routines
    // =========================================================

    private IEnumerator LockRoutine(float duration)
    {
        BeginLock();

        gameplayAnimation.PlayStart(); // ★追加（Controller遷移）

        yield return new WaitForSeconds(duration);

        EndLock();
    }

    private IEnumerator GoalRoutine(float duration)
    {
        BeginLock();

        gameplayAnimation.PlayGoal(); // ★追加

        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.SwitchToCamera(goalCameraID);
        }

        yield return new WaitForSeconds(duration);

        EndLock();
    }

    // =========================================================
    // Lock Control (最重要責務)
    // =========================================================

    private void BeginLock()
    {
        Debug.Log("[Presentation] Disable player control");

        IsLocked = true;

        if (controller) controller.enabled = false;     // 入力停止（最重要）
        if (movement) movement.enabled = false;         // 移動停止
        if (gameplayAnimation) gameplayAnimation.enabled = false; // walk更新停止
    }

    private void EndLock()
    {
        Debug.Log("[Presentation] Enable player control");

        if (controller) controller.enabled = true;
        if (movement) movement.enabled = true;
        if (gameplayAnimation) gameplayAnimation.enabled = true;

        IsLocked = false;
    }
}
