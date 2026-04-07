using UnityEngine;
using UnityEngine.InputSystem;

namespace UniversalCharacterController.Scripts
{
	public class UCCInputsWrapper : MonoBehaviour, IUCCInputsWrapper
	{
		[SerializeField] private UCCInputData inputData;

		public UCCInputData InputData => inputData;

		
		
		public void MoveInput(Vector2 newMoveDirection)
		{
			inputData.move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			inputData.look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			inputData.jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			inputData.sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(inputData.cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}



		#region SEND MESSAGE FROM INPUT COMPONENT FOR TEST
		
		public void OnMove(InputValue value) => MoveInput(value.Get<Vector2>());
		public void OnJump(InputValue value) => JumpInput(value.isPressed);
		public void OnSprint(InputValue value) => SprintInput(value.isPressed);

		public void OnLook(InputValue value)
		{
			if (inputData.cursorInputForLook) LookInput(value.Get<Vector2>());
		}

		#endregion
	}
}
