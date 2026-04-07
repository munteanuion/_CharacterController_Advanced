using System;
using UnityEngine;

namespace UniversalCharacterController.Scripts.Modules
{
    [Serializable]
    public class MovementModuleInspector
    {
        [Tooltip("Move speed of the character in m/s")]
        public float moveSpeed = 4.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 6.0f;

        [Tooltip("Acceleration and deceleration rate")]
        public float speedChangeRate = 10.0f;

        [Tooltip("How fast the character turns to face movement direction (Third Person only)")]
        [Range(0.0f, 0.3f)]
        public float thirdPersonRotationSmoothTime = 0.12f;

        [Space(5)]
        [Tooltip("The height the player can jump")]
        public float jumpHeight = 1.2f;

        [Tooltip("Custom gravity value. Engine default is -9.81f")]
        public float gravity = -15.0f;

        [Space(5)]
        [Tooltip("Time required to pass before being able to jump again")]
        public float jumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state")]
        public float fallTimeout = 0.15f;
    }

    
    
    public readonly struct MovementResult
    {
        public readonly float AnimationBlend;
        public readonly float InputMagnitude;

        public MovementResult(float animationBlend, float inputMagnitude)
        {
            AnimationBlend = animationBlend;
            InputMagnitude = inputMagnitude;
        }
    }



    public class MovementModule
    {
        private const float SpeedOffset = 0.1f;
        private const float SpeedRoundFactor = 1000f;
        private const float MinAnimationBlend = 0.01f;
        private const float GroundedVelocityReset = -2f;
        private const float TerminalVelocity = 53.0f;

        private MovementModuleInspector _data;

        private float _speed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        
        
        public void Initialize(MovementModuleInspector data)
        {
            _data = data;
            _jumpTimeoutDelta = data.jumpTimeout;
            _fallTimeoutDelta = data.fallTimeout;
        }

        
        
        public MovementResult MoveFirstPerson(UCCInputData inputData, CharacterController controller, Transform playerTransform)
        {
            float targetSpeed = inputData.sprint ? _data.sprintSpeed : _data.moveSpeed;
            if (inputData.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
            float inputMagnitude = inputData.analogMovement ? inputData.move.magnitude : 1f;

            _speed = CalculateSmoothedSpeed(currentHorizontalSpeed, targetSpeed, inputMagnitude);

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _data.speedChangeRate);
            if (_animationBlend < MinAnimationBlend) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(inputData.move.x, 0.0f, inputData.move.y).normalized;
            if (inputData.move != Vector2.zero)
                inputDirection = playerTransform.right * inputData.move.x + playerTransform.forward * inputData.move.y;

            controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
                            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            return new MovementResult(_animationBlend, inputMagnitude);
        }

        
        
        public MovementResult MoveThirdPerson(UCCInputData inputData, CharacterController controller,
            Transform playerTransform, GameObject mainCamera)
        {
            float targetSpeed = inputData.sprint ? _data.sprintSpeed : _data.moveSpeed;
            if (inputData.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
            float inputMagnitude = inputData.analogMovement ? inputData.move.magnitude : 1f;

            _speed = CalculateSmoothedSpeed(currentHorizontalSpeed, targetSpeed, inputMagnitude);

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _data.speedChangeRate);
            if (_animationBlend < MinAnimationBlend) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(inputData.move.x, 0.0f, inputData.move.y).normalized;
            if (inputData.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, _targetRotation,
                    ref _rotationVelocity, _data.thirdPersonRotationSmoothTime);
                playerTransform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            return new MovementResult(_animationBlend, inputMagnitude);
        }

        
        
        public void UpdatePhysics(UCCInputData inputData, bool grounded, AnimationModule animationModule)
        {
            if (grounded)
            {
                _fallTimeoutDelta = _data.fallTimeout;

                animationModule.SetJump(false);
                animationModule.SetFreeFall(false);

                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = GroundedVelocityReset;

                if (inputData.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(_data.jumpHeight * -2f * _data.gravity);
                    animationModule.SetJump(true);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = _data.jumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;
                else
                    animationModule.SetFreeFall(true);

                // input.jump = false; // нельзя изменять структуру напрямую, если нужно сбросить — добавить метод в UCCInputsWrapper
            }

            if (_verticalVelocity < TerminalVelocity)
                _verticalVelocity += _data.gravity * Time.deltaTime;
        }

        
        
        private float CalculateSmoothedSpeed(float currentSpeed, float targetSpeed, float inputMagnitude)
        {
            if (currentSpeed >= targetSpeed - SpeedOffset && currentSpeed <= targetSpeed + SpeedOffset)
                return targetSpeed;

            float smoothed = Mathf.Lerp(currentSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * _data.speedChangeRate);
            return Mathf.Round(smoothed * SpeedRoundFactor) / SpeedRoundFactor;
        }
    }
}
