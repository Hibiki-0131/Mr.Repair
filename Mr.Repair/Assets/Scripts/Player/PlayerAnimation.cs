using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    // ========= Parameters =========
    private int hashIsWalking;
    private int hashIsParts;
    private int hashIsChanging;

    private int hashChangeToParts;
    private int hashChangeToNormal;

    private int hashGoal;

    // =========================================================
    // Setup
    // =========================================================

    private void Awake()
    {
        anim = GetComponent<Animator>();

        hashIsWalking = Animator.StringToHash("isWalking");
        hashIsParts = Animator.StringToHash("isParts");
        hashIsChanging = Animator.StringToHash("isChanging");

        hashChangeToParts = Animator.StringToHash("ChangeToParts");
        hashChangeToNormal = Animator.StringToHash("ChangeToNormal");

        hashGoal = Animator.StringToHash("Goal");

        Debug.Log("<color=yellow>[Animation] Ready</color>");
    }

    // =========================================================
    // Walking
    // =========================================================

    public void SetIsWalking(bool isWalking)
    {
        anim.SetBool(hashIsWalking, isWalking);
    }

    // =========================================================
    // Parts Transform  ← ★ここ復活（必須）
    // =========================================================

    public void PlayTransformAnimation(bool toParts)
    {
        Debug.Log($"<color=yellow>[Animation] Transform → {(toParts ? "Parts" : "Normal")}</color>");

        if (anim.GetBool(hashIsChanging))
            return;

        anim.SetBool(hashIsChanging, true);

        anim.ResetTrigger(hashChangeToParts);
        anim.ResetTrigger(hashChangeToNormal);

        if (toParts)
        {
            anim.SetTrigger(hashChangeToParts);
            anim.SetBool(hashIsParts, true);
        }
        else
        {
            anim.SetTrigger(hashChangeToNormal);
            anim.SetBool(hashIsParts, false);
        }
    }

    // アニメーションイベントで呼ぶ
    public void OnTransformAnimationEnd()
    {
        anim.SetBool(hashIsChanging, false);
    }

    // =========================================================
    // Goal
    // =========================================================

    public void PlayGoal()
    {
        Debug.Log("<color=yellow>[Animation] Goal Trigger SET</color>");

        anim.ResetTrigger(hashGoal);
        anim.SetTrigger(hashGoal);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            var info = anim.GetCurrentAnimatorClipInfo(0);
            if (info.Length > 0)
                Debug.Log($"<color=yellow>[Animation] Current : {info[0].clip.name}</color>");
        }
    }
#endif
}
