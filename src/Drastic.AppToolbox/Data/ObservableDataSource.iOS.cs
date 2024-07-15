// <copyright file="ObservableDataSource.iOS.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.AppToolbox.Data;

/// <summary>
/// The observable data source.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
public partial class ObservableDataSource<T>
{
    private float defaultRowHeight = 22;
    private CollectionDataSource? collectionSource;
    private CollectionViewDelegate? collectionDelegate;

    private TableViewDelegate? tableDelegate;
    private TableDataSource? dataSource;

    /// <summary>
    /// The cell identifier.
    /// </summary>
    /// <remarks>Default value is "cid".</remarks>
    private string cellId = "cid";

    /// <summary>
    /// Gets or sets the default height of the row.
    /// </summary>
    /// <value>The default height of the row.</value>
    public float DefaultRowHeight
    {
        get { return this.defaultRowHeight; }
        set { this.defaultRowHeight = value; }
    }

    /// <summary>
    /// Gets or sets the cell identifier.
    /// </summary>
    /// <value>The cell identifier.</value>
    public string CellId
    {
        get { return this.cellId; }
        set { this.cellId = value; }
    }

    private TableDataSource DataSource
    {
        get
        {
            return this.dataSource ??
                (this.dataSource =
                new TableDataSource(this.GetCell!, this.RowsInSection));
        }
    }

    private TableViewDelegate TableDelegate
    {
        get
        {
            return this.tableDelegate ??
                (this.tableDelegate =
                new TableViewDelegate(this.RowSelected, this.GetHeightForRow));
        }
    }

    private CollectionDataSource CollectionSource
    {
        get
        {
            return this.collectionSource ??
                (this.collectionSource = new CollectionDataSource(this.RowsInSection, this.GetCell!));
        }
    }

    private CollectionViewDelegate CollectionDelegate
    {
        get
        {
            return this.collectionDelegate ??
                (this.collectionDelegate = new CollectionViewDelegate(this.ItemSelected));
        }
    }

    /// <summary>
    /// The collection changed event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="notifyCollectionChangedEventArgs">
    /// The notify collection changed event args.
    /// </param>
    partial void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
    {
        var refs = this.observers.Where(a => a.IsAlive).Select(a => a.Target).ToList();

        foreach (var tableView in refs.OfType<UITableView>())
        {
            tableView.InvokeOnMainThread(tableView.ReloadData);
        }

        foreach (var collectionView in refs.OfType<UICollectionView>())
        {
            collectionView.InvokeOnMainThread(collectionView.ReloadData);
        }
    }

    /// <summary>
    /// The observers changed event.
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

            foreach (var tableView in refs.OfType<UITableView>())
            {
                tableView.DataSource = this.DataSource;
                tableView.Delegate = this.TableDelegate;
                tableView.InvokeOnMainThread(tableView.ReloadData);
            }

            // TODO: check why only ICollectionCellProvider
            foreach (var collectionView in refs.OfType<UICollectionView>().Where(a => a is ICollectionCellProvider<T>))
            {
                collectionView.DataSource = this.CollectionSource;
                collectionView.Delegate = this.CollectionDelegate;
                collectionView.InvokeOnMainThread(collectionView.ReloadData);
            }
        }
        else if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove)
        {
            List<object?>? refs = notifyCollectionChangedEventArgs.OldItems?.OfType<WeakReference>().Where(a => a.IsAlive).Select(a => a.Target).ToList() ?? new();

            foreach (var tableView in refs.OfType<UITableView>())
            {
                tableView.DataSource = null!;
                tableView.Delegate = null!;
                tableView.InvokeOnMainThread(tableView.ReloadData);
            }

            foreach (var collectionView in refs.OfType<UICollectionView>())
            {
                collectionView.DataSource = null!;
                collectionView.Delegate = null!;
                collectionView.InvokeOnMainThread(collectionView.ReloadData);
            }
        }
    }

    #region UITableView weak delegate

    /// <summary>
    /// Gets cell for UITableView.
    /// </summary>
    /// <param name="tableView">
    /// The table view.
    /// </param>
    /// <param name="indexPath">
    /// The index path.
    /// </param>
    /// <returns>
    /// The <see cref="UITableViewCell"/>.
    /// </returns>
#pragma warning disable SA1202 // Elements should be ordered by access
    public UITableViewCell? GetCell(UITableView tableView, NSIndexPath indexPath)
