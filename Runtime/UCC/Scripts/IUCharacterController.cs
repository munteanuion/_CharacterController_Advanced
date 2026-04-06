using UnityEngine;

namespace UniversalCharacterController.Scripts
{
    public interface IUCharacterController
    {
        public bool IsGrounded { get; }
        public Transform TargetForCamera { get; }
    }
}