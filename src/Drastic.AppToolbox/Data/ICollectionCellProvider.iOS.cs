// <copyright file="ICollectionCellProvider.iOS.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.AppToolbox.Data;

/// <summary>
/// Represents a provider for collection view cells in iOS.
/// </summary>
/// <typeparam name="T">The type of the item.</typeparam>
public interface ICollectionCellProvider<T>
{
    /// <summary>
    /// Gets the collection view cell for the specified item at the specified index path.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="indexPath">The index path.</param>
    /// <returns>The collection view cell.</returns>
    UICollectionViewCell GetCell(T item, NSIndexPath indexPath);
}
