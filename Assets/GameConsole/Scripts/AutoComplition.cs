using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GameConsole
{
    /// <summary>
    /// Handles the auto-completion system for a command input field, providing suggestions
    /// based on commands available in the GameConsole.
    /// </summary>
    public class AutoCompletion : MonoBehaviour
    {
        // Prefab for creating suggestion UI elements.
        [SerializeField] private SuggestionUI suggestionItemPrefab;

        // Parent transform where the suggestion UI elements will be instantiated.
        [SerializeField] private Transform suggestionContainer;

        // The input field handler that triggers the auto-completion system.
        [SerializeField] private InputField inputFieldHandler;

        // List of currently active suggestion UI elements.
        private List<SuggestionUI> activeSuggestions = new List<SuggestionUI>();

        // Dictionary to store all available commands and their parameters.
        private Dictionary<string, string[]> allSuggestions; // Command + Parameters

        // Index of the currently selected suggestion in the list.
        private int currentIndex = -1; // Current highlighted suggestion

        // Reference to the GameConsole instance to access its commands.
        [SerializeField] private GameConsole gameConsole;

        /// <summary>
        /// Initializes the auto-completion system and populates the suggestions from the GameConsole.
        /// </summary>
        private void Start()
        {
            if (gameConsole == null)
            {
                Debug.LogError("GameConsole reference is not set.");
                return;
            }
            allSuggestions = new Dictionary<string, string[]>(); // Initialize the dictionary to store suggestions

            // Extract commands and parameters from GameConsole's commands
            PopulateSuggestionsFromCommands();
        }

        /// <summary>
        /// Subscribes to the input field's text change event when the component is enabled.
        /// </summary>
        private void OnEnable()
        {
            inputFieldHandler.onTextChanged.AddListener(OnTextChanged);
        }

        /// <summary>
        /// Unsubscribes from the input field's text change event when the component is disabled.
        /// </summary>
        private void OnDisable()
        {
            inputFieldHandler.onTextChanged.RemoveListener(OnTextChanged);
        }

        /// <summary>
        /// Populates the <see cref="allSuggestions"/> dictionary with commands and their parameters from the GameConsole.
        /// </summary>
        private void PopulateSuggestionsFromCommands()
        {
            allSuggestions.Clear(); // Clear any previous suggestions

            foreach (var command in gameConsole.Commands)
            {
                var commandName = command.Key; // The command name
                var parameters = command.Value.Arguments.Select(arg => arg.Name).ToArray(); // Get parameter names

                allSuggestions[commandName] = parameters; // Add command and its parameters to the dictionary
            }
        }

        /// <summary>
        /// Called whenever the text in the input field changes. Filters and updates suggestions accordingly.
        /// </summary>
        /// <param name="input">The current input text.</param>
        private void OnTextChanged(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                ClearSuggestions(); // Clear suggestions if input is empty
                return;
            }

            // Filter suggestions based on user input
            var matches = allSuggestions
                .Where(s => s.Key.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matches.Count == 0)
            {
                ClearSuggestions(); // Clear suggestions if no matches are found
                return;
            }

            // Populate suggestions
            ClearSuggestions();
            foreach (var match in matches)
            {
                var suggestionUI = Instantiate(suggestionItemPrefab, suggestionContainer); // Instantiate suggestion UI element

                // Construct suggestion text with parameters
                string suggestionText = match.Key;
                if (match.Value.Length > 0)
                {
                    suggestionText += " " + string.Join(" ", match.Value.Select(p => $"{{{p}}}")); // Add parameter placeholders
                }

                suggestionUI.suggestionText.text = suggestionText; // Set suggestion text
                activeSuggestions.Add(suggestionUI); // Add the suggestion to the active suggestions list
            }

            currentIndex = -1; // Reset selection
        }

        /// <summary>
        /// Updates the suggestion selection using arrow keys.
        /// </summary>
        private void Update()
        {
            if (activeSuggestions.Count == 0)
                return;
            if (Input.GetKeyDown(KeyCode.DownArrow) || currentIndex < 0)
            {
                NavigateSuggestions(1); // Navigate down in the suggestion list
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                NavigateSuggestions(-1); // Navigate up in the suggestion list
            }
        }

        /// <summary>
        /// Navigates through the suggestions based on the given direction.
        /// </summary>
        /// <param name="direction">The direction to navigate: 1 for down, -1 for up.</param>
        private void NavigateSuggestions(int direction)
        {
            if (currentIndex >= 0 && currentIndex < activeSuggestions.Count)
            {
                activeSuggestions[currentIndex].Deselect(); // Deselect current suggestion
            }

            currentIndex = (currentIndex + direction) % activeSuggestions.Count; // Update the index
            if (currentIndex < 0) currentIndex += activeSuggestions.Count; // Handle negative index overflow

            activeSuggestions[currentIndex].Select(); // Select the new suggestion
            inputFieldHandler.SetPreviewText(activeSuggestions[currentIndex].suggestionText.text.Split(' ')[0]); // Set the input preview text
        }

        /// <summary>
        /// Clears all the currently active suggestions.
        /// </summary>
        private void ClearSuggestions()
        {
            foreach (var suggestion in activeSuggestions)
            {
                Destroy(suggestion.gameObject); // Destroy the suggestion UI elements
            }
            activeSuggestions.Clear(); // Clear the active suggestions list
            currentIndex = -1; // Reset selection
        }
    }
}