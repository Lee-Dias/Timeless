using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
        // Reference to player input handling.
        private PlayerInputs playerInputs; 


        public bool IsPaused{get ; private set ; } = false;

        [SerializeField]
        private GameObject PauseMenu;

        [SerializeField]
        private GameObject Crosshair;

        private void Start(){
            //Ensures the game will start moving
            Time.timeScale = 1;
            // Try to find the PlayerInputs object in the scene.
            playerInputs = FindFirstObjectByType<PlayerInputs>();

            // If PlayerInputs is missing, instantiate it and log a warning.
            if (playerInputs == null)
            {
                Debug.LogWarning($"{nameof(InspectionsHandler)} needs a {nameof(PlayerInputs)} object in the scene. Instantiating a new one.");
                playerInputs = Instantiate(new PlayerInputs());
            }
        }
        private void Update(){
            if (playerInputs.PauseButtonDown){
                IsPaused = !IsPaused;
                PauseSystem();
            }
        }

        private void PauseSystem(){
            if (IsPaused){
                Crosshair.SetActive(false);
                PauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
            }else{
                Crosshair.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                PauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
        }
}