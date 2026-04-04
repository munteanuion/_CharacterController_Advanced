using UnityEngine;
using UniversalCharacterController.Scripts;

public class UICanvasControllerInput : MonoBehaviour
{

    [Header("Output")] public UCCInputs uccInputs;

    public void VirtualMoveInput(Vector2 virtualMoveDirection)
    {
        uccInputs.MoveInput(virtualMoveDirection);
    }

    public void VirtualLookInput(Vector2 virtualLookDirection)
    {
        uccInputs.LookInput(virtualLookDirection);
    }

    public void VirtualJumpInput(bool virtualJumpState)
    {
        uccInputs.JumpInput(virtualJumpState);
    }

    public void VirtualSprintInput(bool virtualSprintState)
    {
        uccInputs.SprintInput(virtualSprintState);
    }

}


