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
        private IUCCInputsWrapper _input;
        private UCCInputData InputData => _input?.InputData ?? default;
        private GameObject _mainCamera;

        #endregion

        
        
        #region Properties

        public Transform TargetForCamera => cameraInspector.cinemachineCameraTarget;
        public bool IsGrounded { get; private set; } = true;

        #endregion

        
        
        #region Unity Lifecycle
        
        public void Init(IUCCInputsWrapper inputWrapper)
        {
            _input = inputWrapper;
            _mainCamera = Camera.main?.gameObject;
            _controller = GetComponent<CharacterController>();
        }

        public void Dispose()
        {
            
        }
        
        
        
        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Teleport(Vector3 position, Vector3 eulerAngles)
        {
            _controller.enabled = false;
            transform.position = position;
            transform.eulerAngles = eulerAngles;
            _controller.enabled = true;
        }

        
        
        private void Awake()
        {
            Init(GetComponent<IUCCInputsWrapper>());
        }

        private void Start()
        {
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
            var inputData = InputData;
            IsGrounded = _groundedModule.Check(transform.position);
            _animationModule.SetGrounded(IsGrounded);
            _movementModule.UpdatePhysics(inputData, IsGrounded, _animationModule);

            MovementResult movementResult = cameraInspector.perspectiveMode == PerspectiveMode.FirstPerson
                ? _movementModule.MoveFirstPerson(inputData, _controller, transform)
                : _movementModule.MoveThirdPerson(inputData, _controller, transform, _mainCamera);

            _animationModule.SetSpeed(movementResult.AnimationBlend);
            _animationModule.SetMotionSpeed(movementResult.InputMagnitude);
        }

        private void LateUpdate()
        {
            var inputData = InputData;
            _cameraModule.LateUpdate(inputData, false/*IsCurrentDeviceMouse*/, transform);
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
