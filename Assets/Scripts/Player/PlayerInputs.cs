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

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public Vector2 ZoomInput { get; private set; }

    public bool IsInteracting { get; private set; }
    public bool InteractButtonDown { get; private set; }
    public bool InteractButtonUp { get; private set; }

    public bool ReturnButton { get; private set; }
    public bool ReturnButtonDown { get; private set; }
    public bool ReturnButtonUp { get; private set; }

    public bool GrabButton { get; private set; }
    public bool GrabButtonDown { get; private set; }
    public bool GrabButtonUp { get; private set; }

    public bool RotateButton { get; private set; }
    public bool RotateButtonDown { get; private set; }
    public bool RotateButtonUp { get; private set; }

    public enum DeviceType { KeyboardMouse, Gamepad, Touch }
    public DeviceType currentDeviceType { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        inputActions = new InputSystem_Actions();
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed && obj is InputAction action)
        {
            DetectCurrentInput(action.activeControl.device);
        }
    }

    private void DetectCurrentInput(InputDevice device)
    {
        currentDeviceType = device switch
        {
            Gamepad => DeviceType.Gamepad,
            Touchscreen => DeviceType.Touch,
            Keyboard or Mouse => DeviceType.KeyboardMouse,
            _ => currentDeviceType
        };
    }

    private void LateUpdate()
    {
        InteractButtonDown = false;
        InteractButtonUp = false;
        ReturnButtonDown = false;
        ReturnButtonUp = false;
        GrabButtonDown = false;
        GrabButtonUp = false;
        RotateButtonDown = false;
        RotateButtonUp = false;
    }

    private void BindAction(InputAction action, System.Action<InputAction.CallbackContext> onPerformed, System.Action<InputAction.CallbackContext> onCanceled)
    {
        action.performed += onPerformed;
        action.canceled += onCanceled;
    }

    private void UnbindAction(InputAction action, System.Action<InputAction.CallbackContext> onPerformed, System.Action<InputAction.CallbackContext> onCanceled)
    {
        action.performed -= onPerformed;
        action.canceled -= onCanceled;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        BindAction(inputActions.Player.Move, OnMovePerformed, OnMoveCanceled);
        BindAction(inputActions.Player.Look, OnLookPerformed, OnLookCanceled);
        BindAction(inputActions.Player.Interact, OnInteractPerformed, OnInteractCanceled);
        BindAction(inputActions.Player.Zoom, OnZoomPerformed, OnZoomCanceled);
        BindAction(inputActions.Player.Return, OnReturnPerformed, OnReturnCanceled);
        BindAction(inputActions.Player.Grab, OnGrabPerformed, OnGrabCanceled);
        BindAction(inputActions.Player.Rotate, OnRotatePerformed, OnRotateCanceled);

        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();

        UnbindAction(inputActions.Player.Move, OnMovePerformed, OnMoveCanceled);
        UnbindAction(inputActions.Player.Look, OnLookPerformed, OnLookCanceled);
        UnbindAction(inputActions.Player.Interact, OnInteractPerformed, OnInteractCanceled);
        UnbindAction(inputActions.Player.Zoom, OnZoomPerformed, OnZoomCanceled);
        UnbindAction(inputActions.Player.Return, OnReturnPerformed, OnReturnCanceled);
        UnbindAction(inputActions.Player.Grab, OnGrabPerformed, OnGrabCanceled);
        UnbindAction(inputActions.Player.Rotate, OnRotatePerformed, OnRotateCanceled);

        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnInteractCanceled(InputAction.CallbackContext context) { InteractButtonUp = true; IsInteracting = false; }
    private void OnInteractPerformed(InputAction.CallbackContext context) { InteractButtonDown = true; IsInteracting = true; }
    private void OnMovePerformed(InputAction.CallbackContext context) { MoveInput = context.ReadValue<Vector2>(); }
    private void OnMoveCanceled(InputAction.CallbackContext context) { MoveInput = Vector2.zero; }
    private void OnLookPerformed(InputAction.CallbackContext context) { LookInput = context.ReadValue<Vector2>() * (currentDeviceType == DeviceType.Gamepad ? gamepadSensitivity : mouseSensitivity); }
    private void OnLookCanceled(InputAction.CallbackContext context) { LookInput = Vector2.zero; }
    private void OnZoomPerformed(InputAction.CallbackContext context) { ZoomInput = context.ReadValue<Vector2>(); }
    private void OnZoomCanceled(InputAction.CallbackContext context) { ZoomInput = Vector2.zero; }
    private void OnReturnPerformed(InputAction.CallbackContext context) { ReturnButtonDown = true; ReturnButton = true; }
    private void OnReturnCanceled(InputAction.CallbackContext context) { ReturnButtonUp = true; ReturnButton = false; }
    private void OnGrabPerformed(InputAction.CallbackContext context) { GrabButtonDown = true; GrabButton = true; }
    private void OnGrabCanceled(InputAction.CallbackContext context) { GrabButtonUp = true; GrabButton = false; }
    private void OnRotatePerformed(InputAction.CallbackContext context) { RotateButtonDown = true; RotateButton = true; }
    private void OnRotateCanceled(InputAction.CallbackContext context) { RotateButtonUp = true; RotateButton = false; }
}
