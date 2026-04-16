using System;
using UnityEngine;

namespace UniversalCharacterController.Scripts
{
    public interface IUCharacterController : IDisposable
    {
        bool IsGrounded { get; }
        Transform TargetForCamera { get; }
        void Init(IUCCInputsWrapper inputWrapper);
        void Enable();
        void Disable();
        void Teleport(Vector3 position, Vector3 eulerAngles);
    }
}