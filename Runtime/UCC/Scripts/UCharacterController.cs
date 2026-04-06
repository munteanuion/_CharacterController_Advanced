using UnityEngine;
using UnityEngine.InputSystem;
using UniversalCharacterController.Scripts.Configs;
using UniversalCharacterController.Scripts.Modules;

namespace UniversalCharacterController.Scripts
{
    /// <summary>
    /// Universal Player Controller supporting both First Person and Third Person perspectives.
    /// Requires CharacterController, StarterAssetsInputs, and PlayerInput (New Input System) on the same GameObject.
    /// Configure each module's Inspector data in the Inspector, assign CinemachineCameraTarget in Camera module,
    /// and set GroundLayers in Grounded module. Switch perspective via SwitchPerspective() or TogglePerspective().
    /// </summary>
    
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class UCharacterController : MonoBehaviour, IUCharacterController
    {
        #region Inspector
        
        [SerializeField] private MovementModuleInspector movementInspector;
        [SerializeField] private CameraModuleInspector cameraInspector;
        [SerializeField] private GroundedModuleInspector groundedInspector;
        [SerializeField] private AnimationModuleInspector animationInspector;
        [SerializeField] private AudioModuleInspector audioInspector;

        #endregion

        
        
        #region Logic Modules

        private readonly MovementModule _movementModule = new ();
        private readonly CameraModule _cameraModule = new ();
        private readonly GroundedModule _groundedModule = new ();
        private readonly AnimationModule _animationModule = new ();
        private readonly AudioModule _audioModule = new ();

        #endregion

        
        
        #region Private State

        private CharacterController _controller;
        private UCCInputs _input;
        private GameObject _mainCamera;
        private PlayerInput _playerInput;

        #endregion

        
        
        #region Properties

        public Transform TargetForCamera => cameraInspector.cinemachineCameraTarget;
        public bool IsGrounded { get; private set; } = true;
        private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

        #endregion

        
        
        #region Unity Lifecycle

        private void Awake()
        {
            _mainCamera = Camera.main?.gameObject;
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<UCCInputs>();
            _playerInput = GetComponent<PlayerInput>();

            TryGetComponent(out Animator detectedAnimator);

            float initialCameraYaw = cameraInspector.cinemachineCameraTarget != null
                ? cameraInspector.cinemachineCameraTarget.transform.rotation.eulerAngles.y
                : 0f;

            _movementModule.Initialize(movementInspector);
            _cameraModule.Initialize(cameraInspector, initialCameraYaw);
            _groundedModule.Initialize(groundedInspector);
            _animationModule.Initialize(animationInspector, detectedAnimator);
            _audioModule.Initialize(audioInspector);
        }

        private void Update()
        {
            IsGrounded = _groundedModule.Check(transform.position);
            _animationModule.SetGrounded(IsGrounded);
            _movementModule.UpdatePhysics(_input, IsGrounded, _animationModule);

            MovementResult movementResult = cameraInspector.perspectiveMode == PerspectiveMode.FirstPerson
                ? _movementModule.MoveFirstPerson(_input, _controller, transform)
                : _movementModule.MoveThirdPerson(_input, _controller, transform, _mainCamera);

            _animationModule.SetSpeed(movementResult.AnimationBlend);
            _animationModule.SetMotionSpeed(movementResult.InputMagnitude);
        }

        private void LateUpdate()
        {
            _cameraModule.LateUpdate(_input, IsCurrentDeviceMouse, transform);
        }

        #endregion

        
        
        #region Public API

        public void SwitchPerspective(PerspectiveMode mode)
        {
            cameraInspector.perspectiveMode = mode;
        }

        #endregion

        
        
        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            _groundedModule.DrawGizmos(
                transform.position, 
                IsGrounded, 
                groundedInspector.groundedOffset, 
                groundedInspector.groundedRadius);
        }

        #endregion

        
        
        #region Animation Events

        private void OnFootstep(AnimationEvent animationEvent)
        {
            _audioModule.PlayFootstep(animationEvent.animatorClipInfo.weight);
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            _audioModule.PlayLanding(animationEvent.animatorClipInfo.weight);
        }

        #endregion
    }
}
