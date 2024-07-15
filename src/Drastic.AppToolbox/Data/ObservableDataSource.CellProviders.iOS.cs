// <copyright file="ObservableDataSource.CellProviders.iOS.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.AppToolbox.Data;

/// <summary>
/// Represents an observable data source.
/// </summary>
/// <typeparam name="T">The type of the data source.</typeparam>
public partial class ObservableDataSource<T>
{
    private Dictionary<WeakReference<object>, ITableCellProvider<T>> tableCellProviders;

    /// <summary>
    /// Gets the dictionary of table cell providers.
    /// </summary>
    private Dictionary<WeakReference<object>, ITableCellProvider<T>> TableCellProviders
    {
        get { return this.tableCellProviders ?? (this.tableCellProviders = new Dictionary<WeakReference<object>, ITableCellProvider<T>>()); }
    }

    /// <summary>
    /// Sets the cell provider for a specific table.
    /// </summary>
    /// <param name="table">The table object.</param>
    /// <param name="provider">The cell provider.</param>
    public void SetCellProvider(object table, ITableCellProvider<T> provider)
    {
        var type = new WeakReference<object>(table);
        this.TableCellProviders.Remove(type);
        this.TableCellProviders.Add(type, provider);
    }

    /// <summary>
    /// Finds the cell provider for a specific table.
    /// </summary>
    /// <param name="table">The table object.</param>
    /// <returns>The cell provider.</returns>
    private ITableCellProvider<T> FindProvider(object table)
    {
        return this.TableCellProviders.Where(a =>
        {
            object o;
            return a.Key.TryGetTarget(out o) && ReferenceEquals(o, table);
        }).Select(b => b.Value).FirstOrDefault();
    }
}
