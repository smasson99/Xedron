using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject cameraGameObject;

        [SerializeField]
        private Transform forwardTransform;

        [SerializeField]
        private Transform visualTransform;

        [Header("Input Settings")]
        [Tooltip("The maximum strength to apply to activate the status IsWalking to the player.")]
        [SerializeField]
        [Range(0, 1)]
        private float isWalkingThreshold = 0.25f;

        [Header("Configuration")]
        [Tooltip("The walk speed of the player.")]
        [SerializeField]
        [Range(0.01f, 100)]
        private float walkSpeed = 3.5f;

        [Tooltip("The run speed of the player.")] [SerializeField] [Range(0.01f, 100)]
        private float runSpeed = 3.5f;

        private XboxOneControllerInput xboxOneControllerInput;
        private Vector3 leftJoysticDirection;

        private CharacterController characterController;
        private bool canMove;

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

            if (characterController == null)
            {
                throw new NullReferenceException(nameof(CharacterController) + "not found!");
            }

            if (playerAnimator == null)
            {
                throw new NullReferenceException(nameof(playerAnimator) + "not found!");
            }

            if (cameraGameObject is null)
            {
                throw new NullReferenceException(nameof(cameraGameObject));
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
        }

        private void UnSubscribeToXBoxOneControllerInputs()
        {
            xboxOneControllerInput.OnLeftJoystickDirectionChanged -= UpdateLeftJoysticDirection;
        }

        private void UpdateLeftJoysticDirection(Vector2 direction)
        {
            leftJoysticDirection = new Vector3(direction.x, 0, direction.y);
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

            characterController.Move(forwardTransform.TransformDirection(leftJoysticDirection) * movingSpeed *
                                     Time.deltaTime);

            CurrentPlayerState = isWalking ? PlayerState.Walk : PlayerState.Run;
        }

        private void SetLookRotation(Vector3 direction)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        private void UpdateVisualRotation(Vector3 rotation)
        {
            visualTransform.rotation = Quaternion.LookRotation(rotation);
        }

        private void Update()
        {
            if (LeftJoysticIsUsed())
            {
                UpdateVisualRotation(forwardTransform.TransformDirection(leftJoysticDirection));

                Move();
            }
            else
            {
                CurrentPlayerState = PlayerState.Idle;
            }
        }
    }
}