using System;
using Cysharp.Threading.Tasks;

namespace Raymap {
    /// <summary>
    /// A game action
    /// </summary>
    public class GameAction {
        /// <summary>
        /// Constructor for an asynchronous action
        /// </summary>
        /// <param name="displayName">The action display name</param>
        /// <param name="requiresInputDir">Indicates if the action requires an input directory</param>
        /// <param name="requiresOutputDir">Indicates if the action requires an output directory</param>
        /// <param name="gameActionFunc">The game action function</param>
        public GameAction(string displayName, bool requiresInputDir, bool requiresOutputDir, Func<string, string, UniTask> gameActionFunc) {
            DisplayName = displayName;
            RequiresInputDir = requiresInputDir;
            RequiresOutputDir = requiresOutputDir;
            GameActionFunc = gameActionFunc;
        }

        /// <summary>
        /// Constructor for a synchronous action
        /// </summary>
        /// <param name="displayName">The action display name</param>
        /// <param name="requiresInputDir">Indicates if the action requires an input directory</param>
        /// <param name="requiresOutputDir">Indicates if the action requires an output directory</param>
        /// <param name="gameActionFunc">The game action function</param>
        public GameAction(string displayName, bool requiresInputDir, bool requiresOutputDir, Action<string, string> gameActionFunc) {
            DisplayName = displayName;
            RequiresInputDir = requiresInputDir;
            RequiresOutputDir = requiresOutputDir;
            GameActionFunc = (input, output) => {
                gameActionFunc(input, output);
                return UniTask.CompletedTask;
            };
        }

        /// <summary>
        /// The action display name
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Indicates if the action requires an input directory
        /// </summary>
        public bool RequiresInputDir { get; }

        /// <summary>
        /// Indicates if the action requires an output directory
        /// </summary>
        public bool RequiresOutputDir { get; }

        /// <summary>
        /// The game action function
        /// </summary>
        public Func<string, string, UniTask> GameActionFunc { get; }
    }
}