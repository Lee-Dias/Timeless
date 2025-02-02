using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
    // Reference to player input handling.
    private PlayerInputs playerInputs;

    public bool IsPaused { get; private set; } = false;

    [SerializeField]
    private GameObject PauseMenu;

    [SerializeField]
    private GameObject InventorySlots;

    [SerializeField]
    private GameObject Crosshair;

    [SerializeField]
    private InspectionsHandler inspectionsHandler;


    private bool canPause = true;

    private void Start()
    {
        //Ensures the game will start moving
        Time.timeScale = 1;

        // Try to find the PlayerInputs object in the scene.
        playerInputs = FindFirstObjectByType<PlayerInputs>();

        // If PlayerInputs is missing, instantiate it and log a warning.
        if (playerInputs == null)
        {
            Debug.LogWarning($"{nameof(InspectionsHandler)} needs a {nameof(PlayerInputs)} object in the scene.");
        }
    }
    private void Update()
    {
        if(inspectionsHandler.inspecting) return;
        if (!canPause) return;
        if (playerInputs.PauseButtonDown)
        {
            ButtonPressed();
        }
    }
    public void ButtonPressed()
    {
        PauseSystem();
    }

    private void PauseSystem()
    {
        if (!IsPaused)
        {
            Crosshair.SetActive(false);
            PauseMenu.SetActive(true);
            InventorySlots.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            IsPaused = !IsPaused;
        }
        else
        {
            if(PauseMenu.activeSelf){
                Crosshair.SetActive(true);
                InventorySlots.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                PauseMenu.SetActive(false);
                Time.timeScale = 1; 
                IsPaused = !IsPaused;
            }

        }
  
    }

    public void SetCanPause(bool value)
    {
        canPause = value;
    }
}
