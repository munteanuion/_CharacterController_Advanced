using UnityEngine;

namespace UniversalCharacterController.Scripts
{
    [System.Serializable]
    public struct UCCInputData
    {
        [Header("Character Input Values")] 
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        [Header("Movement Settings")] 
        public bool analogMovement;

        [Header("Mouse Cursor Settings")] 
        public bool cursorLocked;
        public bool cursorInputForLook;

        [Header("Look Settings")]
        [Tooltip("Sensitivity multiplier for camera look input")]
        public float lookSensitivity;

        [Header("Invert Settings")]
        public bool invertLookY;
    }
}