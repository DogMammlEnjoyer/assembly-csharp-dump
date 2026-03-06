using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	/// <summary>Manages the composition of parts.</summary>
	public class CompositionContainer : ExportProvider, ICompositionService, IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> class.</summary>
		public CompositionContainer() : this(null, Array.Empty<ExportProvider>())
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> class with the specified export providers.</summary>
		/// <param name="providers">An array of <see cref="T:System.ComponentModel.Composition.Hosting.ExportProvider" /> objects that provide the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> access to <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects, or <see langword="null" /> to set <see cref="P:System.ComponentModel.Composition.Hosting.CompositionContainer.Providers" /> to an empty <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="providers" /> contains an element that is <see langword="null" />.</exception>
		public CompositionContainer(params ExportProvider[] providers) : this(null, providers)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> class with the specified export providers and options.</summary>
		/// <param name="compositionOptions">An object that specifies the behavior of this container.</param>
		/// <param name="providers">An array of <see cref="T:System.ComponentModel.Composition.Hosting.ExportProvider" /> objects that provide the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> access to <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects, or <see langword="null" /> to set <see cref="P:System.ComponentModel.Composition.Hosting.CompositionContainer.Providers" /> to an empty <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="providers" /> contains an element that is <see langword="null" />.</exception>
		public CompositionContainer(CompositionOptions compositionOptions, params ExportProvider[] providers) : this(null, compositionOptions, providers)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> class with the specified catalog and export providers.</summary>
		/// <param name="catalog">A catalog that provides <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects to the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" />.</param>
		/// <param name="providers">An array of <see cref="T:System.ComponentModel.Composition.Hosting.ExportProvider" /> objects that provide the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> access to <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects, or <see langword="null" /> to set <see cref="P:System.ComponentModel.Composition.Hosting.CompositionContainer.Providers" /> to an empty <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="providers" /> contains an element that is <see langword="null" />.</exception>
		public CompositionContainer(ComposablePartCatalog catalog, params ExportProvider[] providers) : this(catalog, false, providers)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> class with the specified catalog, thread-safe mode, and export providers.</summary>
		/// <param name="catalog">A catalog that provides <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects to the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" />.</param>
		/// <param name="isThreadSafe">
		///   <see langword="true" /> if this <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> object must be thread-safe; otherwise, <see langword="false" />.</param>
		/// <param name="providers">An array of <see cref="T:System.ComponentModel.Composition.Hosting.ExportProvider" /> objects that provide the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> access to <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects, or <see langword="null" /> to set the <see cref="P:System.ComponentModel.Composition.Hosting.CompositionContainer.Providers" /> property to an empty <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />.</param>
		/// <exception cref="T:System.ArgumentException">One or more elements of <paramref name="providers" /> are <see langword="null" />.</exception>
		public CompositionContainer(ComposablePartCatalog catalog, bool isThreadSafe, params ExportProvider[] providers) : this(catalog, isThreadSafe ? CompositionOptions.IsThreadSafe : CompositionOptions.Default, providers)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> class with the specified catalog, options, and export providers.</summary>
		/// <param name="catalog">A catalog that provides <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects to the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" />.</param>
		/// <param name="compositionOptions">An object that specifies options that affect the behavior of the container.</param>
		/// <param name="providers">An array of <see cref="T:System.ComponentModel.Composition.Hosting.ExportProvider" /> objects that provide the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> access to <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects, or <see langword="null" /> to set <see cref="P:System.ComponentModel.Composition.Hosting.CompositionContainer.Providers" /> to an empty <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="providers" /> contains an element that is <see langword="null" />.</exception>
		public CompositionContainer(ComposablePartCatalog catalog, CompositionOptions compositionOptions, params ExportProvider[] providers)
		{
			if (compositionOptions > (CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe | CompositionOptions.ExportCompositionService))
			{
				throw new ArgumentOutOfRangeException("compositionOptions");
			}
			this._compositionOptions = compositionOptions;
			this._partExportProvider = new ComposablePartExportProvider(compositionOptions);
			this._partExportProvider.SourceProvider = this;
			if (catalog != null || providers.Length != 0)
			{
				if (catalog != null)
				{
					this._catalogExportProvider = new CatalogExportProvider(catalog, compositionOptions);
					this._catalogExportProvider.SourceProvider = this;
					this._localExportProvider = new AggregateExportProvider(new ExportProvider[]
					{
						this._partExportProvider,
						this._catalogExportProvider
					});
				}
				else
				{
					this._localExportProvider = new AggregateExportProvider(new ExportProvider[]
					{
						this._partExportProvider
					});
				}
				if (providers != null && providers.Length != 0)
				{
					this._ancestorExportProvider = new AggregateExportProvider(providers);
					this._rootProvider = new AggregateExportProvider(new ExportProvider[]
					{
						this._localExportProvider,
						this._ancestorExportProvider
					});
				}
				else
				{
					this._rootProvider = this._localExportProvider;
				}
			}
			else
			{
				this._rootProvider = this._partExportProvider;
			}
			if (compositionOptions.HasFlag(CompositionOptions.ExportCompositionService))
			{
				this.ComposeExportedValue(new CompositionContainer.CompositionServiceShim(this));
			}
			this._rootProvider.ExportsChanged += this.OnExportsChangedInternal;
			this._rootProvider.ExportsChanging += this.OnExportsChangingInternal;
			this._providers = ((providers != null) ? new ReadOnlyCollection<ExportProvider>((ExportProvider[])providers.Clone()) : CompositionContainer.EmptyProviders);
		}

		internal CompositionOptions CompositionOptions
		{
			get
			{
				this.ThrowIfDisposed();
				return this._compositionOptions;
			}
		}

		/// <summary>Gets the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog" /> that provides the container access to <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects.</summary>
		/// <returns>The catalog that provides the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> access to exports produced from <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> objects. The default is <see langword="null" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> object has been disposed of.</exception>
		public ComposablePartCatalog Catalog
		{
			get
			{
				this.ThrowIfDisposed();
				if (this._catalogExportProvider == null)
				{
					return null;
				}
				return this._catalogExportProvider.Catalog;
			}
		}

		internal CatalogExportProvider CatalogExportProvider
		{
			get
			{
				this.ThrowIfDisposed();
				return this._catalogExportProvider;
			}
		}

		/// <summary>Gets the export providers that provide the container access to additional <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog" /> objects.</summary>
		/// <returns>A collection of <see cref="T:System.ComponentModel.Composition.Hosting.ExportProvider" /> objects that provide the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> access to additional <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects. The default is an empty <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> has been disposed of.</exception>
		public ReadOnlyCollection<ExportProvider> Providers
		{
			get
			{
				this.ThrowIfDisposed();
				return this._providers;
			}
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this._isDisposed)
			{
				ExportProvider exportProvider = null;
				AggregateExportProvider aggregateExportProvider = null;
				AggregateExportProvider aggregateExportProvider2 = null;
				ComposablePartExportProvider composablePartExportProvider = null;
				CatalogExportProvider catalogExportProvider = null;
				ImportEngine importEngine = null;
				object @lock = this._lock;
				lock (@lock)
				{
					if (!this._isDisposed)
					{
						exportProvider = this._rootProvider;
						this._rootProvider = null;
						aggregateExportProvider2 = this._localExportProvider;
						this._localExportProvider = null;
						aggregateExportProvider = this._ancestorExportProvider;
						this._ancestorExportProvider = null;
						composablePartExportProvider = this._partExportProvider;
						this._partExportProvider = null;
						catalogExportProvider = this._catalogExportProvider;
						this._catalogExportProvider = null;
						importEngine = this._importEngine;
						this._importEngine = null;
						this._isDisposed = true;
					}
				}
				if (exportProvider != null)
				{
					exportProvider.ExportsChanged -= this.OnExportsChangedInternal;
					exportProvider.ExportsChanging -= this.OnExportsChangingInternal;
				}
				if (aggregateExportProvider != null)
				{
					aggregateExportProvider.Dispose();
				}
				if (aggregateExportProvider2 != null)
				{
					aggregateExportProvider2.Dispose();
				}
				if (catalogExportProvider != null)
				{
					catalogExportProvider.Dispose();
				}
				if (composablePartExportProvider != null)
				{
					composablePartExportProvider.Dispose();
				}
				if (importEngine != null)
				{
					importEngine.Dispose();
				}
			}
		}

		/// <summary>Adds or removes the parts in the specified <see cref="T:System.ComponentModel.Composition.Hosting.CompositionBatch" /> from the container and executes composition.</summary>
		/// <param name="batch">Changes to the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> to include during the composition.</param>
		public void Compose(CompositionBatch batch)
		{
			Requires.NotNull<CompositionBatch>(batch, "batch");
			this.ThrowIfDisposed();
			this._partExportProvider.Compose(batch);
		}

		/// <summary>Releases the specified <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> object from the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" />.</summary>
		/// <param name="export">The <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> that needs to be released.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="export" /> is <see langword="null" />.</exception>
		public void ReleaseExport(Export export)
		{
			Requires.NotNull<Export>(export, "export");
			IDisposable disposable = export as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		/// <summary>Removes the specified export from composition and releases its resources if possible.</summary>
		/// <param name="export">An indirect reference to the export to remove.</param>
		/// <typeparam name="T">The type of the export.</typeparam>
		public void ReleaseExport<T>(Lazy<T> export)
		{
			Requires.NotNull<Lazy<T>>(export, "export");
			IDisposable disposable = export as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		/// <summary>Releases a set of <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects from the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" />.</summary>
		/// <param name="exports">A collection of <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects to be released.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="exports" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="exports" /> contains an element that is <see langword="null" />.</exception>
		public void ReleaseExports(IEnumerable<Export> exports)
		{
			Requires.NotNullOrNullElements<Export>(exports, "exports");
			foreach (Export export in exports)
			{
				this.ReleaseExport(export);
			}
		}

		/// <summary>Removes a collection of exports from composition and releases their resources if possible.</summary>
		/// <param name="exports">A collection of indirect references to the exports to be removed.</param>
		/// <typeparam name="T">The type of the exports.</typeparam>
		public void ReleaseExports<T>(IEnumerable<Lazy<T>> exports)
		{
			Requires.NotNullOrNullElements<Lazy<T>>(exports, "exports");
			foreach (Lazy<T> export in exports)
			{
				this.ReleaseExport<T>(export);
			}
		}

		/// <summary>Removes a collection of exports from composition and releases their resources if possible.</summary>
		/// <param name="exports">A collection of indirect references to the exports to be removed and their metadata.</param>
		/// <typeparam name="T">The type of the exports.</typeparam>
		/// <typeparam name="TMetadataView">The type of the exports' metadata view.</typeparam>
		public void ReleaseExports<T, TMetadataView>(IEnumerable<Lazy<T, TMetadataView>> exports)
		{
			Requires.NotNullOrNullElements<Lazy<T, TMetadataView>>(exports, "exports");
			foreach (Lazy<T, TMetadataView> export in exports)
			{
				this.ReleaseExport<T>(export);
			}
		}

		/// <summary>Satisfies the imports of the specified <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> object without registering it for recomposition.</summary>
		/// <param name="part">The part to satisfy the imports of.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> object has been disposed of.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="part" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ComponentModel.Composition.CompositionException">An error occurred during composition. <see cref="P:System.ComponentModel.Composition.CompositionException.Errors" /> will contain a collection of the errors that occurred.</exception>
		public void SatisfyImportsOnce(ComposablePart part)
		{
			this.ThrowIfDisposed();
			if (this._importEngine == null)
			{
				ImportEngine importEngine = new ImportEngine(this, this._compositionOptions);
				object @lock = this._lock;
				lock (@lock)
				{
					if (this._importEngine == null)
					{
						Thread.MemoryBarrier();
						this._importEngine = importEngine;
						importEngine = null;
					}
				}
				if (importEngine != null)
				{
					importEngine.Dispose();
				}
			}
			this._importEngine.SatisfyImportsOnce(part);
		}

		internal void OnExportsChangedInternal(object sender, ExportsChangeEventArgs e)
		{
			this.OnExportsChanged(e);
		}

		internal void OnExportsChangingInternal(object sender, ExportsChangeEventArgs e)
		{
			this.OnExportsChanging(e);
		}

		/// <summary>Returns a collection of all exports that match the conditions in the specified <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition" /> object.</summary>
		/// <param name="definition">The object that defines the conditions of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects to get.</param>
		/// <param name="atomicComposition">The composition transaction to use, or <see langword="null" /> to disable transactional composition.</param>
		/// <returns>A collection of all the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects in this <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> object that match the conditions specified by <paramref name="definition" />.</returns>
		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			IEnumerable<Export> result = null;
			object obj;
			if (!definition.Metadata.TryGetValue("System.ComponentModel.Composition.ImportSource", out obj))
			{
				obj = ImportSource.Any;
			}
			switch ((ImportSource)obj)
			{
			case ImportSource.Any:
				Assumes.NotNull<ExportProvider>(this._rootProvider);
				this._rootProvider.TryGetExports(definition, atomicComposition, out result);
				break;
			case ImportSource.Local:
				Assumes.NotNull<AggregateExportProvider>(this._localExportProvider);
				this._localExportProvider.TryGetExports(definition.RemoveImportSource(), atomicComposition, out result);
				break;
			case ImportSource.NonLocal:
				if (this._ancestorExportProvider != null)
				{
					this._ancestorExportProvider.TryGetExports(definition.RemoveImportSource(), atomicComposition, out result);
				}
				break;
			}
			return result;
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private CompositionOptions _compositionOptions;

		private ImportEngine _importEngine;

		private ComposablePartExportProvider _partExportProvider;

		private ExportProvider _rootProvider;

		private CatalogExportProvider _catalogExportProvider;

		private AggregateExportProvider _localExportProvider;

		private AggregateExportProvider _ancestorExportProvider;

		private readonly ReadOnlyCollection<ExportProvider> _providers;

		private volatile bool _isDisposed;

		private object _lock = new object();

		private static ReadOnlyCollection<ExportProvider> EmptyProviders = new ReadOnlyCollection<ExportProvider>(new ExportProvider[0]);

		private class CompositionServiceShim : ICompositionService
		{
			public CompositionServiceShim(CompositionContainer innerContainer)
			{
				Assumes.NotNull<CompositionContainer>(innerContainer);
				this._innerContainer = innerContainer;
			}

			void ICompositionService.SatisfyImportsOnce(ComposablePart part)
			{
				this._innerContainer.SatisfyImportsOnce(part);
			}

			private CompositionContainer _innerContainer;
		}
	}
}
