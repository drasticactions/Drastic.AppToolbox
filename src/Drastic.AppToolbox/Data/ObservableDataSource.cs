// <copyright file="ObservableDataSource.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.AppToolbox.Data;

#if !WINDOWS && !IOS && !MACCATALYST && !TVOS
/// <summary>
/// The observable data source.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
public partial class ObservableDataSource<T>
{
    /// <summary>
    /// The collection changed partial method must be implemented in the OS specific code.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="notifyCollectionChangedEventArgs">
    /// The notify collection changed event args.
    /// </param>
    partial void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
    {
    }

    /// <summary>
    /// Partial class to be implemented in the OS specific code.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="notifyCollectionChangedEventArgs">
    /// The notify collection changed event args.
    /// </param>
    partial void ObserversChanged(object? sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
    {
    }
}
#endif
