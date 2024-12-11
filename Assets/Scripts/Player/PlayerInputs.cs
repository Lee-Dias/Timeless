using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages player input actions using Unity's new Input System. This class is a singleton
/// and is intended to be attached to a standalone GameObject in the scene. It will persist across
/// scenes using "Don't Destroy On Load".
/// </summary>
/// <remarks>
/// The class handles various types of input including movement, look, zoom, and actions like interact, grab, return, and rotate. 
/// It also tracks the current input device (keyboard/mouse, gamepad, or touch) and adjusts sensitivity for each device type.
/// 
/// <list type="bullet">
///     <item>
///         <description><strong>MoveInput</strong>: A vector representing the player's movement input (horizontal and vertical axes).</description>
///     </item>
///     <item>
///         <description><strong>LookInput</strong>: A vector representing the player's camera (look) movement (horizontal and vertical axes).</description>
///     </item>
///     <item>
///         <description><strong>ZoomInput</strong>: A vector representing the zoom input (used for adjusting the camera's zoom level).</description>
///     </item>
///     <item>
///         <description><strong>IsInteracting</strong>: A boolean indicating whether the player is currently interacting with an object or UI element.</description>
///     </item>
///     <item>
///         <description><strong>InteractButtonDown</strong>: A flag that is set when the interact button is pressed.</description>
///     </item>
///     <item>
///         <description><strong>ReturnButton</strong>: A flag that is set when the return button is pressed.</description>
///     </item>
///     <item>
///         <description><strong>GrabButtonDown</strong>: A flag that is set when the grab button is pressed.</description>
///     </item>
///     <item>
///         <description><strong>RotateButtonDown</strong>: A flag that is set when the rotate button is pressed.</description>
///     </item>
///     <item>
///         <description><strong>CurrentDeviceType</strong>: An enum that tracks the type of input device currently in use (Keyboard/Mouse, Gamepad, or Touch).</description>
///     </item>
/// </list>
/// </remarks>
public class PlayerInputs : Singleton<PlayerInputs>
{
    [Header("Sensitivities")]
    [Tooltip("Sensitivity for mouse input.")]
    [SerializeField] private float mouseSensitivity = .2f; // Mouse sensitivity for look input.

    [Tooltip("Sensitivity for gamepad input.")]
    [SerializeField] private float gamepadSensitivity = 75f; // Gamepad sensitivity for look input.

    private InputSystem_Actions inputActions; // Holds input actions from the Input System.

    /// <summary>
    /// Gets the current movement input vector (x, y). 
    /// Values represent the horizontal (x) and vertical (y) movement axis.
    /// </summary>
    public Vector2 MoveInput { get; private set; } // Stores the movement input.

    /// <summary>
    /// Gets the current look (camera) input vector (x, y). 
    /// The x-axis controls the horizontal look and the y-axis controls the vertical look.
    /// </summary>
    public Vector2 LookInput { get; private set; } // Stores the look (camera) input.

    /// <summary>
    /// Gets the current zoom input vector (x, y). 
    /// This is used to control the zoom level of the camera.
    /// </summary>
    public Vector2 ZoomInput { get; private set; } // Stores the zoom input.

    /// <summary>
    /// Indicates whether the player is currently interacting (e.g., pressing the interact button).
    /// </summary>
    public bool IsInteracting { get; private set; } // Indicates whether the player is interacting.

    /// <summary>
    /// Flag that is set when the interact button is pressed.
    /// </summary>
    public bool InteractButtonDown { get; private set; } // Flag set when the interact button is pressed.

    /// <summary>
    /// Flag that is set when the interact button is released.
    /// </summary>
    public bool InteractButtonUp { get; private set; } // Flag set when the interact button is released.

    /// <summary>
    /// Indicates whether the return button is pressed.
    /// </summary>
    public bool ReturnButton { get; private set; } // Indicates if the return button is pressed.

    /// <summary>
    /// Flag that is set when the return button is pressed.
    /// </summary>
    public bool ReturnButtonDown { get; private set; } // Flag set when the return button is pressed.

    /// <summary>
    /// Flag that is set when the return button is released.
    /// </summary>
    public bool ReturnButtonUp { get; private set; } // Flag set when the return button is released.

    /// <summary>
    /// Indicates whether the grab button is pressed.
    /// </summary>
    public bool GrabButton { get; private set; } // Indicates if the grab button is pressed.

    /// <summary>
    /// Flag that is set when the grab button is pressed.
    /// </summary>
    public bool GrabButtonDown { get; private set; } // Flag set when the grab button is pressed.

