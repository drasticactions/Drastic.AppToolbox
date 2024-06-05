// <copyright file="TaskUtilities.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.AppToolbox.Services;

namespace Drastic.AppToolbox.Tools
{
    /// <summary>
    /// Task Utilities.
    /// </summary>
    public static class TaskUtilities
    {
        /// <summary>
        /// Fire and Forget Safe Async.
        /// </summary>
        /// <param name="task">Task to Fire and Forget.</param>
        /// <param name="handler">Error Handler.</param>
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler? handler = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.HandleError(ex);
            }
        }
    }
}
