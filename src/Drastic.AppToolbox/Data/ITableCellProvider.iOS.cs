// <copyright file="ITableCellProvider.iOS.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.AppToolbox.Data;

/// <summary>
/// Table cell provider interface.
/// </summary>
/// <description>
/// Implement this interface in your UITableView class to override
/// the default table cell view in your application.
/// NOTE: Implementing this in other than the table view bind to the
/// data source will not have any effect.
/// </description>
/// <typeparam name="T">The type of the item.</typeparam>
public interface ITableCellProvider<T>
{
    /// <summary>
    /// Gets the custom cell.
    /// </summary>
    /// <returns><see cref="UITableViewCell"/>.</returns>
    /// <param name="tableView">UITableView.</param>
    /// <param name="item">Item.</param>
    UITableViewCell GetCell(UITableView tableView, T item);

    /// <summary>
    /// Gets the height for row.
    /// </summary>
    /// <returns><see cref="float"/>.</returns>
    /// <param name="indexPath">Index path.</param>
    /// <param name="item">The type of the item.</param>
    float GetHeightForRow(NSIndexPath indexPath, T item);
}