#pragma warning restore SA1202 // Elements should be ordered by access
    {
        if (this.Data is null)
        {
            return null;
        }

        this.InvokeItemRequestedEvent(tableView, indexPath.Row);

        var item = this.Data[indexPath.Row];

        var cellProvider = tableView as ITableCellProvider<T> ?? this.FindProvider(tableView);

        if (cellProvider != null)
        {
            return cellProvider.GetCell(item);
        }

        var cell = tableView.DequeueReusableCell(this.CellId) ??
            new UITableViewCell(UITableViewCellStyle.Value1, this.CellId);

#pragma warning disable CA1422 // プラットフォームの互換性を検証
        cell.TextLabel.Text = item?.ToString();
#pragma warning restore CA1422 // プラットフォームの互換性を検証
        return cell;
    }

    /// <summary>
    /// The rows in section.
    /// </summary>
    /// <param name="tableView">
    /// The table view.
    /// </param>
    /// <param name="section">
    /// The section.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public int RowsInSection(UITableView tableView, int section)
    {
        return this.Data?.Count ?? 0;
    }

    /// <summary>
    /// Handles the row selected event for UITableView.
    /// </summary>
    /// <param name="tableView">The UITableView instance.</param>
    /// <param name="indexPath">The index path of the selected row.</param>
    public void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
        if (this.Data is null)
        {
            return;
        }

        this.InvokeItemSelectedEvent(tableView, this.Data[(int)indexPath.Item]);
    }

    /// <summary>
    /// Gets the height for a specific row in UITableView.
    /// </summary>
    /// <param name="tableView">The UITableView instance.</param>
    /// <param name="indexPath">The index path of the row.</param>
    /// <returns>The height of the row.</returns>
    public float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
    {
        if (this.Data is null)
        {
            return this.DefaultRowHeight;
        }

        var cellProvider = tableView as ITableCellProvider<T> ?? this.FindProvider(tableView);

        return cellProvider != null ?
            cellProvider.GetHeightForRow(indexPath, this.Data[(int)indexPath.Item]) :
            this.DefaultRowHeight;
    }

    #endregion

    #region UICollectionView weak delegate

    /// <summary>
    /// Handles the item selected event for UICollectionView.
    /// </summary>
    /// <param name="collectionView">The UICollectionView instance.</param>
    /// <param name="indexPath">The index path of the selected item.</param>
    public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
    {
        if (this.Data is null)
        {
            return;
        }

        this.InvokeItemSelectedEvent(collectionView, this.Data[(int)indexPath.Item]);
    }

    /// <summary>
    /// Gets the number of rows in the specified section of UICollectionView.
    /// </summary>
    /// <param name="tableView">The UICollectionView instance.</param>
    /// <param name="section">The section index.</param>
    /// <returns>The number of rows in the section.</returns>
    public int RowsInSection(UICollectionView tableView, int section)
    {
        return this.Data?.Count ?? 0;
    }

    /// <summary>
    /// Gets the cell for the specified item in UICollectionView.
    /// </summary>
    /// <param name="collectionView">The UICollectionView instance.</param>
    /// <param name="indexPath">The index path of the item.</param>
    /// <returns>The cell for the item.</returns>
    public UICollectionViewCell? GetCell(UICollectionView collectionView, NSIndexPath indexPath)
    {
        if (this.Data is null)
        {
            return null;
        }

        var cellProvider = collectionView as ICollectionCellProvider<T>;

        return cellProvider?.GetCell(this.Data[indexPath.Row], indexPath);
    }
    #endregion

    private class TableDataSource : UITableViewDataSource
    {
        public delegate UITableViewCell OnGetCell(UITableView tableView, NSIndexPath indexPath);

        public delegate int OnRowsInSection(UITableView tableView, int section);

        private readonly OnGetCell onGetCell;
        private readonly OnRowsInSection onRowsInSection;

        public TableDataSource(OnGetCell onGetCell, OnRowsInSection onRowsInSection)
        {
            this.onGetCell = onGetCell;
            this.onRowsInSection = onRowsInSection;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return this.onGetCell(tableView, indexPath);
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this.onRowsInSection(tableView, (int)section);
        }
    }

    private class TableViewDelegate : UITableViewDelegate
    {
        public delegate void OnRowSelected(UITableView tableView, NSIndexPath indexPath);

        public delegate float OnGetHeightForRow(UITableView tableView, NSIndexPath indexPath);

        private readonly OnRowSelected onRowSelected;
        private readonly OnGetHeightForRow onGetHeightForRow;

        public TableViewDelegate(OnRowSelected onRowSelected, OnGetHeightForRow onGetHeightForRow)
        {
            this.onRowSelected = onRowSelected;
            this.onGetHeightForRow = onGetHeightForRow;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return this.onGetHeightForRow(tableView, indexPath);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            this.onRowSelected(tableView, indexPath);
        }
    }

    private class CollectionDataSource : UICollectionViewDataSource
    {
        public delegate UICollectionViewCell OnGetCell(UICollectionView collectionView, NSIndexPath indexPath);

        public delegate int OnRowsInSection(UICollectionView tableView, int section);

        private readonly OnRowsInSection onRowsInSection;
        private readonly OnGetCell onGetCell;

        public CollectionDataSource(OnRowsInSection onRowsInSection, OnGetCell onGetCell)
        {
            this.onRowsInSection = onRowsInSection;
            this.onGetCell = onGetCell;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return this.onGetCell(collectionView, indexPath);
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.onRowsInSection(collectionView, (int)section);
        }
    }

    private class CollectionViewDelegate : UICollectionViewDelegate
    {
        public delegate void OnItemSelected(UICollectionView collectionView, NSIndexPath indexPath);

        private readonly OnItemSelected onSelected;

        public CollectionViewDelegate(OnItemSelected onSelected)
        {
            this.onSelected = onSelected;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            this.onSelected(collectionView, indexPath);
        }
    }
}
