using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    private int hashIsWalking;
    private int hashIsParts;
    private int hashIsChanging;
    private int hashChangeToParts;
    private int hashChangeToNormal;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        // パラメータをキャッシュ（高速化）
        hashIsWalking = Animator.StringToHash("isWalking");
        hashIsParts = Animator.StringToHash("isParts");
        hashIsChanging = Animator.StringToHash("isChanging");
        hashChangeToParts = Animator.StringToHash("ChangeToParts");
        hashChangeToNormal = Animator.StringToHash("ChangeToNormal");
    }

    public void SetIsWalking(bool isWalking)
    {
        anim.SetBool(hashIsWalking, isWalking);
    }

    public void SetPartsState(bool isParts)
    {
        anim.SetBool(hashIsParts, isParts);
    }

    /// <summary>
    /// 変形アニメーションの開始
    /// </summary>
    public void PlayTransformAnimation(bool toParts)
    {
        // すでに変形中なら再発火しない
        if (anim.GetBool(hashIsChanging))
            return;

        anim.SetBool(hashIsChanging, true);

        // トリガー発火前に両方リセット（前回の競合防止）
        anim.ResetTrigger(hashChangeToParts);
        anim.ResetTrigger(hashChangeToNormal);

        if (toParts)
        {
            anim.SetTrigger(hashChangeToParts);
            anim.SetBool(hashIsParts, true);
            Debug.Log(" 通常 → 部品化 変形開始");
        }
        else
        {
            anim.SetTrigger(hashChangeToNormal);
            anim.SetBool(hashIsParts, false);
            Debug.Log(" 部品 → 通常化 変形開始");
        }
    }

    /// <summary>
    /// アニメーションイベントで呼ばれる（終端）
    /// </summary>
    public void OnTransformAnimationEnd()
    {
        anim.SetBool(hashIsChanging, false);
        Debug.Log("変形アニメ完了 → isChanging=false");
    }
}
