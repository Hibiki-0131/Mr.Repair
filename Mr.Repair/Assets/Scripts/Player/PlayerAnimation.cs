using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    // ========= Hash =========
    private int hashIsWalking;
    private int hashIsParts;
    private int hashIsChanging;
    private int hashChangeToParts;
    private int hashChangeToNormal;

    // ★追加
    private int hashStart;
    private int hashGoal;

    // =========================================

    private void Awake()
    {
        anim = GetComponent<Animator>();

        hashIsWalking = Animator.StringToHash("isWalking");
        hashIsParts = Animator.StringToHash("isParts");
        hashIsChanging = Animator.StringToHash("isChanging");
        hashChangeToParts = Animator.StringToHash("ChangeToParts");
        hashChangeToNormal = Animator.StringToHash("ChangeToNormal");

        // ★追加
        hashStart = Animator.StringToHash("Start");
        hashGoal = Animator.StringToHash("Goal");
    }

    // =========================================
    // Gameplay
    // =========================================

    public void SetIsWalking(bool isWalking)
    {
        anim.SetBool(hashIsWalking, isWalking);
    }

    // =========================================
    // ★ 追加：演出専用API
    // =========================================

    public void PlayStart()
    {
        anim.ResetTrigger(hashGoal);
        anim.SetTrigger(hashStart);

        Debug.Log("[Animation] Start Trigger");
    }

    public void PlayGoal()
    {
        anim.ResetTrigger(hashStart);
        anim.SetTrigger(hashGoal);

        Debug.Log("[Animation] Goal Trigger");
    }

    // =========================================
    // 既存（変形）
    // =========================================

    public void PlayTransformAnimation(bool toParts)
    {
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

    public void OnTransformAnimationEnd()
    {
        anim.SetBool(hashIsChanging, false);
    }
}
