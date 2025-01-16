using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Audio;

namespace GameConsole
{
    /// <summary>
    /// A simple in-game console system that handles user inputs, processes commands, 
    /// and displays results in a log. This class supports command parsing, execution,
    /// and the registration of custom commands that can be invoked via a text input.
    /// The default commands, such as 'help', 'move', and 'spawn', can be replaced
    /// with custom commands for specific game actions.
    ///
    /// The console operates by listening for user input from an <see cref="InputField"/>
    /// and logs messages in a <see cref="TMP_Text"/> UI element. It supports dynamic
    /// command execution with arguments and provides helpful logs for feedback.
    /// </summary>
    public class GameConsole : MonoBehaviour
    {
        // Command input field where users type commands.
        [SerializeField] private InputField commandInputField;

        // Text component where the console log messages are displayed.
        [SerializeField] private TMP_Text logText;

        [SerializeField] private GameObject canvas;

        [SerializeField] private InputActionReference openConsoleAction;

        [SerializeField] private Item[] gears;

        [SerializeField] private Item[] allItems;

        [SerializeField] private AudioMixer audioMixer;

        public UnityEvent consoleOpened;
        public UnityEvent consoleClosed;

        private bool open;

        // Dictionary to store available commands and their corresponding definitions.
        private Dictionary<string, CommandDefinition> commands;

        public Dictionary<string, CommandDefinition> Commands
            => new(commands);


        private void Awake()
        {
            // Initialize the commands dictionary with placeholder commands.
            commands = new Dictionary<string, CommandDefinition>
            {
                // Example 'help' command which lists all available commands
                {
                    "help",
                    new CommandDefinition(ShowHelp)
                },
                // Command to restart the current scene
                {
                    "restart_scene",
                    new CommandDefinition(RestartScene)
                },
                // Command to start the game with a specific scene ID
                {
                    "start_game",
                    new CommandDefinition(StartGame)
                },
                {
                    "home",
                    new CommandDefinition(Home)
                },
                {
                    "get_item",
                    new CommandDefinition(GetItem,
                        new List<CommandArgument>
                        {
                            new CommandArgument("item_name", typeof(string))
                        })
                },
                {
                    "clear_inventory",
                    new CommandDefinition(ClearInventory)
                },
                {
                    "snd_effects", new CommandDefinition(SoundEffectVolume,
                        new List<CommandArgument>
                        {
                            new CommandArgument("volume", typeof(float), 1.0f)
                        })
                },
                {
                    "snd_music", new CommandDefinition(MusicVolume,
                    new List<CommandArgument>
                    {
                        new CommandArgument("volume", typeof(float), 1.0f)
                    })
                },
                {
                    "snd_main", new CommandDefinition(MainVolume, new List<CommandArgument>
                    {
                        new CommandArgument("volume", typeof(float), 1.0f)
                    })
                },
            };

            // Check if references are properly assigned.
            if (commandInputField == null || logText == null)
            {
                Debug.LogError("GameConsole references are not set correctly.", this);
                return;
            }
            open = canvas.activeInHierarchy;
            Log("Console initialized. Type 'help' for a list of commands.");
        }

        private void OnEnable()
        {
            // Add listener for when a command is entered in the input field.
            commandInputField.onSubmit.AddListener(OnCommandEntered);

            openConsoleAction.action.performed += OnConsoleAction;
        }

        private void OnDisable()
        {
            // Remove the listener when the object is disabled.
            commandInputField.onSubmit.RemoveListener(OnCommandEntered);

            openConsoleAction.action.performed -= OnConsoleAction;
        }

        public void OnConsoleAction(InputAction.CallbackContext context)
        {
            open = !open;
            if (open)
            {
                canvas.SetActive(true);
                consoleOpened?.Invoke();
            }
            else
            {
                canvas.SetActive(false);
                consoleClosed?.Invoke();
            }
        }

        /// <summary>
        /// Called when a command is entered in the input field.
        /// </summary>
        /// <param name="input">The command entered by the user.</param>
        public void OnCommandEntered(string input)
        {
            if (string.IsNullOrWhiteSpace(input) ||
                string.IsNullOrEmpty(input)) return; // Ignore empty inputs.

            // Log the command entered by the user.
            Log($"> {input}");

            // Parse and execute the command.
            ParseCommand(input);
        }

