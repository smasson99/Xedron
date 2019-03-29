using System;
using UnityEngine;

namespace Game
{
    public class PlayerAnimator : MonoBehaviour
    {
        private const string DefaultIdleTriggerName = "IdleTrigger";
        private const string DefaultWalkTriggerName = "WalkTrigger";
        private const string DefaultRunTriggerName = "RunTrigger";

        [SerializeField] private string idleTriggerName = DefaultIdleTriggerName;
        [SerializeField] private string walkTriggerName = DefaultWalkTriggerName;
        [SerializeField] private string runTriggerName = DefaultRunTriggerName;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            if (animator == null)
            {
                throw new NullReferenceException("");
            }
        }

        private void ResetTriggers()
        {
            animator.ResetTrigger(idleTriggerName);
            animator.ResetTrigger(walkTriggerName);
            animator.ResetTrigger(runTriggerName);
        }

        public enum PlayerAnimatorStatus
        {
            Idle,
            Walk,
            Run
        }

        public void SetState(PlayerAnimatorStatus animatorStatus)
        {
            ResetTriggers();
            
            switch (animatorStatus)
            {
                case PlayerAnimatorStatus.Idle:
                    animator.SetTrigger(idleTriggerName);
                    break;
                case PlayerAnimatorStatus.Walk:
                    animator.SetTrigger(walkTriggerName);
                    break;
                case PlayerAnimatorStatus.Run:
                    animator.SetTrigger(runTriggerName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animatorStatus), animatorStatus, null);
            }
        }
    }
}