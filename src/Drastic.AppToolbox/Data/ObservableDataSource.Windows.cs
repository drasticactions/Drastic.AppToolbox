// <copyright file="ObservableDataSource.Windows.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Windows.Foundation;

namespace Drastic.AppToolbox.Data;

/// <summary>
/// The observable data source.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
public partial class ObservableDataSource<T> : ISupportIncrementalLoading
{
    /// <summary>
    /// Gets a value indicating whether there are more items to load.
    /// </summary>
    /// <returns><c>true</c> if there are more items to load; otherwise, <c>false</c>.</returns>
    public bool HasMoreItems { get; private set; } = true;

    /// <summary>
    /// Loads more items asynchronously.
    /// </summary>
    /// <param name="count">The number of items to load.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
    {
        return AsyncInfo.Run((c) => this.LoadMoreItemsAsync(c, count));
    }

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

    private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
    {
        var result = await this.LoadMoreItemsAsync((int)count);
        this.HasMoreItems = result > 0;
        return new LoadMoreItemsResult { Count = (uint)result };
    }
}