        /// <summary>
        /// Parses the entered command and executes it.
        /// </summary>
        /// <param name="input">The command string to parse.</param>
        private void ParseCommand(string input)
        {
            string[] parts = input.Split(' ');  // Split the input into command and arguments.
            string commandName = parts[0].ToLower();  // Get the command name (first word).

            // Check if the command exists in the dictionary.
            if (!commands.TryGetValue(commandName, out CommandDefinition command))
            {
                Log($"Unknown command: {commandName}"); // Log if the command is not found.
                return;
            }

            string[] args = parts.Length > 1 ? parts[1..] : Array.Empty<string>(); // Extract arguments.

            try
            {
                // Parse the arguments for the command.
                object[] parsedArgs = command.ParseArguments(args);
                // Invoke the action associated with the command using the parsed arguments.
                command.Action.Invoke(parsedArgs);
            }
            catch (Exception ex)
            {
                Log("An error occurred.");
                Debug.LogError($"Error: {ex.Message}"); // Log errors during command execution.
            }
        }

        /// <summary>
        /// Displays the available commands and their usage.
        /// </summary>
        private void ShowHelp(params object[] args)
        {
            Log("");
            Log("<size=22>Available commands:</size>");
            Log("");
            foreach (var cmd in commands)
            {
                string commandText = $"{cmd.Key}";
                string usage = cmd.Value.GetUsage();
                if (!string.IsNullOrEmpty(usage) || !string.IsNullOrWhiteSpace(usage))
                {
                    commandText += $": {usage}";
                }
                commandText += ";";

                // Log each command's name and usage description.
                Log($"{commandText}");
            }
        }

        private void StartGame(params object[] args)
        {
            SceneManager.LoadScene("PrototypeScene");
        }

        private void RestartScene(params object[] args)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Home(params object[] args)
        {
            SceneManager.LoadScene("MainMenu");
        }

        private void GetItem(params object[] args)
        {
            if (SceneManager.GetActiveScene().name != "PrototypeScene")
            {
                Log("This command can only be used in game.");
                return;
            }

            if (args.Length == 0)
            {
                Log("Please provide an item name.");
                return;
            }
            if (args[0] is string itemName)
            {
                itemName = itemName.ToLower();
                PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();

                if (itemName == "gearall" || itemName == "gear*"
                    || itemName == "allgears" || itemName == "allgear"
                    || itemName == "*gear")
                {
                    foreach (Item gear in gears)
                    {
                        if (playerInventory != null) playerInventory.AddItemToInventory(gear);
                    }
                    Log("All gears added to inventory.");
                }
                else
                {
                    Item item = allItems.FirstOrDefault(g => g.Name.ToLower() == itemName.ToLower());
                    if (item != null)
                    {
                        if (playerInventory != null)
                        {
                            playerInventory.AddItemToInventory(item);
                            Log($"{item.Name} added to inventory.");
                        }
                    }
                    else
                    {
                        Log($"Item '{itemName}' not found.");
                    }
                }
            }
        }

        private void ClearInventory(params object[] args)
        {
            PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
            if (playerInventory != null)
            {
                playerInventory.RemoveAllItemsFromInventory();
            }
            Log("Inventory cleared.");
        }

        private void SoundEffectVolume(params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                Log("Please provide a volume value.");
                return;
            }

            float v = (float)args[0];

            if (v <= 0)
            {
                v = 1e-5f;
                Log($"Sound effects volume set to {0}");
            }
            else Log($"Sound effects volume set to {v}");

            audioMixer.SetFloat("effectsVol", Mathf.Log10(v) * 20);
        }

        private void MusicVolume(params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                Log("Please provide a volume value.");
                return;
            }

            float v = (float)args[0];

            if (v <= 0)
            {
                v = 1e-5f;
                Log($"Music volume set to {0}");
            }
            else Log($"Music volume set to {v}");

            audioMixer.SetFloat("effectsVol", Mathf.Log10(v) * 20);
        }

        private void MainVolume(params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                Log("Please provide a volume value.");
                return;
            }

            float v = (float)args[0];

            if (v <= 0)
            {
                v = 1e-5f;
                Log($"Main volume set to {0}");
            }
            else Log($"Main volume set to {v}");

            audioMixer.SetFloat("effectsVol", Mathf.Log10(v) * 20);
        }

        /// <summary>
        /// Logs messages to the console display.
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        private void Log(string message)
        {
            logText.text += message + "\n"; // Append the message to the log text.
        }
    }
}

