using System;

namespace Raymap {
    /// <summary>
    /// Attribute for the <see cref="GameModeSelection"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class GameModeAttribute : Attribute {
        /// <summary>
        /// Default constructor
        /// </summary>
        public GameModeAttribute(EngineCategory category, Type managerType, string displayName) {
            Category = category;
            ManagerType = managerType;
            DisplayName = displayName;
        }

        /// <summary>
        /// The engine category
        /// </summary>
        public EngineCategory Category { get; }

        /// <summary>
        /// The display name
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The manager type
        /// </summary>
        public Type ManagerType { get; }
    }
}