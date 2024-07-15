// <copyright file="EventArgs.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.AppToolbox.Core;

/// <summary>
/// Generic event argument class.
/// </summary>
/// <typeparam name="T">Type of the argument.</typeparam>
public class EventArgs<T> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventArgs{T}"/> class.
    /// </summary>
    /// <param name="value">Type of the argument.</param>
    public EventArgs(T value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Gets the value of the event argument.
    /// </summary>
    public T Value { get; private set; }
}