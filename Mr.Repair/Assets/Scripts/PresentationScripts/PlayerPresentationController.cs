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
    [SerializeField] private float goalMoveSpeed = 3f;

    // ★ 外部から参照するロック状態
    public bool IsLocked { get; private set; }

    private System.Action onGoalAnimationCompleted;
    private SoundTrigger soundTrigger;

    // =========================================================
    // 初期化
    // =========================================================

    private void Reset()
    {
        controller = GetComponent<PlayerController>();
        movement = GetComponent<PlayerMovement>();
        gameplayAnimation = GetComponent<PlayerAnimation>();
    }

    private void Start()
    {
        soundTrigger = FindObjectOfType<SoundTrigger>();
    }

    // =========================================================
    // Public API
    // =========================================================

    /// <summary>
    /// 旧API互換
    /// </summary>
    public void PlayGoal(System.Action onCompleted = null)
    {
        PlayGoal(transform.position, onCompleted);
    }

    /// <summary>
    /// ゴール演出開始
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
    // Goal Routine
    // =========================================================

    private IEnumerator GoalRoutine(Vector3 goalPos)
    {
        Debug.Log("<color=cyan>[Presentation] GoalRoutine START</color>");

        BeginLock();

        //-----------------------------------
        // プレイヤーをゴール位置へ配置
        //-----------------------------------
        transform.position = goalPos;

        yield return null; // 物理安定用1フレーム待機

        //-----------------------------------
        // カメラ切替
        //-----------------------------------
        if (CameraManager.Instance != null)
            CameraManager.Instance.SwitchToCamera(goalCameraID);

        //-----------------------------------
        // アニメーション再生
        //-----------------------------------
        gameplayAnimation.PlayGoal();
    }

    // =========================================================
    // Animation Events
    // =========================================================

    /// <summary>
    /// GoalSE再生（Animation Eventから呼ぶ）
    /// </summary>
    public void PlayGoalSE()
    {
        if (soundTrigger == null)
            soundTrigger = FindObjectOfType<SoundTrigger>();

        soundTrigger?.PlayByKey("GoalSE", transform.position);
    }

    /// <summary>
    /// ゴールアニメ終了イベント
    /// </summary>
    public void OnGoalAnimationEnd()
    {
        Debug.Log("<color=yellow>[Presentation] Goal Animation End Event</color>");

        onGoalAnimationCompleted?.Invoke();
        onGoalAnimationCompleted = null;

        // ※ ゴール後はステージ遷移するためロック解除しない
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

        // Animationは停止させない
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

    /// <summary>
    /// スタート演出用ロック
    /// </summary>
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
