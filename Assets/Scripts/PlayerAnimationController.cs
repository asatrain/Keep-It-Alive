using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Walk,
        Run,
        Chop
    }

    [SerializeField] private Animator animator;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int Chop = Animator.StringToHash("chop");
    private State currentState;

    public void SetState(State state)
    {
        if (currentState == state)
            return;

        switch (state)
        {
            case State.Idle:
                animator.SetBool(IsWalking, false);
                animator.SetBool(IsRunning, false);
                break;
            case State.Walk:
                animator.SetBool(IsWalking, true);
                animator.SetBool(IsRunning, false);
                break;
            case State.Run:
                animator.SetBool(IsWalking, false);
                animator.SetBool(IsRunning, true);
                break;
            case State.Chop:
                animator.SetTrigger(Chop);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        currentState = state;
    }

    public bool IsChopping()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Chopping");
    }
}