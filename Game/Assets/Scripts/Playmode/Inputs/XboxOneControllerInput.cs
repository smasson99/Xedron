using System;
using UnityEngine;
using XInputDotNetPure;

namespace Game
{
    public delegate void TriggerInputHandler();

    public delegate void DirectionalInputHandler(Vector2 direction);

    public class XboxOneControllerInput : MonoBehaviour
    {
        private GamePadState gamePadState;

        [Header("Player")]
        [Tooltip("The index of the player, should be always zero if the game is single player.")]
        [SerializeField]
        private PlayerIndex playerIndex = 0;

        [Header("Configs")]
        [Tooltip("The strength to apply to activate one of the left and right triggers.")]
        [SerializeField]
        private float triggerPressedThreshold = 0.25f;

        [Tooltip(
            "Minimum length of a thumbstick direction which is considered a non-null direction. Cannot be modified at runtime.")]
        [SerializeField] [Range(0, 1)]
        private float directionThreshold = 0.25f;
        

#if UNITY_EDITOR
        [Header("Debug")]
        [Tooltip("Do you want the history of the buttons pressed by the user to show in the log panel?")]
        [SerializeField]
        private bool logDebugEnabled;
#endif

        private enum GamePadButtonCode
        {
            A,
            B,
            X,
            Y,
            LeftBumper,
            RightBumper,
            LeftTrigger,
            RightTrigger,
            LeftStickButton,
            RightStickButton,
            Back,
            Start,
            Left,
            Right,
            Up,
            Down
        }

        private bool aPressed;
        private bool bPressed;
        private bool xPressed;
        private bool yPressed;
        private bool leftBumperPressed;
        private bool rightBumperPressed;
        private bool leftTriggerPressed;
        private bool rightTriggerPressed;
        private bool leftStickButtonPressed;
        private bool rightStickButtonPressed;
        private bool backPressed;
        private bool startPressed;
        private bool leftPressed;
        private bool rightPressed;
        private bool upPressed;
        private bool downPressed;

        private Vector2 leftJoysticDirection;
        private Vector2 rightJoysticDirection;

        private readonly float squaredDirectionThreshold;

        public event TriggerInputHandler OnAPressed;
        public event TriggerInputHandler OnBPressed;
        public event TriggerInputHandler OnXPressed;
        public event TriggerInputHandler OnYPressed;
        public event TriggerInputHandler OnLeftBumperPressed;
        public event TriggerInputHandler OnRightBumperPressed;
        public event TriggerInputHandler OnLeftTriggerPressed;
        public event TriggerInputHandler OnRightTriggerPressed;
        public event TriggerInputHandler OnBackPressed;
        public event TriggerInputHandler OnStartPressed;
        public event TriggerInputHandler OnLeftStickButtonPressed;
        public event TriggerInputHandler OnRightStickButtonPressed;
        public event TriggerInputHandler OnLeftPressed;
        public event TriggerInputHandler OnRightPressed;
        public event TriggerInputHandler OnUpPressed;
        public event TriggerInputHandler OnDownPressed;

        public event TriggerInputHandler OnAReleased;
        public event TriggerInputHandler OnBReleased;
        public event TriggerInputHandler OnXReleased;
        public event TriggerInputHandler OnYReleased;
        public event TriggerInputHandler OnLeftBumperReleased;
        public event TriggerInputHandler OnRightBumperReleased;
        public event TriggerInputHandler OnLeftTriggerReleased;
        public event TriggerInputHandler OnRightTriggerReleased;
        public event TriggerInputHandler OnBackReleased;
        public event TriggerInputHandler OnStartReleased;
        public event TriggerInputHandler OnLeftStickButtonReleased;
        public event TriggerInputHandler OnRightStickButtonReleased;
        public event TriggerInputHandler OnLeftReleased;
        public event TriggerInputHandler OnRightReleased;
        public event TriggerInputHandler OnUpReleased;
        public event TriggerInputHandler OnDownReleased;

        public event DirectionalInputHandler OnLeftJoystickDirectionChanged;
        public event DirectionalInputHandler OnRightJoystickDirectionChanged;

        public bool APressed
        {
            get { return aPressed; }
            private set
            {
                if (aPressed != value)
                {
                    if (value) NotifyAPressed();
                    else NotifyAReleased();

                    aPressed = value;
                }
            }
        }

        public bool BPressed
        {
            get { return bPressed; }
            private set
            {
                if (bPressed != value)
                {
                    if (value) NotifyBPressed();
                    else NotifyBReleased();

                    bPressed = value;
                }
            }
        }

