using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	/// <summary>Retrieves exports from a part.</summary>
	public class ComposablePartExportProvider : ExportProvider, IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ComposablePartExportProvider" /> class.</summary>
		public ComposablePartExportProvider() : this(false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ComposablePartExportProvider" /> class, optionally in thread-safe mode.</summary>
		/// <param name="isThreadSafe">
		///   <see langword="true" /> if the <see cref="T:System.ComponentModel.Composition.Hosting.ComposablePartExportProvider" /> object must be thread-safe; otherwise, <see langword="false" />.</param>
		public ComposablePartExportProvider(bool isThreadSafe) : this(isThreadSafe ? CompositionOptions.IsThreadSafe : CompositionOptions.Default)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ComposablePartExportProvider" /> class with the specified composition options.</summary>
		/// <param name="compositionOptions">Options that specify the behavior of this provider.</param>
		public ComposablePartExportProvider(CompositionOptions compositionOptions)
		{
			if (compositionOptions > (CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe | CompositionOptions.ExportCompositionService))
			{
				throw new ArgumentOutOfRangeException("compositionOptions");
			}
			this._compositionOptions = compositionOptions;
			this._lock = new CompositionLock(compositionOptions.HasFlag(CompositionOptions.IsThreadSafe));
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ComposablePartExportProvider" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Composition.Hosting.ComposablePartExportProvider" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this._isDisposed)
			{
				bool flag = false;
				ImportEngine importEngine = null;
				try
				{
					using (this._lock.LockStateForWrite())
					{
						if (!this._isDisposed)
						{
							importEngine = this._importEngine;
							this._importEngine = null;
							this._sourceProvider = null;
							this._isDisposed = true;
							flag = true;
						}
					}
				}
				finally
				{
					if (importEngine != null)
					{
						importEngine.Dispose();
					}
					if (flag)
					{
						this._lock.Dispose();
					}
				}
			}
		}

		/// <summary>Gets or sets the export provider that provides access to additional <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects.</summary>
		/// <returns>A provider that provides the <see cref="T:System.ComponentModel.Composition.Hosting.ComposablePartExportProvider" /> access to <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects.  
		///  The default is <see langword="null" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Hosting.ComposablePartExportProvider" /> has been disposed of.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">This property has already been set.  
		///  -or-  
		///  The methods on the <see cref="T:System.ComponentModel.Composition.Hosting.ComposablePartExportProvider" /> have already been accessed.</exception>
		public ExportProvider SourceProvider
		{
			get
			{
				this.ThrowIfDisposed();
				return this._sourceProvider;
			}
			set
			{
				this.ThrowIfDisposed();
				Requires.NotNull<ExportProvider>(value, "value");
				using (this._lock.LockStateForWrite())
				{
					this.EnsureCanSet<ExportProvider>(this._sourceProvider);
					this._sourceProvider = value;
				}
			}
		}

		private ImportEngine ImportEngine
		{
			get
			{
				if (this._importEngine == null)
				{
					Assumes.NotNull<ExportProvider>(this._sourceProvider);
					ImportEngine importEngine = new ImportEngine(this._sourceProvider, this._compositionOptions);
					using (this._lock.LockStateForWrite())
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
				return this._importEngine;
			}
		}

		/// <summary>Gets a collection of all exports in this provider that match the conditions of the specified import.</summary>
		/// <param name="definition">The <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition" /> that defines the conditions of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> to get.</param>
		/// <param name="atomicComposition">The composition transaction to use, or <see langword="null" /> to disable transactional composition.</param>
		/// <returns>A collection of all exports in this provider that match the specified conditions.</returns>
		protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			this.EnsureRunning();
			List<ComposablePart> list = null;
			using (this._lock.LockStateForRead())
			{
				list = atomicComposition.GetValueAllowNull(this, this._parts);
			}
			if (list.Count == 0)
			{
				return null;
			}
			List<Export> list2 = new List<Export>();
			foreach (ComposablePart composablePart in list)
			{
				foreach (ExportDefinition exportDefinition in composablePart.ExportDefinitions)
				{
					if (definition.IsConstraintSatisfiedBy(exportDefinition))
					{
						list2.Add(this.CreateExport(composablePart, exportDefinition));
					}
				}
			}
			return list2;
		}

		/// <summary>Executes composition on the specified batch.</summary>
		/// <param name="batch">The batch to execute composition on.</param>
		/// <exception cref="T:System.InvalidOperationException">The container is already in the process of composing.</exception>
		public void Compose(CompositionBatch batch)
		{
			this.ThrowIfDisposed();
			this.EnsureRunning();
			Requires.NotNull<CompositionBatch>(batch, "batch");
			if (batch.PartsToAdd.Count == 0 && batch.PartsToRemove.Count == 0)
			{
				return;
			}
			CompositionResult compositionResult = CompositionResult.SucceededResult;
			List<ComposablePart> updatedPartsList = this.GetUpdatedPartsList(ref batch);
			using (AtomicComposition atomicComposition = new AtomicComposition())
			{
				if (this._currentlyComposing)
				{
					throw new InvalidOperationException(Strings.ReentrantCompose);
				}
				this._currentlyComposing = true;
				try
				{
					atomicComposition.SetValue(this, updatedPartsList);
					this.Recompose(batch, atomicComposition);
					foreach (ComposablePart part2 in batch.PartsToAdd)
					{
						try
						{
							this.ImportEngine.PreviewImports(part2, atomicComposition);
						}
						catch (ChangeRejectedException ex)
						{
							compositionResult = compositionResult.MergeResult(new CompositionResult(ex.Errors));
						}
					}
					compositionResult.ThrowOnErrors(atomicComposition);
					using (this._lock.LockStateForWrite())
					{
						this._parts = updatedPartsList;
					}
					atomicComposition.Complete();
				}
				finally
				{
					this._currentlyComposing = false;
				}
			}
			using (IEnumerator<ComposablePart> enumerator = batch.PartsToAdd.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ComposablePart part = enumerator.Current;
					compositionResult = compositionResult.MergeResult(CompositionServices.TryInvoke(delegate
					{
						this.ImportEngine.SatisfyImports(part);
					}));
				}
			}
			compositionResult.ThrowOnErrors();
		}

		private List<ComposablePart> GetUpdatedPartsList(ref CompositionBatch batch)
		{
			Assumes.NotNull<CompositionBatch>(batch);
			List<ComposablePart> list = null;
			using (this._lock.LockStateForRead())
			{
				list = this._parts.ToList<ComposablePart>();
			}
			foreach (ComposablePart item in batch.PartsToAdd)
			{
				list.Add(item);
			}
			List<ComposablePart> list2 = null;
			foreach (ComposablePart item2 in batch.PartsToRemove)
			{
				if (list.Remove(item2))
				{
					if (list2 == null)
					{
						list2 = new List<ComposablePart>();
					}
					list2.Add(item2);
				}
			}
			batch = new CompositionBatch(batch.PartsToAdd, list2);
			return list;
		}

		private void Recompose(CompositionBatch batch, AtomicComposition atomicComposition)
		{
			ComposablePartExportProvider.<>c__DisplayClass21_0 CS$<>8__locals1 = new ComposablePartExportProvider.<>c__DisplayClass21_0();
			CS$<>8__locals1.<>4__this = this;
			Assumes.NotNull<CompositionBatch>(batch);
			foreach (ComposablePart part2 in batch.PartsToRemove)
			{
				this.ImportEngine.ReleaseImports(part2, atomicComposition);
			}
			ComposablePartExportProvider.<>c__DisplayClass21_0 CS$<>8__locals2 = CS$<>8__locals1;
			IEnumerable<ExportDefinition> addedExports;
			if (batch.PartsToAdd.Count == 0)
			{
				addedExports = new ExportDefinition[0];
			}
			else
			{
				addedExports = batch.PartsToAdd.SelectMany((ComposablePart part) => part.ExportDefinitions).ToArray<ExportDefinition>();
			}
			CS$<>8__locals2.addedExports = addedExports;
			ComposablePartExportProvider.<>c__DisplayClass21_0 CS$<>8__locals3 = CS$<>8__locals1;
			IEnumerable<ExportDefinition> removedExports;
			if (batch.PartsToRemove.Count == 0)
			{
				removedExports = new ExportDefinition[0];
			}
			else
			{
				removedExports = batch.PartsToRemove.SelectMany((ComposablePart part) => part.ExportDefinitions).ToArray<ExportDefinition>();
			}
			CS$<>8__locals3.removedExports = removedExports;
			this.OnExportsChanging(new ExportsChangeEventArgs(CS$<>8__locals1.addedExports, CS$<>8__locals1.removedExports, atomicComposition));
			atomicComposition.AddCompleteAction(delegate
			{
				CS$<>8__locals1.<>4__this.OnExportsChanged(new ExportsChangeEventArgs(CS$<>8__locals1.addedExports, CS$<>8__locals1.removedExports, null));
			});
		}

		private Export CreateExport(ComposablePart part, ExportDefinition export)
		{
			return new Export(export, () => this.GetExportedValue(part, export));
		}

		private object GetExportedValue(ComposablePart part, ExportDefinition export)
		{
			this.ThrowIfDisposed();
			this.EnsureRunning();
			return CompositionServices.GetExportedValueFromComposedPart(this.ImportEngine, part, export);
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
		}

		[DebuggerStepThrough]
		private void EnsureCanRun()
		{
			if (this._sourceProvider == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ObjectMustBeInitialized, "SourceProvider"));
			}
		}

		[DebuggerStepThrough]
		private void EnsureRunning()
		{
			if (!this._isRunning)
			{
				using (this._lock.LockStateForWrite())
				{
					if (!this._isRunning)
					{
						this.EnsureCanRun();
						this._isRunning = true;
					}
				}
			}
		}

		[DebuggerStepThrough]
		private void EnsureCanSet<T>(T currentValue) where T : class
		{
			if (this._isRunning || currentValue != null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ObjectAlreadyInitialized, Array.Empty<object>()));
			}
		}

		private List<ComposablePart> _parts = new List<ComposablePart>();

		private volatile bool _isDisposed;

		private volatile bool _isRunning;

		private CompositionLock _lock;

		private ExportProvider _sourceProvider;

		private ImportEngine _importEngine;

		private volatile bool _currentlyComposing;

		private CompositionOptions _compositionOptions;
	}
}
