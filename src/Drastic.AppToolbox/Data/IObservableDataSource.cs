// <copyright file="IObservableDataSource.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

#nullable enable

using System.Collections.ObjectModel;
using Drastic.AppToolbox.Core;

namespace Drastic.AppToolbox.Data;

/// <summary>
/// Represents an observable data source.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
public interface IObservableDataSource<T>
{
    /// <summary>
    /// Occurs when an item is selected.
    /// </summary>
    event EventHandler<EventArgs<T>>? OnSelected;

    /// <summary>
    /// Occurs when a request is made.
    /// </summary>
    event EventHandler<EventArgs<int>>? OnRequested;

    /// <summary>
    /// Gets or sets the collection of data.
    /// </summary>
    ObservableCollection<T>? Data { get; set; }

    /// <summary>
    /// Sets the filter predicate.
    /// </summary>
    Predicate<T>? Filter { set; }

    /// <summary>
    /// Binds an observer to the data source.
    /// </summary>
    /// <param name="observer">The observer to bind.</param>
    void Bind(object observer);

    /// <summary>
    /// Unbinds an observer from the data source.
    /// </summary>
    /// <param name="observer">The observer to unbind.</param>
    /// <returns><c>true</c> if the observer was successfully unbound; otherwise, <c>false</c>.</returns>
    bool Unbind(object observer);

    /// <summary>
    /// Adds an item to the data source.
    /// </summary>
    /// <param name="item">The item to add.</param>
    void Add(T item);

    /// <summary>
    /// Replaces an item in the data source with a new item.
    /// </summary>
    /// <param name="original">The original item to replace.</param>
    /// <param name="replacement">The replacement item.</param>
    /// <returns><c>true</c> if the item was successfully replaced; otherwise, <c>false</c>.</returns>
    bool Replace(T original, T replacement);

    /// <summary>
    /// Removes an item from the data source.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    void Remove(T item);
}