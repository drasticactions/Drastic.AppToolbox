// <copyright file="ObservableDataSource.Windows.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Drastic.AppToolbox.Data;

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
        if (notifyCollectionChangedEventArgs.Action != NotifyCollectionChangedAction.Reset)
        {
            return;
        }

        var refs = this.observers.Where(a => a.IsAlive).Select(a => a.Target).ToList();

        foreach (var observer in refs.OfType<FrameworkElement>())
        {
            observer.DispatcherQueue.TryEnqueue(() => observer.DataContext = this.Data);
        }

        foreach (var longList in refs.OfType<ListView>())
        {
            longList.DispatcherQueue.TryEnqueue(() => longList.ItemsSource = this.Data);
        }
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
        if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
        {
            List<object?>? refs = notifyCollectionChangedEventArgs.NewItems?.OfType<WeakReference>().Where(a => a.IsAlive).Select(a => a.Target).ToList() ?? new();

            foreach (var element in refs.OfType<FrameworkElement>())
            {
                element.DispatcherQueue.TryEnqueue(() => element.DataContext = this.Data);
            }

            foreach (var longList in refs.OfType<ListView>())
            {
                longList.DispatcherQueue.TryEnqueue(() => longList.ItemsSource = this.Data);
                longList.SelectionChanged += this.LongList_SelectionChanged;
            }
        }
        else if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove)
        {
            List<object?>? refs = notifyCollectionChangedEventArgs.OldItems?.OfType<WeakReference>().Where(a => a.IsAlive).Select(a => a.Target).ToList() ?? new();
            foreach (var element in refs.OfType<FrameworkElement>())
            {
                element.DispatcherQueue.TryEnqueue(() => element.DataContext = null);
            }

            foreach (var longList in refs.OfType<ListView>())
            {
                longList.DispatcherQueue.TryEnqueue(() => longList.ItemsSource = null);
                longList.SelectionChanged -= this.LongList_SelectionChanged;
            }
        }
    }

    private void LongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (var item in e.AddedItems.OfType<T>())
        {
            this.InvokeItemSelectedEvent(sender, item);
        }
    }
}
