// <copyright file="IErrorHandler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.AppToolbox.Services
{
    /// <summary>
    /// Error Handler Service.
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// Handle error in UI.
        /// </summary>
        /// <param name="ex">Exception being thrown.</param>
        void HandleError(Exception ex);
    }
}