        public bool XPressed
        {
            get { return xPressed; }
            private set
            {
                if (xPressed != value)
                {
                    if (value) NotifyXPressed();
                    else NotifyXReleased();

                    xPressed = value;
                }
            }
        }

        public bool YPressed
        {
            get { return yPressed; }
            private set
            {
                if (yPressed != value)
                {
                    if (value) NotifyYPressed();
                    else NotifyYReleased();

                    yPressed = value;
                }
            }
        }

        public bool LeftBumperPressed
        {
            get { return leftBumperPressed; }
            private set
            {
                if (leftBumperPressed != value)
                {
                    if (value) NotifyLeftBumperPressed();
                    else NotifyLeftBumperReleased();

                    leftBumperPressed = value;
                }
            }
        }

        public bool RightBumperPressed
        {
            get { return rightBumperPressed; }
            private set
            {
                if (rightBumperPressed != value)
                {
                    if (value) NotifyRightBumperPressed();
                    else NotifyRightBumperReleased();

                    rightBumperPressed = value;
                }
            }
        }

        public bool LeftTriggerPressed
        {
            get { return leftTriggerPressed; }
            private set
            {
                if (leftTriggerPressed != value)
                {
                    if (value) NotifyLeftTriggerPressed();
                    else NotifyLeftTriggerReleased();

                    leftTriggerPressed = value;
                }
            }
        }

        public bool RightTriggerPressed
        {
            get { return rightTriggerPressed; }
            private set
            {
                if (rightTriggerPressed != value)
                {
                    if (value) NotifyRightTriggerPressed();
                    else NotifyRightTriggerReleased();

                    rightTriggerPressed = value;
                }
            }
        }

        public bool LeftStickButtonPressed
        {
            get { return leftStickButtonPressed; }
            private set
            {
                if (leftStickButtonPressed != value)
                {
                    if (value) NotifyLeftStickButtonPressed();
                    else NotifyLeftStickButtonReleased();

                    leftStickButtonPressed = value;
                }
            }
        }

        public bool RightStickButtonPressed
        {
            get { return rightStickButtonPressed; }
            private set
            {
                if (rightStickButtonPressed != value)
                {
                    if (value) NotifyRightStickButtonPressed();
                    else NotifyRightStickButtonReleased();

                    rightStickButtonPressed = value;
                }
            }
        }

        public bool BackPressed
        {
            get { return backPressed; }
            private set
            {
                if (backPressed != value)
                {
                    if (value) NotifyBackPressed();
                    else NotifyBackReleased();

                    backPressed = value;
                }
            }
        }

        public bool StartPressed
        {
            get { return startPressed; }
            private set
            {
                if (startPressed != value)
                {
                    if (value) NotifyStartPressed();
                    else NotifyStartReleased();

                    startPressed = value;
                }
            }
        }

        public bool LeftPressed
        {
            get { return leftPressed; }
            private set
            {
                if (leftPressed != value)
                {
                    if (value) NotifyLeftPressed();
                    else NotifyLeftReleased();

                    leftPressed = value;
                }
            }
        }

        public bool RightPressed
        {
            get { return rightPressed; }
            private set
            {
                if (rightPressed != value)
                {
                    if (value) NotifyRightPressed();
                    else NotifyRightReleased();

                    rightPressed = value;
                }
            }
        }

        public bool UpPressed
        {
            get { return upPressed; }
            private set
            {
                if (upPressed != value)
                {
                    if (value) NotifyUpPressed();
                    else NotifyUpReleased();

                    upPressed = value;
                }
            }
        }

        public bool DownPressed
        {
            get { return downPressed; }
            private set
            {
                if (downPressed != value)
                {
                    if (value) NotifyDownPressed();
                    else NotifyDownReleased();

                    downPressed = value;
                }
            }
        }

        public Vector2 LeftJoysticDirection
        {
            get { return leftJoysticDirection; }
            set
            {
                if (!leftJoysticDirection.Equals(value))
                {
                    NotifyLeftJoystickDirectionChanged(value);

                    leftJoysticDirection = value;
                }
            }
        }

        public Vector2 RightJoysticDirection
        {
            get { return rightJoysticDirection; }
            set
            {
                if (!rightJoysticDirection.Equals(value))
                {
                    NotifyRightJoystickDirectionChanged(value);

                    rightJoysticDirection = value;
                }
            }
        }

        private void SayToLog(string message)
        {
#if UNITY_EDITOR
            if (logDebugEnabled) Debug.Log(message);
#endif
        }

        private void NotifyAPressed()
        {
            OnAPressed?.Invoke();

            SayToLog("A pressed.");
        }

        private void NotifyBPressed()
        {
            OnBPressed?.Invoke();

            SayToLog("B pressed.");
        }

