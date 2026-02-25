using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerEffectController : MonoBehaviour
{
    [Header("Effect References")]
    [SerializeField] private ParticleSystem walkDust;
    [SerializeField] private ParticleSystem goalEffect;

    private PlayerMovement movement;

    private bool isPlaying = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();

        if (walkDust != null)
            walkDust.Stop();
    }

    private void Update()
    {
        if (movement == null || walkDust == null)
            return;

        bool shouldPlay =
            movement.IsMoving &&
            !movement.IsPartsMode &&
            IsGrounded();

        if (shouldPlay && !isPlaying)
        {
            walkDust.Play();
            isPlaying = true;
        }
        else if (!shouldPlay && isPlaying)
        {
            walkDust.Stop();
            isPlaying = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.2f,
                               Vector3.down,
                               1.0f);
    }

    public void PlayGoalEffect()
    {
        Debug.Log("<color=green>GoalEffect Called</color>");

        if (goalEffect == null)
        {
            Debug.LogError("goalEffect is NULL");
            return;
        }

        goalEffect.Play();
    }
}