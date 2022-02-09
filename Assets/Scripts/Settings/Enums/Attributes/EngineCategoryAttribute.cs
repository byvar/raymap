using System;

namespace Raymap {
    /// <summary>
    /// Attribute for the <see cref="EngineCategory"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EngineCategoryAttribute : Attribute {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EngineCategoryAttribute(Engine engine, string displayName) {
            Engine = engine;
            DisplayName = displayName;
        }

        /// <summary>
        /// The display name
        /// </summary>
        public string DisplayName { get; }
        
        /// <summary>
        /// The engine this category is for
        /// </summary>
        public Engine Engine { get; }
    }
}