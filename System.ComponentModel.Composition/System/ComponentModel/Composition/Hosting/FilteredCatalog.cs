using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	/// <summary>Represents a catalog after a filter function is applied to it.</summary>
	public class FilteredCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
	{
		/// <summary>Gets a new <see cref="T:System.ComponentModel.Composition.Hosting.FilteredCatalog" /> object that contains all the parts from this catalog and all their dependencies.</summary>
		/// <returns>The new catalog.</returns>
		public FilteredCatalog IncludeDependencies()
		{
			return this.IncludeDependencies((ImportDefinition i) => i.Cardinality == ImportCardinality.ExactlyOne);
		}

		/// <summary>Gets a new <see cref="T:System.ComponentModel.Composition.Hosting.FilteredCatalog" /> object that contains all the parts from this catalog and all dependencies that can be reached through imports that match the specified filter.</summary>
		/// <param name="importFilter">The filter for imports.</param>
		/// <returns>The new catalog.</returns>
		public FilteredCatalog IncludeDependencies(Func<ImportDefinition, bool> importFilter)
		{
			Requires.NotNull<Func<ImportDefinition, bool>>(importFilter, "importFilter");
			this.ThrowIfDisposed();
			return this.Traverse(new FilteredCatalog.DependenciesTraversal(this, importFilter));
		}

		/// <summary>Gets a new <see cref="T:System.ComponentModel.Composition.Hosting.FilteredCatalog" /> object that contains all the parts from this catalog and all their dependents.</summary>
		/// <returns>The new catalog.</returns>
		public FilteredCatalog IncludeDependents()
		{
			return this.IncludeDependents((ImportDefinition i) => i.Cardinality == ImportCardinality.ExactlyOne);
		}

		/// <summary>Gets a new <see cref="T:System.ComponentModel.Composition.Hosting.FilteredCatalog" /> object that contains all the parts from this catalog and all dependents that can be reached through imports that match the specified filter.</summary>
		/// <param name="importFilter">The filter for imports.</param>
		/// <returns>The new catalog.</returns>
		public FilteredCatalog IncludeDependents(Func<ImportDefinition, bool> importFilter)
		{
			Requires.NotNull<Func<ImportDefinition, bool>>(importFilter, "importFilter");
			this.ThrowIfDisposed();
			return this.Traverse(new FilteredCatalog.DependentsTraversal(this, importFilter));
		}

		private FilteredCatalog Traverse(FilteredCatalog.IComposablePartCatalogTraversal traversal)
		{
			Assumes.NotNull<FilteredCatalog.IComposablePartCatalogTraversal>(traversal);
			this.FreezeInnerCatalog();
			FilteredCatalog result;
			try
			{
				traversal.Initialize();
				HashSet<ComposablePartDefinition> traversalClosure = FilteredCatalog.GetTraversalClosure(this._innerCatalog.Where(this._filter), traversal);
				result = new FilteredCatalog(this._innerCatalog, (ComposablePartDefinition p) => traversalClosure.Contains(p));
			}
			finally
			{
				this.UnfreezeInnerCatalog();
			}
			return result;
		}

		private static HashSet<ComposablePartDefinition> GetTraversalClosure(IEnumerable<ComposablePartDefinition> parts, FilteredCatalog.IComposablePartCatalogTraversal traversal)
		{
			Assumes.NotNull<FilteredCatalog.IComposablePartCatalogTraversal>(traversal);
			HashSet<ComposablePartDefinition> hashSet = new HashSet<ComposablePartDefinition>();
			FilteredCatalog.GetTraversalClosure(parts, hashSet, traversal);
			return hashSet;
		}

		private static void GetTraversalClosure(IEnumerable<ComposablePartDefinition> parts, HashSet<ComposablePartDefinition> traversedParts, FilteredCatalog.IComposablePartCatalogTraversal traversal)
		{
			foreach (ComposablePartDefinition composablePartDefinition in parts)
			{
				if (traversedParts.Add(composablePartDefinition))
				{
					IEnumerable<ComposablePartDefinition> parts2 = null;
					if (traversal.TryTraverse(composablePartDefinition, out parts2))
					{
						FilteredCatalog.GetTraversalClosure(parts2, traversedParts, traversal);
					}
				}
			}
		}

		private void FreezeInnerCatalog()
		{
			INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = this._innerCatalog as INotifyComposablePartCatalogChanged;
			if (notifyComposablePartCatalogChanged != null)
			{
				notifyComposablePartCatalogChanged.Changing += FilteredCatalog.ThrowOnRecomposition;
			}
		}

		private void UnfreezeInnerCatalog()
		{
			INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = this._innerCatalog as INotifyComposablePartCatalogChanged;
			if (notifyComposablePartCatalogChanged != null)
			{
				notifyComposablePartCatalogChanged.Changing -= FilteredCatalog.ThrowOnRecomposition;
			}
		}

		private static void ThrowOnRecomposition(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			throw new ChangeRejectedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.FilteredCatalog" /> class with the specified underlying catalog and filter.</summary>
		/// <param name="catalog">The underlying catalog.</param>
		/// <param name="filter">The function to filter parts.</param>
		public FilteredCatalog(ComposablePartCatalog catalog, Func<ComposablePartDefinition, bool> filter) : this(catalog, filter, null)
		{
		}

		internal FilteredCatalog(ComposablePartCatalog catalog, Func<ComposablePartDefinition, bool> filter, FilteredCatalog complement)
		{
			Requires.NotNull<ComposablePartCatalog>(catalog, "catalog");
			Requires.NotNull<Func<ComposablePartDefinition, bool>>(filter, "filter");
			this._innerCatalog = catalog;
			this._filter = ((ComposablePartDefinition p) => filter(p.GetGenericPartDefinition() ?? p));
			this._complement = complement;
			INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = this._innerCatalog as INotifyComposablePartCatalogChanged;
			if (notifyComposablePartCatalogChanged != null)
			{
				notifyComposablePartCatalogChanged.Changed += this.OnChangedInternal;
				notifyComposablePartCatalogChanged.Changing += this.OnChangingInternal;
			}
		}

		/// <summary>Called by the <see langword="Dispose()" /> and <see langword="Finalize()" /> methods to release the managed and unmanaged resources used by the current instance of the <see cref="T:System.ComponentModel.Composition.Hosting.FilteredCatalog" /> class.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && !this._isDisposed)
				{
					INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = null;
					try
					{
						object @lock = this._lock;
						lock (@lock)
						{
							if (!this._isDisposed)
							{
								this._isDisposed = true;
								notifyComposablePartCatalogChanged = (this._innerCatalog as INotifyComposablePartCatalogChanged);
								this._innerCatalog = null;
							}
						}
					}
					finally
					{
						if (notifyComposablePartCatalogChanged != null)
						{
							notifyComposablePartCatalogChanged.Changed -= this.OnChangedInternal;
							notifyComposablePartCatalogChanged.Changing -= this.OnChangingInternal;
						}
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>Returns an enumerator that iterates through the catalog.</summary>
		/// <returns>An enumerator that can be used to iterate through the catalog.</returns>
		public override IEnumerator<ComposablePartDefinition> GetEnumerator()
		{
			return this._innerCatalog.Where(this._filter).GetEnumerator();
		}

		/// <summary>Gets a catalog that contains parts that are present in the underlying catalog but that were filtered out by the filter function.</summary>
		/// <returns>A catalog that contains the complement of this catalog.</returns>
		public FilteredCatalog Complement
		{
			get
			{
				this.ThrowIfDisposed();
				if (this._complement == null)
				{
					FilteredCatalog filteredCatalog = new FilteredCatalog(this._innerCatalog, (ComposablePartDefinition p) => !this._filter(p), this);
					object @lock = this._lock;
					lock (@lock)
					{
						if (this._complement == null)
						{
							Thread.MemoryBarrier();
							this._complement = filteredCatalog;
							filteredCatalog = null;
						}
					}
					if (filteredCatalog != null)
					{
						filteredCatalog.Dispose();
					}
				}
				return this._complement;
			}
		}

		/// <summary>Gets the exported parts from this catalog that match the specified import.</summary>
		/// <param name="definition">The import to match.</param>
		/// <returns>A collection of matching parts.</returns>
		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ImportDefinition>(definition, "definition");
			List<Tuple<ComposablePartDefinition, ExportDefinition>> list = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
			foreach (Tuple<ComposablePartDefinition, ExportDefinition> tuple in this._innerCatalog.GetExports(definition))
			{
				if (this._filter(tuple.Item1))
				{
					list.Add(tuple);
				}
			}
			return list;
		}

		/// <summary>Occurs when the underlying catalog has changed.</summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

		/// <summary>Occurs when the underlying catalog is changing.</summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

		/// <summary>Raises the <see cref="E:System.ComponentModel.Composition.Hosting.FilteredCatalog.Changed" /> event.</summary>
		/// <param name="e">Provides data for the event.</param>
		protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changed = this.Changed;
			if (changed != null)
			{
				changed(this, e);
			}
		}

		/// <summary>Raises the <see cref="E:System.ComponentModel.Composition.Hosting.FilteredCatalog.Changing" /> event.</summary>
		/// <param name="e">Provides data for the event.</param>
		protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changing = this.Changing;
			if (changing != null)
			{
				changing(this, e);
			}
		}

		private void OnChangedInternal(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			ComposablePartCatalogChangeEventArgs composablePartCatalogChangeEventArgs = this.ProcessEventArgs(e);
			if (composablePartCatalogChangeEventArgs != null)
			{
				this.OnChanged(this.ProcessEventArgs(composablePartCatalogChangeEventArgs));
			}
		}

		private void OnChangingInternal(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			ComposablePartCatalogChangeEventArgs composablePartCatalogChangeEventArgs = this.ProcessEventArgs(e);
			if (composablePartCatalogChangeEventArgs != null)
			{
				this.OnChanging(this.ProcessEventArgs(composablePartCatalogChangeEventArgs));
			}
		}

		private ComposablePartCatalogChangeEventArgs ProcessEventArgs(ComposablePartCatalogChangeEventArgs e)
		{
			ComposablePartCatalogChangeEventArgs composablePartCatalogChangeEventArgs = new ComposablePartCatalogChangeEventArgs(e.AddedDefinitions.Where(this._filter), e.RemovedDefinitions.Where(this._filter), e.AtomicComposition);
			if (composablePartCatalogChangeEventArgs.AddedDefinitions.FastAny<ComposablePartDefinition>() || composablePartCatalogChangeEventArgs.RemovedDefinitions.FastAny<ComposablePartDefinition>())
			{
				return composablePartCatalogChangeEventArgs;
			}
			return null;
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private Func<ComposablePartDefinition, bool> _filter;

		private ComposablePartCatalog _innerCatalog;

		private FilteredCatalog _complement;

		private object _lock = new object();

		private volatile bool _isDisposed;

		internal class DependenciesTraversal : FilteredCatalog.IComposablePartCatalogTraversal
		{
			public DependenciesTraversal(FilteredCatalog catalog, Func<ImportDefinition, bool> importFilter)
			{
				Assumes.NotNull<FilteredCatalog>(catalog);
				Assumes.NotNull<Func<ImportDefinition, bool>>(importFilter);
				this._parts = catalog._innerCatalog;
				this._importFilter = importFilter;
			}

			public void Initialize()
			{
				this.BuildExportersIndex();
			}

			private void BuildExportersIndex()
			{
				this._exportersIndex = new Dictionary<string, List<ComposablePartDefinition>>();
				foreach (ComposablePartDefinition composablePartDefinition in this._parts)
				{
					foreach (ExportDefinition exportDefinition in composablePartDefinition.ExportDefinitions)
					{
						this.AddToExportersIndex(exportDefinition.ContractName, composablePartDefinition);
					}
				}
			}

			private void AddToExportersIndex(string contractName, ComposablePartDefinition part)
			{
				List<ComposablePartDefinition> list = null;
				if (!this._exportersIndex.TryGetValue(contractName, out list))
				{
					list = new List<ComposablePartDefinition>();
					this._exportersIndex.Add(contractName, list);
				}
				list.Add(part);
			}

			public bool TryTraverse(ComposablePartDefinition part, out IEnumerable<ComposablePartDefinition> reachableParts)
			{
				reachableParts = null;
				List<ComposablePartDefinition> list = null;
				foreach (ImportDefinition import in part.ImportDefinitions.Where(this._importFilter))
				{
					List<ComposablePartDefinition> list2 = null;
					foreach (string key in import.GetCandidateContractNames(part))
					{
						if (this._exportersIndex.TryGetValue(key, out list2))
						{
							foreach (ComposablePartDefinition composablePartDefinition in list2)
							{
								foreach (ExportDefinition export in composablePartDefinition.ExportDefinitions)
								{
									if (import.IsImportDependentOnPart(composablePartDefinition, export, part.IsGeneric() != composablePartDefinition.IsGeneric()))
									{
										if (list == null)
										{
											list = new List<ComposablePartDefinition>();
										}
										list.Add(composablePartDefinition);
									}
								}
							}
						}
					}
				}
				reachableParts = list;
				return reachableParts != null;
			}

			private IEnumerable<ComposablePartDefinition> _parts;

			private Func<ImportDefinition, bool> _importFilter;

			private Dictionary<string, List<ComposablePartDefinition>> _exportersIndex;
		}

		internal class DependentsTraversal : FilteredCatalog.IComposablePartCatalogTraversal
		{
			public DependentsTraversal(FilteredCatalog catalog, Func<ImportDefinition, bool> importFilter)
			{
				Assumes.NotNull<FilteredCatalog>(catalog);
				Assumes.NotNull<Func<ImportDefinition, bool>>(importFilter);
				this._parts = catalog._innerCatalog;
				this._importFilter = importFilter;
			}

			public void Initialize()
			{
				this.BuildImportersIndex();
			}

			private void BuildImportersIndex()
			{
				this._importersIndex = new Dictionary<string, List<ComposablePartDefinition>>();
				foreach (ComposablePartDefinition composablePartDefinition in this._parts)
				{
					foreach (ImportDefinition import in composablePartDefinition.ImportDefinitions)
					{
						foreach (string contractName in import.GetCandidateContractNames(composablePartDefinition))
						{
							this.AddToImportersIndex(contractName, composablePartDefinition);
						}
					}
				}
			}

			private void AddToImportersIndex(string contractName, ComposablePartDefinition part)
			{
				List<ComposablePartDefinition> list = null;
				if (!this._importersIndex.TryGetValue(contractName, out list))
				{
					list = new List<ComposablePartDefinition>();
					this._importersIndex.Add(contractName, list);
				}
				list.Add(part);
			}

			public bool TryTraverse(ComposablePartDefinition part, out IEnumerable<ComposablePartDefinition> reachableParts)
			{
				reachableParts = null;
				List<ComposablePartDefinition> list = null;
				foreach (ExportDefinition exportDefinition in part.ExportDefinitions)
				{
					List<ComposablePartDefinition> list2 = null;
					if (this._importersIndex.TryGetValue(exportDefinition.ContractName, out list2))
					{
						foreach (ComposablePartDefinition composablePartDefinition in list2)
						{
							using (IEnumerator<ImportDefinition> enumerator3 = composablePartDefinition.ImportDefinitions.Where(this._importFilter).GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									if (enumerator3.Current.IsImportDependentOnPart(part, exportDefinition, part.IsGeneric() != composablePartDefinition.IsGeneric()))
									{
										if (list == null)
										{
											list = new List<ComposablePartDefinition>();
										}
										list.Add(composablePartDefinition);
									}
								}
							}
						}
					}
				}
				reachableParts = list;
				return reachableParts != null;
			}

			private IEnumerable<ComposablePartDefinition> _parts;

			private Func<ImportDefinition, bool> _importFilter;

			private Dictionary<string, List<ComposablePartDefinition>> _importersIndex;
		}

		internal interface IComposablePartCatalogTraversal
		{
			void Initialize();

			bool TryTraverse(ComposablePartDefinition part, out IEnumerable<ComposablePartDefinition> reachableParts);
		}
	}
}
