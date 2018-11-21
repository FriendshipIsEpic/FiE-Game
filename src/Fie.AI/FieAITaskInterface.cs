using System;
using System.Collections.Generic;

namespace Fie.AI {
    /// <summary>
    /// An AI task that a character can perform. Called to handle movement/targetting/etc.
    /// </summary>
    public interface FieAITaskInterface {
        /// <summary>
        /// Runs this task.
        /// </summary>
        /// <param name="manager">The manager responsible for calling this task</param>
        /// <returns></returns>
        bool TaskExec(FieAITaskController manager);

        bool Task(FieAITaskController manager);

        /// <summary>
        /// Gets a relative importance mapping for this task given different targets and contexts.
        /// </summary>
        Dictionary<Type, int> GetWeight();
    }
}
