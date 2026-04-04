using System;
using UnityEngine;

namespace UniversalCharacterController.Scripts.Tools
{
    public class FollowTarget : MonoBehaviour
    {
        private enum UpdateLoop { Update, LateUpdate, FixedUpdate }

        [SerializeField] private Transform target;
        [SerializeField] private UpdateLoop updateLoop = UpdateLoop.LateUpdate;

        [Header("Position")]
        [SerializeField] private bool followPosition = true;
        [SerializeField] private bool followX = true;
        [SerializeField] private bool followY = true;
        [SerializeField] private bool followZ = true;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private bool lerpPosition;
        [SerializeField] private float positionLerpSpeed = 10f;

        [Header("Rotation")]
        [SerializeField] private bool followRotation;
        [SerializeField] private bool followRotationX = true;
        [SerializeField] private bool followRotationY = true;
        [SerializeField] private bool followRotationZ = true;
        [SerializeField] private Vector3 rotationOffset;
        [SerializeField] private bool lerpRotation;
        [SerializeField] private float rotationLerpSpeed = 10f;

        private void Update()
        {
            if (updateLoop != UpdateLoop.Update) return;
            Tick(Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (updateLoop != UpdateLoop.LateUpdate) return;
            Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (updateLoop != UpdateLoop.FixedUpdate) return;
            Tick(Time.fixedDeltaTime);
        }

        private void Tick(float deltaTime)
        {
            if (!target) return;
            ApplyPosition(deltaTime);
            ApplyRotation(deltaTime);
        }

        private void ApplyPosition(float deltaTime)
        {
            if (!followPosition) return;
            
            var current = transform.position;
            var worldOffset = target.TransformDirection(positionOffset);
            var targetPos = target.position + worldOffset;

            var desired = new Vector3(
                followX ? targetPos.x : current.x,
                followY ? targetPos.y : current.y,
                followZ ? targetPos.z : current.z
            );

            transform.position = lerpPosition
                ? Vector3.Lerp(current, desired, positionLerpSpeed * deltaTime)
                : desired;
        }

        private void ApplyRotation(float deltaTime)
        {
            if (!followRotation) return;

            var currentEuler = transform.eulerAngles;
            var desired = target.rotation * Quaternion.Euler(rotationOffset);
            var desiredEuler = new Vector3(
                followRotationX ? desired.eulerAngles.x : currentEuler.x,
                followRotationY ? desired.eulerAngles.y : currentEuler.y,
                followRotationZ ? desired.eulerAngles.z : currentEuler.z
            );

            var finalRotation = Quaternion.Euler(desiredEuler);

            transform.rotation = lerpRotation
                ? Quaternion.Lerp(transform.rotation, finalRotation, rotationLerpSpeed * deltaTime)
                : finalRotation;
        }


        private void OnValidate()
        {
            ApplyPosition(1);
            ApplyRotation(1);
        }
    }
}