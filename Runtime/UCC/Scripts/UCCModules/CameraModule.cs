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
        
        
        public void LateUpdate(UCCInputData inputData, bool isMouseInput, Transform playerTransform)
        {
            if (_data.perspectiveMode == PerspectiveMode.FirstPerson)
                RotateFirstPerson(inputData, playerTransform, isMouseInput);
            else
                RotateThirdPerson(inputData, isMouseInput);
        }


        private void RotateFirstPerson(UCCInputData inputData, Transform playerTransform, bool isMouseInput)
        {
            if (inputData.look.sqrMagnitude < Threshold) return;

            float deltaTimeMultiplier = isMouseInput ? 1.0f : Time.deltaTime;
            float sensitivity = inputData.lookSensitivity > 0f ? inputData.lookSensitivity : 1f;
            float yawDelta = inputData.look.x * _data.firstPersonRotationSpeed * deltaTimeMultiplier * sensitivity;

            float lookY = inputData.invertLookY ? -inputData.look.y : inputData.look.y;
            _cinemachineTargetPitch += lookY * _data.firstPersonRotationSpeed * deltaTimeMultiplier * sensitivity;
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _data.bottomClamp, _data.topClamp);

            _data.cinemachineCameraTarget.localRotation =
                Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            playerTransform.Rotate(Vector3.up * yawDelta);
        }


        private void RotateThirdPerson(UCCInputData inputData, bool isMouseInput)
        {
            if (!_data.lockCameraPosition && inputData.look.sqrMagnitude >= Threshold)
            {
                float deltaTimeMultiplier = isMouseInput ? 1.0f : Time.deltaTime;
                float sensitivity = inputData.lookSensitivity > 0f ? inputData.lookSensitivity : 1f;
                _cinemachineTargetYaw += inputData.look.x * deltaTimeMultiplier * sensitivity;
                float lookY = inputData.invertLookY ? -inputData.look.y : inputData.look.y;
                _cinemachineTargetPitch += lookY * deltaTimeMultiplier * sensitivity;
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
