using System;

namespace GameConsole
{
    /// <summary>
    /// Represents an argument for a command, including its name, type, and optional default value.
    /// This class is used to define the parameters that a command expects when it is invoked.
    /// </summary>
    public class CommandArgument
    {
        /// <summary>
        /// The name of the argument. This is how the argument is referred to in the command syntax.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of the argument. This defines what type of data (e.g., string, int, float) the argument should be.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The default value for the argument, if provided. This is used when the user does not provide a value for this argument.
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// A property that checks if the argument has a default value set.
        /// </summary>
        public bool HasDefaultValue => DefaultValue != null;

        private string[] argumentExemples;
        public string[] ArgumentExemples => (string[])argumentExemples.Clone();

        /// <summary>
        /// Constructor to initialize a new command argument with a name, type, and an optional default value.
        /// </summary>
        /// <param name="name">The name of the argument (e.g., "direction", "speed").</param>
        /// <param name="type">The type of the argument (e.g., typeof(int), typeof(string)).</param>
        /// <param name="defaultValue">An optional default value for the argument (default is null).</param>
        public CommandArgument(string name, Type type, string[] argumentExemples = null, object defaultValue = null)
        {
            Name = name;  // Assign the name of the argument.
            Type = type;  // Assign the type of the argument.
            this.argumentExemples = argumentExemples;
            DefaultValue = defaultValue;  // Assign the default value (if any).
        }
    }
}