using System;
using UnityEngine;
using UniversalCharacterController.Scripts.Configs;

namespace UniversalCharacterController.Scripts.Modules
{
    [Serializable]
    public class CameraModuleInspector
    {
        public PerspectiveMode perspectiveMode = PerspectiveMode.ThirdPerson;
        
        [Tooltip("The follow target set in the Cinemachine Virtual Camera")]
        public Transform cinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float topClamp = 90.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float bottomClamp = -90.0f;

        [Tooltip("Additional degrees to override the camera angle (Third Person only)")]
        public float cameraAngleOverride = 0.0f;

        [Tooltip("Lock the camera position on all axes (Third Person only)")]
        public bool lockCameraPosition = false;

        [Space(5)]
        [Tooltip("Camera rotation speed (First Person mode only)")]
        public float firstPersonRotationSpeed = 1.0f;
    }
    
    

    public class CameraModule
    {
        private const float Threshold = 0.01f;

        private CameraModuleInspector _data;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        
        
        public void Initialize(CameraModuleInspector data, float initialYaw)
        {
            _data = data;
            _cinemachineTargetYaw = initialYaw;
        }
        
        
        public void LateUpdate(UCCInputsWrapper input, bool isMouseInput, Transform playerTransform)
        {
            if (_data.perspectiveMode == PerspectiveMode.FirstPerson)
                RotateFirstPerson(input, playerTransform, isMouseInput);
            else
                RotateThirdPerson(input, isMouseInput);
        }


        private void RotateFirstPerson(UCCInputsWrapper input, Transform playerTransform, bool isMouseInput)
        {
            if (input.look.sqrMagnitude < Threshold) return;

            float deltaTimeMultiplier = isMouseInput ? 1.0f : Time.deltaTime;
            float yawDelta = input.look.x * _data.firstPersonRotationSpeed * deltaTimeMultiplier;

            _cinemachineTargetPitch += input.look.y * _data.firstPersonRotationSpeed * deltaTimeMultiplier;
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _data.bottomClamp, _data.topClamp);

            _data.cinemachineCameraTarget.localRotation =
                Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            playerTransform.Rotate(Vector3.up * yawDelta);
        }


        private void RotateThirdPerson(UCCInputsWrapper input, bool isMouseInput)
        {
            if (!_data.lockCameraPosition && input.look.sqrMagnitude >= Threshold)
            {
                float deltaTimeMultiplier = isMouseInput ? 1.0f : Time.deltaTime;
                _cinemachineTargetYaw += input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += input.look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _data.bottomClamp, _data.topClamp);

            _data.cinemachineCameraTarget.rotation = Quaternion.Euler(
                _cinemachineTargetPitch + _data.cameraAngleOverride,
                _cinemachineTargetYaw,
                0.0f);
        }

        
        
        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
