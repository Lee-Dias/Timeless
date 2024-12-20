using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.InteropServices;

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

        public UnityEvent consoleOpened;
        public UnityEvent consoleClosed;

        private bool open;

        // Dictionary to store available commands and their corresponding definitions.
        private Dictionary<string, CommandDefinition> commands;

        private void Awake()
        {
            // Initialize the commands dictionary with placeholder commands.
            commands = new Dictionary<string, CommandDefinition>
            {
                // Example 'help' command which lists all available commands
                {
                    "help",
                    new CommandDefinition(
                        action: args => ShowHelp() // Action to show help
                    )
                },
                // Command to restart the current scene
                {
                    "restart_scene",
                    new CommandDefinition(
                        action: args => RestartScene() // Action to restart the scene
                    )
                },
                // Command to start the game with a specific scene ID
                {
                    "start_game",
                    new CommandDefinition(
                        action: args => StartGame((int)args[0]), // Pass the scene ID to the action
                        arguments: new List<CommandArgument>
                        {
                            new CommandArgument("scene", typeof(int), 0) // Default scene ID is 0
                        }
                    )
                },
                {
                    "home",
                    new CommandDefinition(
                        action: args => Home()
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
            if (string.IsNullOrWhiteSpace(input)) return; // Ignore empty inputs.

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
                Log($"Error: {ex.Message}"); // Log errors during command execution.
            }
        }

        /// <summary>
        /// Logs messages to the console display.
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        private void Log(string message)
        {
            logText.text += message + "\n"; // Append the message to the log text.
        }

        /// <summary>
        /// Displays the available commands and their usage.
        /// </summary>
        private void ShowHelp(params object[] args)
        {
            Log("Available commands:");
            foreach (var cmd in commands)
            {
                // Log each command's name and usage description.
                Log($"{cmd.Key}: {cmd.Value.GetUsage()}");
            }
        }

        private void StartGame(params object[] args)
        {
            if ((int)args[0] == 0) SceneManager.LoadScene("FirstCutscene");
            else if ((int)args[0] == 1) SceneManager.LoadScene("PrototypeScene");
        }

        private void RestartScene(params object[] args)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Home(params object[] args)
        {
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Retrieves the list of available commands.
        /// </summary>
        public Dictionary<string, CommandDefinition> GetCommands()
        {
            return commands;
        }
    }
}

