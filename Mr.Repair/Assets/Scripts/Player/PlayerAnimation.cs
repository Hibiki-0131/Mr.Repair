// PlayerAnimation.cs (rotationŠÖ˜Aˆ—‚Ííœ)
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetIsWalking(bool isWalking)
    {
        anim.SetBool("isWalking", isWalking);
    }

    public void SetPartsState(bool isParts)
    {
        anim.SetBool("isParts", isParts);
    }

    public void PlayTransformAnimation(bool toParts)
    {
        if (toParts)
            anim.SetTrigger("ChangeToParts");
        else
            anim.SetTrigger("ChangeToNormal");
    }
}
