using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Audio;
using System.Reflection;

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
        [SerializeField] private PlayerPrefs playerPrefs;

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

        public Dictionary<string, CommandDefinition> Commands => new(commands);


        private void Awake()
        {
            // Initialize the commands dictionary with placeholder commands.
            commands = new Dictionary<string, CommandDefinition>
            {
                // Example 'help' command which lists all available commands
                {
                    "help",
                    new CommandDefinition(
                        ShowHelp,
                        "Displays a list of all available commands.\n" +
                        "Provide a command name as an argument to see details " +
                        "about what it does.",
                        new[]
                        {
                            new CommandArgument("Command name", typeof(string), new string[]{"home"})
                        }
                    )

                },
                {
                    "restart_scene",
                    new CommandDefinition(RestartScene, "Restart current scene.")
                },
                {
                    "start_game",
                    new CommandDefinition(StartGame, "Start the game.")
                },
                {
                    "home",
                    new CommandDefinition(Home, "Go to Main Menu.")
                },
                {
                    "get_item",
                    new CommandDefinition(GetItem,
                        help: "Gives player an item. Item list:\n"+
                            "Berries;\n" +
                            "Bone;\n" +
                            "CoinLady;\n" +
                            "CoinSoldier;\n" +
                            "CoinWolf;\n" +
                            "Cristal;\n" +
                            "GearEgypt;\n" +
                            "GearMedieval;\n" +
                            "GearPreHistoric;\n" +
                            "GearWildWest;\n" +
                            "Gear* - Gives you all gears;\n" +
                            "Shield;\n" +
                            "StoneAx;\n" +
                            "StoneBone;\n" +
                            "StoneLeaf;\n" +
                            "StonePyramid;\n" +
                            "StoneSun;\n" +
                            "StoneWater;\n" +
                            "Stone* - Gives you all stones;\n" +
                            "VenusFigurine;\n",
                        arguments:
                        new CommandArgument[]
                        {
                            new("item_name", typeof(string))
                        })
                },
                {
                    "clear_inventory",
                    new CommandDefinition(ClearInventory, "Clear player inventory.")
                },
                {
                    "snd_effects", new CommandDefinition(SoundEffectVolume,
                        help: "Change sound effects volume.\n" +
                            " 0 = mute, 1 = default volume.\n" +
                            " To make it lower then default use decimals."+
                            " To make it higher then default volume, give it a higher number.",
                        arguments:
                        new CommandArgument[]
                        {
                            new("volume", typeof(float), defaultValue: 1.0f)
                        })
                },
                {
                    "snd_music", new CommandDefinition(MusicVolume,
                        help: "Change music volume.\n" +
                            " 0 = mute, 1 = default volume.\n" +
                            " To make it lower then default use decimals."+
                            " To make it higher then default volume, give it a higher number.",
                        arguments:
                        new CommandArgument[]
                        {
                            new("volume", typeof(float), defaultValue: 1.0f)
                        })
                },
                {
                    "snd_main", new CommandDefinition(MainVolume,
                        help: "Change main sound volume. This includes all sounds in the game.\n" +
                            " 0 = mute, 1 = default volume.\n" +
                            " To make it lower then default use decimals."+
                            " To make it higher then default volume, give it a higher number.",
                        arguments:
                        new CommandArgument[]
                        {
                            new("volume", typeof(float), defaultValue: 1.0f)
                        })
                },
                {
                    "sense", new CommandDefinition(ChangeSense,
                        help: "Change mouse sensitivity.",
                        arguments: new CommandArgument[]
                        {
                            new("value", typeof(float), defaultValue: 1f)
                        })
                },
                {
                    "invert_mouse_y", new CommandDefinition(ChangeMuseInvertY,
                        help: "Iverts mouse y",
                        arguments: new CommandArgument[]
                        {
                            new ("value", typeof(bool))
                        }
                    )
                },
                {
                    "invert_zoom", new CommandDefinition(ChangeZoomInvert,
                        help: "Inverts zoom",
                        arguments: new CommandArgument[]
                        {
                            new ("value", typeof(bool))
                        }
                    )
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
        private void ParseCommand(string input) //todo: preview command arguments too.
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
            if (args != null && args.Length > 0 && args[0] is string commandStr)
            {
                if (commands.TryGetValue(commandStr, out CommandDefinition value))
                {
                    Log(value.Help, 6);
                }
                else ShowHelp(null);
            }
            else
            {
                Log("<size=22>Available commands:</size>", 8, 6);

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

                if (string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName))
                {
                    Log("Please provide an item name.");
                    return;
                }

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

        /// <summary>
        /// <br>Adjusts the sound effects volume in the audio mixer.</br>
        /// <br>Converts a linear volume value (0.0 to 1.0) into a decibel (dB) scale to match Unity's audio system.</br>
        /// </summary>
        /// <remarks>
        /// <br>- Clamps very low values (≤ 1e-5) to prevent errors from logarithm calculations.</br>
        /// <br>- Unity audio mixer interprets 0 dB as the original volume and clamps values near 0 to -80 dB (mute).</br>
        /// </remarks>
        /// <param name="args">
        /// <br>A single parameter is expected:</br>
        /// <br>- <c>args[0]</c>: A float between, being 0 mute.</br>
        /// </param>
        private void SoundEffectVolume(params object[] args)
        {
            // Check if arguments are provided
            if (args == null || args.Length == 0)
            {
                Log("Please provide a volume value.");
                return;
            }

            // Retrieve the first argument and cast it to a float
            float v = (float)args[0];

            playerPrefs.effectsVolume = v;

            // Ensure the volume is not too small to avoid invalid calculations
            // 1e-5f is used as a practical lower bound to prevent issues like taking the logarithm of zero.
            if (v <= 1e-5f)
            {
                v = 1e-5f; // Clamp to the lower bound
                Log($"Sound effects volume set to {0}"); // Log that the volume is effectively muted
            }
            else
            {
                // Log the actual volume level
                Log($"Sound effects volume set to {v}");
            }

            // Convert the linear volume (0.0 to 1.0) to a decibel scale using a logarithmic function
            // Unity’s audio mixer expects decibel values. `20 * Mathf.Log10(v)` converts:
            // - Linear input of 1.0 to 0 dB (no attenuation).
            // - Linear input < 1.0 to negative decibels (attenuated volume).
            // - Values near 0 are clamped to approximately -80 dB.
            audioMixer.SetFloat("effectsVol", Mathf.Log10(v) * 20);
        }

        /// <summary>
        /// <br>Adjusts the sound effects volume in the audio mixer.</br>
        /// <br>Converts a linear volume value (0.0 to 1.0) into a decibel (dB) scale to match Unity's audio system.</br>
        /// </summary>
        /// <remarks>
        /// <br>- Clamps very low values (≤ 1e-5) to prevent errors from logarithm calculations.</br>
        /// <br>- Unity audio mixer interprets 0 dB as the original volume and clamps values near 0 to -80 dB (mute).</br>
        /// </remarks>
        /// <param name="args">
        /// <br>A single parameter is expected:</br>
        /// <br>- <c>args[0]</c>: A float between, being 0 mute.</br>
        /// </param>
        private void MusicVolume(params object[] args)
        {
            // Check if arguments are provided
            if (args == null || args.Length == 0)
            {
                Log("Please provide a volume value.");
                return;
            }

            // Retrieve the first argument and cast it to a float
            float v = (float)args[0];

            playerPrefs.musicVolume = v;

            // Ensure the volume is not too small to avoid invalid calculations
            // 1e-5f is used as a practical lower bound to prevent issues like taking the logarithm of zero.
            if (v <= 1e-5f)
            {
                v = 1e-5f; // Clamp to the lower bound
                Log($"Music volume set to {0}"); // Log that the volume is effectively muted
            }
            else
            {
                // Log the actual volume level
                Log($"Music volume set to {v}");
            }

            // Convert the linear volume (0.0 to 1.0) to a decibel scale using a logarithmic function
            // Unity’s audio mixer expects decibel values. `20 * Mathf.Log10(v)` converts:
            // - Linear input of 1.0 to 0 dB (no attenuation).
            // - Linear input < 1.0 to negative decibels (attenuated volume).
            // - Values near 0 are clamped to approximately -80 dB.

            audioMixer.SetFloat("musicVol", Mathf.Log10(v) * 20);
        }

        /// <summary>
        /// <br>Adjusts the sound effects volume in the audio mixer.</br>
        /// <br>Converts a linear volume value (0.0 to 1.0) into a decibel (dB) scale to match Unity's audio system.</br>
        /// </summary>
        /// <remarks>
        /// <br>- Clamps very low values (≤ 1e-5) to prevent errors from logarithm calculations.</br>
        /// <br>- Unity audio mixer interprets 0 dB as the original volume and clamps values near 0 to -80 dB (mute).</br>
        /// </remarks>
        /// <param name="args">
        /// <br>A single parameter is expected:</br>
        /// <br>- <c>args[0]</c>: A float between, being 0 mute.</br>
        /// </param>
        private void MainVolume(params object[] args)
        {
            // Check if arguments are provided
            if (args == null || args.Length == 0)
            {
                Log("Please provide a volume value.");
                return;
            }

            // Retrieve the first argument and cast it to a float
            float v = (float)args[0];

            playerPrefs.mainVolume = v;

            // Ensure the volume is not too small to avoid invalid calculations
            // 1e-5f is used as a practical lower bound to prevent issues like taking the logarithm of zero.
            if (v <= 1e-5f)
            {
                v = 1e-5f; // Clamp to the lower bound
                Log($"Main volume set to {0}"); // Log that the volume is effectively muted
            }
            else
            {
                // Log the actual volume level
                Log($"Main volume set to {v}");
            }

            // Convert the linear volume (0.0 to 1.0) to a decibel scale using a logarithmic function
            // Unity’s audio mixer expects decibel values. `20 * Mathf.Log10(v)` converts:
            // - Linear input of 1.0 to 0 dB (no attenuation).
            // - Linear input < 1.0 to negative decibels (attenuated volume).
            // - Values near 0 are clamped to approximately -80 dB.
            audioMixer.SetFloat("mainVol", Mathf.Log10(v) * 20);
        }

        private void ChangeSense(params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                Log("Please provide a sensitivity value.");
                return;
            }

            if (args[0] is float senseValue)
            {
                playerPrefs.sense = senseValue;
                Log($"Mouse sensitivity set to {senseValue}");
            }
            else
            {
                Log("Invalid value. Expected a number.");
            }
        }

        private void ChangeMuseInvertY(params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (args[0] is bool boolValue)
                {
                    playerPrefs.invertMouseY = boolValue;
                    Log($"Invert Mouse Y set to {playerPrefs.invertMouseY}");
                }
                else if (args[0] == null)
                {
                    playerPrefs.invertMouseY = !playerPrefs.invertMouseY;
                    Log($"Invert Mouse Y set to {playerPrefs.invertMouseY}");
                }
                else
                {
                    Log("Invalid value. Expected: True, False, 0, 1");
                }
            }
            else
            {
                playerPrefs.invertMouseY = !playerPrefs.invertMouseY;
                Log($"Invert Mouse Y set to {playerPrefs.invertMouseY}");
            }
        }

        private void ChangeZoomInvert(params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (args[0] is bool boolValue)
                {
                    playerPrefs.invertZoom = boolValue;
                    Log($"Invert Mouse Y set to {playerPrefs.invertZoom}");
                }
                else if (args[0] == null)
                {
                    playerPrefs.invertZoom = !playerPrefs.invertZoom;
                    Log($"Invert Zoom set to {playerPrefs.invertZoom}");
                }
                else
                {
                    Log("Invalid value. Expected: True, False, 0, 1");
                }
            }
            else
            {
                playerPrefs.invertZoom = !playerPrefs.invertZoom;
                Log($"Invert Zoom set to {playerPrefs.invertZoom}");
            }
        }

        /// <summary>
        /// Logs a message to the console with optional spacing before and after the message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="beforeSpace">The size of the space before the message.</param>
        /// <param name="afterSpace">The size of the space after the message.</param>
        private void Log(string message, float beforeSpace = 4, float afterSpace = 0)
        {
            if (beforeSpace > 0)
                logText.text += $"<size={beforeSpace}> </size>\n";

            logText.text += $"{message}\n";

            if (afterSpace > 0)
                logText.text += $"<size={afterSpace}> </size>\n";
        }
    }
}

