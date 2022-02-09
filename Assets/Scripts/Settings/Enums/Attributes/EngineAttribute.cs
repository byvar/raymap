using System;

namespace Raymap {
    /// <summary>
    /// Attribute for the <see cref="Engine"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EngineAttribute : Attribute {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EngineAttribute(string displayName) {
            DisplayName = displayName;
        }

        /// <summary>
        /// The display name
        /// </summary>
        public string DisplayName { get; }
    }
}