    /// <summary>
    /// Flag that is set when the grab button is released.
    /// </summary>
    public bool GrabButtonUp { get; private set; } // Flag set when the grab button is released.

    /// <summary>
    /// Indicates whether the rotate button is pressed.
    /// </summary>
    public bool RotateButton { get; private set; } // Indicates if the rotate button is pressed.

    /// <summary>
    /// Flag that is set when the rotate button is pressed.
    /// </summary>
    public bool RotateButtonDown { get; private set; } // Flag set when the rotate button is pressed.

    /// <summary>
    /// Flag that is set when the rotate button is released.
    /// </summary>
    public bool RotateButtonUp { get; private set; } // Flag set when the rotate button is released.

    /// <summary>
    /// Indicates whether the player is currently interacting (e.g., pressing the interact button).
    /// </summary>
    public bool IsInspecting { get; private set; } // Indicates whether the player is inspecting.

    /// <summary>
    /// Flag that is set when the interact button is pressed.
    /// </summary>
    public bool InspectButtonDown { get; private set; } // Flag set when the inspect button is pressed.

    /// <summary>
    /// Flag that is set when the interact button is released.
    /// </summary>
    public bool InspectButtonUp { get; private set; } // Flag set when the inspect button is released.

    /// <summary>
    /// Flag that is set when the interact button is pressed.
    /// </summary>
    public bool PauseButton { get; private set; } // Flag set when the inspect button is pressed.

    /// <summary>
    /// Flag that is set when the interact button is pressed.
    /// </summary>
    public bool PauseButtonDown { get; private set; } // Flag set when the inspect button is pressed.

    /// <summary>
    /// Flag that is set when the interact button is released.
    /// </summary>
    public bool PauseButtonUp { get; private set; } // Flag set when the inspect button is released.


    /// <summary>
    /// Enum representing the type of input device currently being used by the player.
    /// Can be <see cref="DeviceType.KeyboardMouse"/>, <see cref="DeviceType.Gamepad"/>, or <see cref="DeviceType.Touch"/>.
    /// </summary>
    public enum DeviceType { KeyboardMouse, Gamepad, Touch } // Enum to track current input device.

    /// <summary>
    /// The current device type the player is using for input (e.g., Keyboard/Mouse, Gamepad, or Touch).
    /// </summary>
    public DeviceType CurrentDeviceType { get; private set; } // Stores the current input device type.

