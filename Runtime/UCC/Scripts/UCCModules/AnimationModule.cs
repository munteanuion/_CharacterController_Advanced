using System;
using UnityEngine;

namespace UniversalCharacterController.Scripts.Modules
{
    [Serializable]
    public class AnimationModuleInspector
    {
        [Tooltip("Optional Animator override. Leave empty to auto-detect from the same GameObject.")]
        public Animator animatorOverride;
    }

    public class AnimationModule
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int GroundedHash = Animator.StringToHash("Grounded");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int FreeFallHash = Animator.StringToHash("FreeFall");
        private static readonly int MotionSpeedHash = Animator.StringToHash("MotionSpeed");

        private Animator _animator;

        public bool HasAnimator { get; private set; }

        public void Initialize(AnimationModuleInspector data, Animator fallbackAnimator)
        {
            _animator = data.animatorOverride != null ? data.animatorOverride : fallbackAnimator;
            HasAnimator = _animator != null;
        }

        public void SetSpeed(float speed)
        {
            if (!HasAnimator) return;
            _animator.SetFloat(SpeedHash, speed);
        }

        public void SetMotionSpeed(float motionSpeed)
        {
            if (!HasAnimator) return;
            _animator.SetFloat(MotionSpeedHash, motionSpeed);
        }

        public void SetGrounded(bool isGrounded)
        {
            if (!HasAnimator) return;
            _animator.SetBool(GroundedHash, isGrounded);
        }

        public void SetJump(bool isJumping)
        {
            if (!HasAnimator) return;
            _animator.SetBool(JumpHash, isJumping);
        }

        public void SetFreeFall(bool isFreeFalling)
        {
            if (!HasAnimator) return;
            _animator.SetBool(FreeFallHash, isFreeFalling);
        }
    }
}

