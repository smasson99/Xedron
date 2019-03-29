using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        [Tooltip("The maximum strength to apply to activate the status IsWalking to the player.")]
        [SerializeField]
        [Range(0, 1)]
        private float isWalkingThreshold = 0.25f;

        [Tooltip("The delay in seconds before moving the player.")] [SerializeField] [Range(0, 2.3f)]
        private float moveDelayInSeconds = 0;

        private bool canMove;

        private XboxOneControllerInput xboxOneControllerInput;
        private ThirdPersonCamera thirdPersonCamera;

        private Vector3 leftJoysticDirection;
        private CharacterController characterController;

        private PlayerAnimator playerAnimator;

        private enum PlayerState
        {
            Disabled,
            Idle,
            Walk,
            Run
        }

        private PlayerState currentPlayerState;

        private PlayerState CurrentPlayerState
        {
            get => currentPlayerState;
            set
            {
                if (value != currentPlayerState)
                {
                    currentPlayerState = value;

                    UpdateState();
                }
            }
        }

        [Tooltip("The walk speed of the player.")] [SerializeField] [Range(0.01f, 100)]
        private float walkSpeed = 3.5f;

        [Tooltip("The run speed of the player.")] [SerializeField] [Range(0.01f, 100)]
        private float runSpeed = 3.5f;

        private void UpdateState()
        {
            switch (currentPlayerState)
            {
                case PlayerState.Idle:
                    playerAnimator.SetState(PlayerAnimator.PlayerAnimatorStatus.Idle);
                    break;
                case PlayerState.Walk:
                    playerAnimator.SetState(PlayerAnimator.PlayerAnimatorStatus.Walk);
                    break;
                case PlayerState.Run:
                    playerAnimator.SetState(PlayerAnimator.PlayerAnimatorStatus.Run);
                    break;
                case PlayerState.Disabled:
                    playerAnimator.SetState(PlayerAnimator.PlayerAnimatorStatus.Idle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Awake()
        {
            GetComponents();
            InitializeValues();
            VerifyComponents();
        }

        private void GetComponents()
        {
            xboxOneControllerInput = GetComponentInChildren<XboxOneControllerInput>();
            thirdPersonCamera = Camera.main?.GetComponent<ThirdPersonCamera>();
            characterController = GetComponent<CharacterController>();
            playerAnimator = GetComponentInChildren<PlayerAnimator>();
        }

        private void InitializeValues()
        {
            leftJoysticDirection = Vector2.zero;
            canMove = false;
        }

        private void VerifyComponents()
        {
            if (xboxOneControllerInput == null)
            {
                throw new NullReferenceException(nameof(xboxOneControllerInput) + " not found!");
            }

            if (thirdPersonCamera == null)
            {
                throw new NullReferenceException(nameof(thirdPersonCamera) + " not found!");
            }

            if (characterController == null)
            {
                throw new NullReferenceException(nameof(CharacterController) + "not found!");
            }

            if (characterController == null)
            {
                throw new NullReferenceException(nameof(playerAnimator) + "not found!");
            }
        }

        private void OnEnable()
        {
            SubscribeToXBoxOneControllerInputs();
        }

        private void OnDisable()
        {
            UnSubscribeToXBoxOneControllerInputs();
        }

        private void SubscribeToXBoxOneControllerInputs()
        {
            xboxOneControllerInput.OnLeftJoystickDirectionChanged += UpdateLeftJoysticDirection;
            xboxOneControllerInput.OnRightJoystickDirectionChanged += UpdateRightJoystickDirection;
        }

        private void UnSubscribeToXBoxOneControllerInputs()
        {
            xboxOneControllerInput.OnLeftJoystickDirectionChanged -= UpdateLeftJoysticDirection;
            xboxOneControllerInput.OnRightJoystickDirectionChanged -= UpdateRightJoystickDirection;
        }

        private void UpdateLeftJoysticDirection(Vector2 direction)
        {
            leftJoysticDirection = new Vector3(direction.x, 0, direction.y);
        }

        private void UpdateRightJoystickDirection(Vector2 direction)
        {
            thirdPersonCamera.UpdateRightJoysticDirection(direction);
        }

        private bool LeftJoysticIsUsed()
        {
            return leftJoysticDirection != Vector3.zero;
        }

        private bool IsWalking()
        {
            float magnitude = xboxOneControllerInput.LeftJoysticDirection.sqrMagnitude;

            return LeftJoysticIsUsed() && magnitude < isWalkingThreshold * isWalkingThreshold;
        }

        private void Move()
        {
            bool isWalking = IsWalking();

            float movingSpeed = isWalking ? walkSpeed : runSpeed;

            characterController.Move(transform.forward * movingSpeed *
                                     Time.deltaTime);

            CurrentPlayerState = isWalking ? PlayerState.Walk : PlayerState.Run;
        }

        private void SetLookRotation(Vector3 direction)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        private void Update()
        {
            if (LeftJoysticIsUsed())
            {
                SetLookRotation(leftJoysticDirection);

                Move();
            }
            else
            {
                CurrentPlayerState = PlayerState.Idle;
            }
        }
    }
}