    protected override void Awake()
    {
        base.Awake();
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
        }
    }

    /// <summary>
    /// Detects changes in input actions and updates the device type accordingly.
    /// </summary>
    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed && obj is InputAction action)
        {
            DetectCurrentInput(action.activeControl.device); // Detects the active input device.
        }
    }

    /// <summary>
    /// Determines the current input device type based on the active device.
    /// </summary>
    private void DetectCurrentInput(InputDevice device)
    {
        CurrentDeviceType = device switch
        {
            Gamepad => DeviceType.Gamepad, // Sets to Gamepad if the input is from a gamepad.
            Touchscreen => DeviceType.Touch, // Sets to Touch if the input is from a touchscreen.
            Keyboard or Mouse => DeviceType.KeyboardMouse, // Sets to Keyboard/Mouse if the input is from a keyboard or mouse.
            _ => CurrentDeviceType // Defaults to current device type if unknown.
        };
    }

    private void LateUpdate()
    {
        // Reset button states after each update.
        InteractButtonDown = false;
        InteractButtonUp = false;
        ReturnButtonDown = false;
        ReturnButtonUp = false;
        GrabButtonDown = false;
        GrabButtonUp = false;
        RotateButtonDown = false;
        RotateButtonUp = false;
        InspectButtonDown = false;
        InspectButtonUp = false;
        PauseButtonDown = false;
        PauseButtonUp = false;

    }

    /// <summary>
    /// Binds an action to the respective performed and canceled actions.
    /// </summary>
    private void BindAction(InputAction action, System.Action<InputAction.CallbackContext> onPerformed, System.Action<InputAction.CallbackContext> onCanceled)
    {
        action.performed += onPerformed; // Binds the performed action.
        action.canceled += onCanceled; // Binds the canceled action.
    }

    /// <summary>
    /// Unbinds an action from the performed and canceled actions.
    /// </summary>
    private void UnbindAction(InputAction action, System.Action<InputAction.CallbackContext> onPerformed, System.Action<InputAction.CallbackContext> onCanceled)
    {
        action.performed -= onPerformed; // Unbinds the performed action.
        action.canceled -= onCanceled; // Unbinds the canceled action.
    }

    private void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Enable(); // Enables the input actions.

            // Bind input actions to the respective methods.
            BindAction(inputActions.Player.Move, OnMovePerformed, OnMoveCanceled);
            BindAction(inputActions.Player.Look, OnLookPerformed, OnLookCanceled);
            BindAction(inputActions.Player.Interact, OnInteractPerformed, OnInteractCanceled);
            BindAction(inputActions.Player.Zoom, OnZoomPerformed, OnZoomCanceled);
            BindAction(inputActions.Player.Return, OnReturnPerformed, OnReturnCanceled);
            BindAction(inputActions.Player.Grab, OnGrabPerformed, OnGrabCanceled);
            BindAction(inputActions.Player.Rotate, OnRotatePerformed, OnRotateCanceled);
            BindAction(inputActions.Player.Inspect, OnInspectPerformed, OnInspectCanceled);
            BindAction(inputActions.Player.Pause, OnPausePerformed, OnPauseCanceled);


            InputSystem.onActionChange += OnActionChange; // Subscribes to input action changes.
        }
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Disable(); // Disable the input actions.

            // Unbind input actions.
            UnbindAction(inputActions.Player.Move, OnMovePerformed, OnMoveCanceled);
            UnbindAction(inputActions.Player.Look, OnLookPerformed, OnLookCanceled);
            UnbindAction(inputActions.Player.Interact, OnInteractPerformed, OnInteractCanceled);
            UnbindAction(inputActions.Player.Zoom, OnZoomPerformed, OnZoomCanceled);
            UnbindAction(inputActions.Player.Return, OnReturnPerformed, OnReturnCanceled);
            UnbindAction(inputActions.Player.Grab, OnGrabPerformed, OnGrabCanceled);
            UnbindAction(inputActions.Player.Rotate, OnRotatePerformed, OnRotateCanceled);
            UnbindAction(inputActions.Player.Inspect, OnInspectPerformed, OnInspectCanceled);
            UnbindAction(inputActions.Player.Pause, OnPausePerformed, OnPauseCanceled);
        }
        
        InputSystem.onActionChange -= OnActionChange; // Unsubscribe from input action changes.
    }


    // The following methods handle specific actions like move, look, interact, etc.

    private void OnInteractCanceled(InputAction.CallbackContext context) { InteractButtonUp = true; IsInteracting = false; }
    private void OnInteractPerformed(InputAction.CallbackContext context) { InteractButtonDown = true; IsInteracting = true; }
    private void OnMovePerformed(InputAction.CallbackContext context) { MoveInput = context.ReadValue<Vector2>(); }
    private void OnMoveCanceled(InputAction.CallbackContext context) { MoveInput = Vector2.zero; }
    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>() *
                                                                        (CurrentDeviceType == DeviceType.Gamepad ?
                                                                        gamepadSensitivity : mouseSensitivity);
    }
    private void OnLookCanceled(InputAction.CallbackContext context) { LookInput = Vector2.zero; }
    private void OnZoomPerformed(InputAction.CallbackContext context) { ZoomInput = context.ReadValue<Vector2>(); }
    private void OnZoomCanceled(InputAction.CallbackContext context) { ZoomInput = Vector2.zero; }
    private void OnReturnPerformed(InputAction.CallbackContext context) { ReturnButtonDown = true; ReturnButton = true; }
    private void OnReturnCanceled(InputAction.CallbackContext context) { ReturnButtonUp = true; ReturnButton = false; }
    private void OnGrabPerformed(InputAction.CallbackContext context) { GrabButtonDown = true; GrabButton = true; }
    private void OnGrabCanceled(InputAction.CallbackContext context) { GrabButtonUp = true; GrabButton = false; }
    private void OnRotatePerformed(InputAction.CallbackContext context) { RotateButtonDown = true; RotateButton = true; }
    private void OnRotateCanceled(InputAction.CallbackContext context) { RotateButtonUp = true; RotateButton = false; }
    private void OnInspectCanceled(InputAction.CallbackContext context) { InspectButtonUp = true; IsInspecting = false; }
    private void OnInspectPerformed(InputAction.CallbackContext context) { InspectButtonDown = true; IsInspecting = true; }
    private void OnPauseCanceled(InputAction.CallbackContext context) { PauseButtonUp = true; PauseButton = false; }
    private void OnPausePerformed(InputAction.CallbackContext context) { PauseButtonDown = true; PauseButton = true; }
}