        private void NotifyXPressed()
        {
            OnXPressed?.Invoke();

            SayToLog("X pressed.");
        }

        private void NotifyYPressed()
        {
            OnYPressed?.Invoke();

            SayToLog("Y pressed.");
        }

        private void NotifyLeftBumperPressed()
        {
            OnLeftBumperPressed?.Invoke();

            SayToLog("Left Bumper pressed.");
        }

        private void NotifyRightBumperPressed()
        {
            OnRightBumperPressed?.Invoke();

            SayToLog("Right Bumper pressed");
        }

        private void NotifyLeftTriggerPressed()
        {
            OnLeftTriggerPressed?.Invoke();

            SayToLog("Left Trigger pressed.");
        }

        private void NotifyRightTriggerPressed()
        {
            OnRightTriggerPressed?.Invoke();

            SayToLog("Right Trigger pressed.");
        }

        private void NotifyLeftStickButtonPressed()
        {
            OnLeftStickButtonPressed?.Invoke();

            SayToLog("Left Stick Button pressed.");
        }

        private void NotifyRightStickButtonPressed()
        {
            OnRightStickButtonPressed?.Invoke();

            SayToLog("Right Stick Button pressed.");
        }

        private void NotifyBackPressed()
        {
            OnBackPressed?.Invoke();

            SayToLog("Back pressed.");
        }

        private void NotifyStartPressed()
        {
            OnStartPressed?.Invoke();

            SayToLog("Start pressed.");
        }

        private void NotifyLeftPressed()
        {
            OnLeftPressed?.Invoke();

            SayToLog("Left pressed.");
        }

        private void NotifyRightPressed()
        {
            OnRightPressed?.Invoke();

            SayToLog("Right pressed.");
        }

        private void NotifyUpPressed()
        {
            OnUpPressed?.Invoke();

            SayToLog("Up pressed.");
        }

        private void NotifyDownPressed()
        {
            OnDownPressed?.Invoke();

            SayToLog("Down pressed.");
        }

        private void NotifyAReleased()
        {
            OnAReleased?.Invoke();

            SayToLog("A released.");
        }

        private void NotifyBReleased()
        {
            OnBReleased?.Invoke();

            SayToLog("B released.");
        }

        private void NotifyXReleased()
        {
            OnXReleased?.Invoke();

            SayToLog("X released.");
        }

        private void NotifyYReleased()
        {
            OnYReleased?.Invoke();

            SayToLog("Y released.");
        }

        private void NotifyLeftBumperReleased()
        {
            OnLeftBumperReleased?.Invoke();

            SayToLog("Left Bumper released.");
        }

        private void NotifyRightBumperReleased()
        {
            OnRightBumperReleased?.Invoke();

            SayToLog("Right Bumper released.");
        }

        private void NotifyLeftTriggerReleased()
        {
            OnLeftTriggerReleased?.Invoke();

            SayToLog("Left Trigger released.");
        }

        private void NotifyRightTriggerReleased()
        {
            OnRightTriggerReleased?.Invoke();

            SayToLog("Right Trigger released.");
        }

        private void NotifyLeftStickButtonReleased()
        {
            OnLeftStickButtonReleased?.Invoke();

            SayToLog("Left Stick Button released.");
        }

        private void NotifyRightStickButtonReleased()
        {
            OnRightStickButtonReleased?.Invoke();

            SayToLog("Right Stick Button released.");
        }

        private void NotifyBackReleased()
        {
            OnBackReleased?.Invoke();

            SayToLog("Back released.");
        }

        private void NotifyStartReleased()
        {
            OnStartReleased?.Invoke();

            SayToLog("Start released.");
        }

        private void NotifyLeftReleased()
        {
            OnLeftReleased?.Invoke();

            SayToLog("Left released.");
        }

        private void NotifyRightReleased()
        {
            OnRightReleased?.Invoke();

            SayToLog("Right released.");
        }

        private void NotifyUpReleased()
        {
            OnUpReleased?.Invoke();

            SayToLog("Up released.");
        }

        private void NotifyDownReleased()
        {
            OnDownReleased?.Invoke();

            SayToLog("Down released.");
        }

        private void NotifyLeftJoystickDirectionChanged(Vector2 direction)
        {
            OnLeftJoystickDirectionChanged?.Invoke(direction);
        }

        private void NotifyRightJoystickDirectionChanged(Vector2 direction)
        {
            OnRightJoystickDirectionChanged?.Invoke(direction);
        }

        private void UpdateState()
        {
            gamePadState = GamePad.GetState(playerIndex);
        }

