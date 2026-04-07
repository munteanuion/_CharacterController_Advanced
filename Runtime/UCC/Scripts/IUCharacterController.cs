using System;
using UnityEngine;

namespace UniversalCharacterController.Scripts
{
    public interface IUCharacterController : IDisposable
    {
        bool IsGrounded { get; }
        Transform TargetForCamera { get; }
        void Init(IUCCInputsWrapper inputWrapper);
    }
}