
using UnityEngine;

namespace UniversalCharacterController.Scripts
{
    public interface IUCCInputsWrapper
    {
        public UCCInputData InputData { get; }
        
        public void MoveInput(Vector2 newMoveDirection);
        public void LookInput(Vector2 newLookDirection);
        public void JumpInput(bool newJumpState);
        public void SprintInput(bool newSprintState);
    }
}