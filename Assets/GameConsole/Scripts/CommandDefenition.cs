using System;
using System.Collections.Generic;

namespace GameConsole
{
    /// <summary>
    /// Represents a command within the console, consisting of an action (method) to execute 
    /// and a list of arguments that the command requires. This class handles parsing the 
    /// user's input into the appropriate types, validates arguments, and executes the command's action.
    /// </summary>
    public class CommandDefinition
    {
        /// <summary>
        /// The action (method) that will be invoked when this command is executed.
        /// </summary>
        public Action<object[]> Action { get; }

        /// <summary>
        /// The list of arguments that this command expects to receive.
        /// </summary>
        public List<CommandArgument> Arguments { get; }

        /// <summary>
        /// Constructor that initializes the command with its action and arguments.
        /// </summary>
        /// <param name="action">The action (method) that will be invoked for this command.</param>
        /// <param name="arguments">A list of arguments that the command expects.</param>
        public CommandDefinition(Action<object[]> action, List<CommandArgument> arguments = null)
        {
            Action = action;  // Assign the action to be performed when the command is executed.
            
            Arguments = new List<CommandArgument>();
            if (arguments != null) Arguments = arguments;  // Assign the list of arguments required for this command.
        }

        /// <summary>
        /// Parses the input arguments provided by the user and converts them into the expected types 
        /// based on the argument definitions.
        /// </summary>
        /// <param name="inputArgs">The raw input arguments as strings from the user.</param>
        /// <returns>An array of parsed arguments in the correct data types.</returns>
        public object[] ParseArguments(string[] inputArgs)
        {
            // Create an array to hold the parsed argument values.
            object[] parsedArgs = new object[Arguments.Count];

            // Iterate over each defined argument.
            for (int i = 0; i < Arguments.Count; i++)
            {
                // If there are enough input arguments, parse and convert the input argument.
                if (i < inputArgs.Length)
                {
                    parsedArgs[i] = ConvertArgument(inputArgs[i], Arguments[i]);
                }
                // If the argument is missing, check if it has a default value.
                else if (Arguments[i].HasDefaultValue)
                {
                    parsedArgs[i] = Arguments[i].DefaultValue;
                }
                // If the argument is missing and has no default value, throw an exception.
                else
                {
                    throw new ArgumentException($"Missing required argument: {Arguments[i].Name}");
                }
            }

            return parsedArgs;  // Return the array of parsed arguments.
        }

        /// <summary>
        /// Converts the string input argument to the correct type defined for the argument.
        /// </summary>
        /// <param name="input">The raw string argument to be converted.</param>
        /// <param name="argument">The argument definition which contains the expected type.</param>
        /// <returns>The input value converted to the appropriate type.</returns>
        private object ConvertArgument(string input, CommandArgument argument)
        {
            try
            {
                // Attempt to convert the input string to the expected type.
                if (argument.Type == typeof(int))
                    return int.Parse(input);
                if (argument.Type == typeof(float))
                    return float.Parse(input);
                if (argument.Type == typeof(bool))
                    return bool.Parse(input);

                return input;  // If no conversion is required, return the input as a string.
            }
            catch
            {
                // If conversion fails, throw an exception indicating the issue.
                throw new ArgumentException($"Invalid value for {argument.Name}: expected {argument.Type.Name}");
            }
        }

        /// <summary>
        /// Returns a string representation of the usage for this command, including its arguments.
        /// Arguments with default values are enclosed in square brackets, while required arguments are enclosed in angle brackets.
        /// </summary>
        /// <returns>A string showing the expected usage of the command.</returns>
        public string GetUsage()
        {
            // Build the usage string, combining all arguments, with optional/default ones in square brackets.
            string usage = string.Join(" ", Arguments.ConvertAll(arg =>
                arg.HasDefaultValue ? $"[{arg.Name}]" : $"<{arg.Name}>"));
            return usage;  // Return the constructed usage string.
        }
    }
}