        private bool CheckIfXInputButtonActivated(GamePadButtonCode buttonCode)
        {
            switch (buttonCode)
            {
                case GamePadButtonCode.A:
                    return gamePadState.Buttons.A == ButtonState.Pressed;
                case GamePadButtonCode.B:
                    return gamePadState.Buttons.B == ButtonState.Pressed;
                case GamePadButtonCode.X:
                    return gamePadState.Buttons.X == ButtonState.Pressed;
                case GamePadButtonCode.Y:
                    return gamePadState.Buttons.Y == ButtonState.Pressed;
                case GamePadButtonCode.LeftBumper:
                    return gamePadState.Buttons.LeftShoulder == ButtonState.Pressed;
                case GamePadButtonCode.RightBumper:
                    return gamePadState.Buttons.RightShoulder == ButtonState.Pressed;
                case GamePadButtonCode.LeftTrigger:
                    return gamePadState.Triggers.Left >= triggerPressedThreshold;
                case GamePadButtonCode.RightTrigger:
                    return gamePadState.Triggers.Right >= triggerPressedThreshold;
                case GamePadButtonCode.LeftStickButton:
                    return gamePadState.Buttons.LeftStick == ButtonState.Pressed;
                case GamePadButtonCode.RightStickButton:
                    return gamePadState.Buttons.RightStick == ButtonState.Pressed;
                case GamePadButtonCode.Back:
                    return gamePadState.Buttons.Back == ButtonState.Pressed;
                case GamePadButtonCode.Start:
                    return gamePadState.Buttons.Start == ButtonState.Pressed;
                case GamePadButtonCode.Left:
                    return gamePadState.DPad.Left == ButtonState.Pressed;
                case GamePadButtonCode.Right:
                    return gamePadState.DPad.Right == ButtonState.Pressed;
                case GamePadButtonCode.Up:
                    return gamePadState.DPad.Up == ButtonState.Pressed;
                case GamePadButtonCode.Down:
                    return gamePadState.DPad.Down == ButtonState.Pressed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttonCode), buttonCode, null);
            }
        }

        private void GetXBoxOneControllerInputs()
        {
            APressed = CheckIfXInputButtonActivated(GamePadButtonCode.A);
            BPressed = CheckIfXInputButtonActivated(GamePadButtonCode.B);
            XPressed = CheckIfXInputButtonActivated(GamePadButtonCode.X);
            YPressed = CheckIfXInputButtonActivated(GamePadButtonCode.Y);

            LeftBumperPressed = CheckIfXInputButtonActivated(GamePadButtonCode.LeftBumper);
            RightBumperPressed = CheckIfXInputButtonActivated(GamePadButtonCode.RightBumper);

            LeftTriggerPressed = CheckIfXInputButtonActivated(GamePadButtonCode.LeftTrigger);
            RightTriggerPressed = CheckIfXInputButtonActivated(GamePadButtonCode.RightTrigger);

            LeftStickButtonPressed = CheckIfXInputButtonActivated(GamePadButtonCode.LeftStickButton);
            RightStickButtonPressed = CheckIfXInputButtonActivated(GamePadButtonCode.RightStickButton);

            BackPressed = CheckIfXInputButtonActivated(GamePadButtonCode.Back);
            StartPressed = CheckIfXInputButtonActivated(GamePadButtonCode.Start);

            LeftPressed = CheckIfXInputButtonActivated(GamePadButtonCode.Left);
            RightPressed = CheckIfXInputButtonActivated(GamePadButtonCode.Right);
            UpPressed = CheckIfXInputButtonActivated(GamePadButtonCode.Up);
            DownPressed = CheckIfXInputButtonActivated(GamePadButtonCode.Down);
        }

        private Vector2 GetLeftJoystickDirection()
        {
            var direction = new Vector2(gamePadState.ThumbSticks.Left.X, gamePadState.ThumbSticks.Left.Y);
            return direction.SqrMagnitude() > squaredDirectionThreshold ? direction : Vector2.zero;
        }

        private Vector2 GetRightJoystickDirection()
        {
            var direction = new Vector2(gamePadState.ThumbSticks.Right.X, gamePadState.ThumbSticks.Right.Y);
            return direction.SqrMagnitude() > squaredDirectionThreshold ? direction : Vector2.zero;
        }

        private void UpdateJoystickDirections()
        {
            LeftJoysticDirection = GetLeftJoystickDirection();
            RightJoysticDirection = GetRightJoystickDirection();
        }

        private void Update()
        {
            UpdateState();

            GetXBoxOneControllerInputs();

            UpdateJoystickDirections();
        }

        public XboxOneControllerInput()
        {
            squaredDirectionThreshold = directionThreshold * directionThreshold;

            UpdateState();
        }
    }
}