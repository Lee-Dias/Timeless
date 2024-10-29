using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages player input actions using the new Input System. This class is a singleton
/// and must be placed on a standalone GameObject in the scene. 
/// It will be added to "Don't Destroy On Load" to persist across scenes.
/// </summary>
public class PlayerInputs : Singleton<PlayerInputs>
{

    [Header("Sensitivities")]

    [SerializeField] private float mouseSensitivity = .2f;
    [SerializeField] private float gamepadSensitivity = 75f;

    private InputSystem_Actions inputActions;

    /// <summary>
    /// Gets the player's current movement input as a 2D vector.
    /// </summary>
    public Vector2 MoveInput { get; private set; }

    /// <summary>
    /// Gets the player's current look input as a 2D vector.
    /// </summary>
    public Vector2 LookInput { get; private set; }

    /// <summary>
    /// Indicates whether the interact button is currently held down.
    /// </summary>
    public bool IsInteracting { get; private set; }

    /// <summary>
    /// True during the frame when the interact button is pressed down.
    /// </summary>
    public bool InteractButtonDown { get; private set; }

    /// <summary>
    /// True during the frame when the interact button is released.
    /// </summary>
    public bool InteractButtonUp { get; private set; }

    /// <summary>
    /// Enum for the types of input devices supported: Keyboard/Mouse, Gamepad, or Touch.
    /// </summary>
    public enum DeviceType
    {
        KeyboardMouse,
        Gamepad,
        Touch
    }

    /// <summary>
    /// The current input type being used by the player.
    /// </summary>
    public DeviceType currentDeviceType { get; private set; }


    /// <summary>
    /// Initializes the singleton instance and configures input actions.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        inputActions = new InputSystem_Actions();
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        InteractButtonUp = true;
        IsInteracting = false;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        IsInteracting = true;
        InteractButtonDown = true;
    }


    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }


    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }


    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();

        if (currentDeviceType == DeviceType.Gamepad) LookInput *= gamepadSensitivity;
        else LookInput *= mouseSensitivity;
    }


    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        LookInput = Vector2.zero;
    }

    /// <summary>
    /// Called when an input action changes, used to detect the current input type.
    /// </summary>
    /// <param name="obj">The input action or binding change.</param>
    /// <param name="change">The type of action change.</param>
    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            InputAction action = obj as InputAction;
            if (action != null)
            {
                DetectCurrentInput(action.activeControl.device);
            }
        }
    }

    /// <summary>
    /// Detects and sets the current input device type.
    /// </summary>
    /// <param name="device">The active input device.</param>
    private void DetectCurrentInput(InputDevice device)
    {
        if (device is Gamepad)
        {
            currentDeviceType = DeviceType.Gamepad;
        }
        else if (device is Touchscreen)
        {
            currentDeviceType = DeviceType.Touch;
        }
        else if (device is Keyboard || device is Mouse)
        {
            currentDeviceType = DeviceType.KeyboardMouse;
        }

        Debug.Log(currentDeviceType);
    }

    /// <summary>
    /// Resets <see cref="InteractButtonDown"/> and <see cref="InteractButtonUp"/> flags at the end of each frame.
    /// </summary>
    private void LateUpdate()
    {
        // it's stupid, I know, but it's the best option
        InteractButtonDown = false;
        InteractButtonUp = false;
    }

    /// <summary>
    /// Enables input actions and registers callback methods for player actions.
    /// </summary>
    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Player.Look.performed += OnLookPerformed;
        inputActions.Player.Look.canceled += OnLookCanceled;

        inputActions.Player.Interact.performed += OnInteractPerformed;
        inputActions.Player.Interact.canceled += OnInteractCanceled;

        InputSystem.onActionChange += OnActionChange;
    }

    /// <summary>
    /// Disables input actions and unregisters callback methods for player actions.
    /// </summary>
    private void OnDisable()
    {
        inputActions.Player.Disable();

        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Player.Look.performed -= OnLookPerformed;
        inputActions.Player.Look.canceled -= OnLookCanceled;

        inputActions.Player.Interact.performed -= OnInteractPerformed;
        inputActions.Player.Interact.canceled -= OnInteractCanceled;

        InputSystem.onActionChange -= OnActionChange;
    }
}
