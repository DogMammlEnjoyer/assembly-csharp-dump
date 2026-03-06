using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	/// <summary>Performs composition for containers.</summary>
	public class ImportEngine : ICompositionService, IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ImportEngine" /> class.</summary>
		/// <param name="sourceProvider">The <see cref="T:System.ComponentModel.Composition.Hosting.ExportProvider" /> that provides the <see cref="T:System.ComponentModel.Composition.Hosting.ImportEngine" /> access to <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects.</param>
		public ImportEngine(ExportProvider sourceProvider) : this(sourceProvider, CompositionOptions.Default)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ImportEngine" /> class, optionally in thread-safe mode.</summary>
		/// <param name="sourceProvider">The <see cref="T:System.ComponentModel.Composition.Hosting.ExportProvider" /> that provides the <see cref="T:System.ComponentModel.Composition.Hosting.ImportEngine" /> access to <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects.</param>
		/// <param name="isThreadSafe">
		///   <see langword="true" /> if thread safety is required; otherwise, <see langword="false" />.</param>
		public ImportEngine(ExportProvider sourceProvider, bool isThreadSafe) : this(sourceProvider, isThreadSafe ? CompositionOptions.IsThreadSafe : CompositionOptions.Default)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ImportEngine" /> class with the specified options.</summary>
		/// <param name="sourceProvider">The <see cref="T:System.ComponentModel.Composition.Hosting.ExportProvider" /> that provides the <see cref="T:System.ComponentModel.Composition.Hosting.ImportEngine" /> access to <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects.</param>
		/// <param name="compositionOptions">An object that specifies options that affect the behavior of the engine.</param>
		public ImportEngine(ExportProvider sourceProvider, CompositionOptions compositionOptions)
		{
			Requires.NotNull<ExportProvider>(sourceProvider, "sourceProvider");
			this._compositionOptions = compositionOptions;
			this._sourceProvider = sourceProvider;
			this._sourceProvider.ExportsChanging += this.OnExportsChanging;
			this._lock = new CompositionLock(compositionOptions.HasFlag(CompositionOptions.IsThreadSafe));
		}

		/// <summary>Previews all the required imports for the specified part to make sure that they can be satisfied, without actually setting them.</summary>
		/// <param name="part">The part to preview the imports of.</param>
		/// <param name="atomicComposition">The composition transaction to use, or <see langword="null" /> for no composition transaction.</param>
		public void PreviewImports(ComposablePart part, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ComposablePart>(part, "part");
			if (this._compositionOptions.HasFlag(CompositionOptions.DisableSilentRejection))
			{
				return;
			}
			IDisposable compositionLockHolder = this._lock.IsThreadSafe ? this._lock.LockComposition() : null;
			bool flag = compositionLockHolder != null;
			try
			{
				if (flag && atomicComposition != null)
				{
					atomicComposition.AddRevertAction(delegate
					{
						compositionLockHolder.Dispose();
					});
				}
				ImportEngine.PartManager partManager = this.GetPartManager(part, true);
				this.TryPreviewImportsStateMachine(partManager, part, atomicComposition).ThrowOnErrors(atomicComposition);
				this.StartSatisfyingImports(partManager, atomicComposition);
				if (flag && atomicComposition != null)
				{
					atomicComposition.AddCompleteAction(delegate
					{
						compositionLockHolder.Dispose();
					});
				}
			}
			finally
			{
				if (flag && atomicComposition == null)
				{
					compositionLockHolder.Dispose();
				}
			}
		}

		/// <summary>Satisfies the imports of the specified part.</summary>
		/// <param name="part">The part to satisfy the imports of.</param>
		public void SatisfyImports(ComposablePart part)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ComposablePart>(part, "part");
			ImportEngine.PartManager partManager = this.GetPartManager(part, true);
			if (partManager.State == ImportEngine.ImportState.Composed)
			{
				return;
			}
			using (this._lock.LockComposition())
			{
				this.TrySatisfyImports(partManager, part, true).ThrowOnErrors();
			}
		}

		/// <summary>Satisfies the imports of the specified part without registering them for recomposition.</summary>
		/// <param name="part">The part to satisfy the imports of.</param>
		public void SatisfyImportsOnce(ComposablePart part)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ComposablePart>(part, "part");
			ImportEngine.PartManager partManager = this.GetPartManager(part, true);
			if (partManager.State == ImportEngine.ImportState.Composed)
			{
				return;
			}
			using (this._lock.LockComposition())
			{
				this.TrySatisfyImports(partManager, part, false).ThrowOnErrors();
			}
		}

		/// <summary>Releases all the exports used to satisfy the imports of the specified part.</summary>
		/// <param name="part">The part to release the imports of.</param>
		/// <param name="atomicComposition">The composition transaction to use, or <see langword="null" /> for no composition transaction.</param>
		public void ReleaseImports(ComposablePart part, AtomicComposition atomicComposition)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ComposablePart>(part, "part");
			using (this._lock.LockComposition())
			{
				ImportEngine.PartManager partManager = this.GetPartManager(part, false);
				if (partManager != null)
				{
					this.StopSatisfyingImports(partManager, atomicComposition);
				}
			}
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ImportEngine" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Composition.Hosting.ImportEngine" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this._isDisposed)
			{
				bool flag = false;
				ExportProvider exportProvider = null;
				using (this._lock.LockStateForWrite())
				{
					if (!this._isDisposed)
					{
						exportProvider = this._sourceProvider;
						this._sourceProvider = null;
						this._recompositionManager = null;
						this._partManagers = null;
						this._isDisposed = true;
						flag = true;
					}
				}
				if (exportProvider != null)
				{
					exportProvider.ExportsChanging -= this.OnExportsChanging;
				}
				if (flag)
				{
					this._lock.Dispose();
				}
			}
		}

		private CompositionResult TryPreviewImportsStateMachine(ImportEngine.PartManager partManager, ComposablePart part, AtomicComposition atomicComposition)
		{
			CompositionResult result = CompositionResult.SucceededResult;
			if (partManager.State == ImportEngine.ImportState.ImportsPreviewing)
			{
				return new CompositionResult(new CompositionError[]
				{
					ErrorBuilder.CreatePartCycle(part)
				});
			}
			if (partManager.State == ImportEngine.ImportState.NoImportsSatisfied)
			{
				partManager.State = ImportEngine.ImportState.ImportsPreviewing;
				IEnumerable<ImportDefinition> imports = part.ImportDefinitions.Where(new Func<ImportDefinition, bool>(ImportEngine.IsRequiredImportForPreview));
				atomicComposition.AddRevertActionAllowNull(delegate
				{
					partManager.State = ImportEngine.ImportState.NoImportsSatisfied;
				});
				result = result.MergeResult(this.TrySatisfyImportSubset(partManager, imports, atomicComposition));
				if (!result.Succeeded)
				{
					partManager.State = ImportEngine.ImportState.NoImportsSatisfied;
					return result;
				}
				partManager.State = ImportEngine.ImportState.ImportsPreviewed;
			}
			return result;
		}

		private CompositionResult TrySatisfyImportsStateMachine(ImportEngine.PartManager partManager, ComposablePart part)
		{
			CompositionResult result = CompositionResult.SucceededResult;
			while (partManager.State < ImportEngine.ImportState.Composed)
			{
				ImportEngine.ImportState state = partManager.State;
				switch (partManager.State)
				{
				case ImportEngine.ImportState.NoImportsSatisfied:
				case ImportEngine.ImportState.ImportsPreviewed:
				{
					partManager.State = ImportEngine.ImportState.PreExportImportsSatisfying;
					IEnumerable<ImportDefinition> imports = from import in part.ImportDefinitions
					where import.IsPrerequisite
					select import;
					result = result.MergeResult(this.TrySatisfyImportSubset(partManager, imports, null));
					partManager.State = ImportEngine.ImportState.PreExportImportsSatisfied;
					break;
				}
				case ImportEngine.ImportState.ImportsPreviewing:
					return new CompositionResult(new CompositionError[]
					{
						ErrorBuilder.CreatePartCycle(part)
					});
				case ImportEngine.ImportState.PreExportImportsSatisfying:
				case ImportEngine.ImportState.PostExportImportsSatisfying:
					if (this.InPrerequisiteLoop())
					{
						return result.MergeError(ErrorBuilder.CreatePartCycle(part));
					}
					return result;
				case ImportEngine.ImportState.PreExportImportsSatisfied:
				{
					partManager.State = ImportEngine.ImportState.PostExportImportsSatisfying;
					IEnumerable<ImportDefinition> imports2 = from import in part.ImportDefinitions
					where !import.IsPrerequisite
					select import;
					result = result.MergeResult(this.TrySatisfyImportSubset(partManager, imports2, null));
					partManager.State = ImportEngine.ImportState.PostExportImportsSatisfied;
					break;
				}
				case ImportEngine.ImportState.PostExportImportsSatisfied:
					partManager.State = ImportEngine.ImportState.ComposedNotifying;
					partManager.ClearSavedImports();
					result = result.MergeResult(partManager.TryOnComposed());
					partManager.State = ImportEngine.ImportState.Composed;
					break;
				case ImportEngine.ImportState.ComposedNotifying:
					return result;
				}
				if (!result.Succeeded)
				{
					partManager.State = state;
					return result;
				}
			}
			return result;
		}

		private CompositionResult TrySatisfyImports(ImportEngine.PartManager partManager, ComposablePart part, bool shouldTrackImports)
		{
			Assumes.NotNull<ComposablePart>(part);
			CompositionResult result = CompositionResult.SucceededResult;
			if (partManager.State == ImportEngine.ImportState.Composed)
			{
				return result;
			}
			if (this._recursionStateStack.Count >= 100)
			{
				return result.MergeError(ErrorBuilder.ComposeTookTooManyIterations(100));
			}
			this._recursionStateStack.Push(partManager);
			try
			{
				result = result.MergeResult(this.TrySatisfyImportsStateMachine(partManager, part));
			}
			finally
			{
				this._recursionStateStack.Pop();
			}
			if (shouldTrackImports)
			{
				this.StartSatisfyingImports(partManager, null);
			}
			return result;
		}

		private CompositionResult TrySatisfyImportSubset(ImportEngine.PartManager partManager, IEnumerable<ImportDefinition> imports, AtomicComposition atomicComposition)
		{
			CompositionResult result = CompositionResult.SucceededResult;
			ComposablePart part = partManager.Part;
			foreach (ImportDefinition importDefinition in imports)
			{
				Export[] array = partManager.GetSavedImport(importDefinition);
				if (array == null)
				{
					CompositionResult<IEnumerable<Export>> compositionResult = ImportEngine.TryGetExports(this._sourceProvider, part, importDefinition, atomicComposition);
					if (!compositionResult.Succeeded)
					{
						result = result.MergeResult(compositionResult.ToResult());
						continue;
					}
					array = compositionResult.Value.AsArray<Export>();
				}
				if (atomicComposition == null)
				{
					result = result.MergeResult(partManager.TrySetImport(importDefinition, array));
				}
				else
				{
					partManager.SetSavedImport(importDefinition, array, atomicComposition);
				}
			}
			return result;
		}

		private void OnExportsChanging(object sender, ExportsChangeEventArgs e)
		{
			CompositionResult compositionResult = CompositionResult.SucceededResult;
			AtomicComposition atomicComposition = e.AtomicComposition;
			IEnumerable<ImportEngine.PartManager> enumerable = this._recompositionManager.GetAffectedParts(e.ChangedContractNames);
			ImportEngine.EngineContext engineContext;
			if (atomicComposition != null && atomicComposition.TryGetValue<ImportEngine.EngineContext>(this, out engineContext))
			{
				enumerable = enumerable.ConcatAllowingNull(engineContext.GetAddedPartManagers()).Except(engineContext.GetRemovedPartManagers());
			}
			IEnumerable<ExportDefinition> changedExports = e.AddedExports.ConcatAllowingNull(e.RemovedExports);
			foreach (ImportEngine.PartManager partManager in enumerable)
			{
				compositionResult = compositionResult.MergeResult(this.TryRecomposeImports(partManager, changedExports, atomicComposition));
			}
			compositionResult.ThrowOnErrors(atomicComposition);
		}

		private CompositionResult TryRecomposeImports(ImportEngine.PartManager partManager, IEnumerable<ExportDefinition> changedExports, AtomicComposition atomicComposition)
		{
			CompositionResult result = CompositionResult.SucceededResult;
			ImportEngine.ImportState state = partManager.State;
			if (state != ImportEngine.ImportState.ImportsPreviewed && state != ImportEngine.ImportState.Composed)
			{
				return new CompositionResult(new CompositionError[]
				{
					ErrorBuilder.InvalidStateForRecompposition(partManager.Part)
				});
			}
			IEnumerable<ImportDefinition> affectedImports = ImportEngine.RecompositionManager.GetAffectedImports(partManager.Part, changedExports);
			bool flag = partManager.State == ImportEngine.ImportState.Composed;
			bool flag2 = false;
			foreach (ImportDefinition import in affectedImports)
			{
				result = result.MergeResult(this.TryRecomposeImport(partManager, flag, import, atomicComposition));
				flag2 = true;
			}
			if (result.Succeeded && flag2 && flag)
			{
				if (atomicComposition == null)
				{
					result = result.MergeResult(partManager.TryOnComposed());
				}
				else
				{
					atomicComposition.AddCompleteAction(delegate
					{
						partManager.TryOnComposed().ThrowOnErrors();
					});
				}
			}
			return result;
		}

		private CompositionResult TryRecomposeImport(ImportEngine.PartManager partManager, bool partComposed, ImportDefinition import, AtomicComposition atomicComposition)
		{
			if (partComposed && !import.IsRecomposable)
			{
				return new CompositionResult(new CompositionError[]
				{
					ErrorBuilder.PreventedByExistingImport(partManager.Part, import)
				});
			}
			CompositionResult<IEnumerable<Export>> compositionResult = ImportEngine.TryGetExports(this._sourceProvider, partManager.Part, import, atomicComposition);
			if (!compositionResult.Succeeded)
			{
				return compositionResult.ToResult();
			}
			Export[] exports = compositionResult.Value.AsArray<Export>();
			if (partComposed)
			{
				if (atomicComposition == null)
				{
					return partManager.TrySetImport(import, exports);
				}
				atomicComposition.AddCompleteAction(delegate
				{
					partManager.TrySetImport(import, exports).ThrowOnErrors();
				});
			}
			else
			{
				partManager.SetSavedImport(import, exports, atomicComposition);
			}
			return CompositionResult.SucceededResult;
		}

		private void StartSatisfyingImports(ImportEngine.PartManager partManager, AtomicComposition atomicComposition)
		{
			if (atomicComposition == null)
			{
				if (!partManager.TrackingImports)
				{
					partManager.TrackingImports = true;
					this._recompositionManager.AddPartToIndex(partManager);
					return;
				}
			}
			else
			{
				this.GetEngineContext(atomicComposition).AddPartManager(partManager);
			}
		}

		private void StopSatisfyingImports(ImportEngine.PartManager partManager, AtomicComposition atomicComposition)
		{
			if (atomicComposition == null)
			{
				this._partManagers.Remove(partManager.Part);
				partManager.DisposeAllDependencies();
				if (partManager.TrackingImports)
				{
					partManager.TrackingImports = false;
					this._recompositionManager.AddPartToUnindex(partManager);
					return;
				}
			}
			else
			{
				this.GetEngineContext(atomicComposition).RemovePartManager(partManager);
			}
		}

		private ImportEngine.PartManager GetPartManager(ComposablePart part, bool createIfNotpresent)
		{
			ImportEngine.PartManager partManager = null;
			using (this._lock.LockStateForRead())
			{
				if (this._partManagers.TryGetValue(part, out partManager))
				{
					return partManager;
				}
			}
			if (createIfNotpresent)
			{
				using (this._lock.LockStateForWrite())
				{
					if (!this._partManagers.TryGetValue(part, out partManager))
					{
						partManager = new ImportEngine.PartManager(this, part);
						this._partManagers.Add(part, partManager);
					}
				}
			}
			return partManager;
		}

		private ImportEngine.EngineContext GetEngineContext(AtomicComposition atomicComposition)
		{
			Assumes.NotNull<AtomicComposition>(atomicComposition);
			ImportEngine.EngineContext engineContext;
			if (!atomicComposition.TryGetValue<ImportEngine.EngineContext>(this, true, out engineContext))
			{
				ImportEngine.EngineContext parentEngineContext;
				atomicComposition.TryGetValue<ImportEngine.EngineContext>(this, false, out parentEngineContext);
				engineContext = new ImportEngine.EngineContext(this, parentEngineContext);
				atomicComposition.SetValue(this, engineContext);
				atomicComposition.AddCompleteAction(new Action(engineContext.Complete));
			}
			return engineContext;
		}

		private bool InPrerequisiteLoop()
		{
			ImportEngine.PartManager partManager = this._recursionStateStack.First<ImportEngine.PartManager>();
			ImportEngine.PartManager partManager2 = null;
			foreach (ImportEngine.PartManager partManager3 in this._recursionStateStack.Skip(1))
			{
				if (partManager3.State == ImportEngine.ImportState.PreExportImportsSatisfying)
				{
					return true;
				}
				if (partManager3 == partManager)
				{
					partManager2 = partManager3;
					break;
				}
			}
			Assumes.IsTrue(partManager2 == partManager);
			return false;
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private static CompositionResult<IEnumerable<Export>> TryGetExports(ExportProvider provider, ComposablePart part, ImportDefinition definition, AtomicComposition atomicComposition)
		{
			CompositionResult<IEnumerable<Export>> result;
			try
			{
				result = new CompositionResult<IEnumerable<Export>>(provider.GetExports(definition, atomicComposition).AsArray<Export>());
			}
			catch (ImportCardinalityMismatchException exception)
			{
				CompositionException innerException = new CompositionException(ErrorBuilder.CreateImportCardinalityMismatch(exception, definition));
				result = new CompositionResult<IEnumerable<Export>>(new CompositionError[]
				{
					ErrorBuilder.CreatePartCannotSetImport(part, definition, innerException)
				});
			}
			return result;
		}

		internal static bool IsRequiredImportForPreview(ImportDefinition import)
		{
			return import.Cardinality == ImportCardinality.ExactlyOne;
		}

		private const int MaximumNumberOfCompositionIterations = 100;

		private volatile bool _isDisposed;

		private ExportProvider _sourceProvider;

		private Stack<ImportEngine.PartManager> _recursionStateStack = new Stack<ImportEngine.PartManager>();

		private ConditionalWeakTable<ComposablePart, ImportEngine.PartManager> _partManagers = new ConditionalWeakTable<ComposablePart, ImportEngine.PartManager>();

		private ImportEngine.RecompositionManager _recompositionManager = new ImportEngine.RecompositionManager();

		private readonly CompositionLock _lock;

		private readonly CompositionOptions _compositionOptions;

		private class EngineContext
		{
			public EngineContext(ImportEngine importEngine, ImportEngine.EngineContext parentEngineContext)
			{
				this._importEngine = importEngine;
				this._parentEngineContext = parentEngineContext;
			}

			public void AddPartManager(ImportEngine.PartManager part)
			{
				Assumes.NotNull<ImportEngine.PartManager>(part);
				if (!this._removedPartManagers.Remove(part))
				{
					this._addedPartManagers.Add(part);
				}
			}

			public void RemovePartManager(ImportEngine.PartManager part)
			{
				Assumes.NotNull<ImportEngine.PartManager>(part);
				if (!this._addedPartManagers.Remove(part))
				{
					this._removedPartManagers.Add(part);
				}
			}

			public IEnumerable<ImportEngine.PartManager> GetAddedPartManagers()
			{
				if (this._parentEngineContext != null)
				{
					return this._addedPartManagers.ConcatAllowingNull(this._parentEngineContext.GetAddedPartManagers());
				}
				return this._addedPartManagers;
			}

			public IEnumerable<ImportEngine.PartManager> GetRemovedPartManagers()
			{
				if (this._parentEngineContext != null)
				{
					return this._removedPartManagers.ConcatAllowingNull(this._parentEngineContext.GetRemovedPartManagers());
				}
				return this._removedPartManagers;
			}

			public void Complete()
			{
				foreach (ImportEngine.PartManager partManager in this._addedPartManagers)
				{
					this._importEngine.StartSatisfyingImports(partManager, null);
				}
				foreach (ImportEngine.PartManager partManager2 in this._removedPartManagers)
				{
					this._importEngine.StopSatisfyingImports(partManager2, null);
				}
			}

			private ImportEngine _importEngine;

			private List<ImportEngine.PartManager> _addedPartManagers = new List<ImportEngine.PartManager>();

			private List<ImportEngine.PartManager> _removedPartManagers = new List<ImportEngine.PartManager>();

			private ImportEngine.EngineContext _parentEngineContext;
		}

		private class PartManager
		{
			public PartManager(ImportEngine importEngine, ComposablePart part)
			{
				this._importEngine = importEngine;
				this._part = part;
			}

			public ComposablePart Part
			{
				get
				{
					return this._part;
				}
			}

			public ImportEngine.ImportState State
			{
				get
				{
					ImportEngine.ImportState state;
					using (this._importEngine._lock.LockStateForRead())
					{
						state = this._state;
					}
					return state;
				}
				set
				{
					using (this._importEngine._lock.LockStateForWrite())
					{
						this._state = value;
					}
				}
			}

			public bool TrackingImports { get; set; }

			public IEnumerable<string> GetImportedContractNames()
			{
				if (this.Part == null)
				{
					return Enumerable.Empty<string>();
				}
				if (this._importedContractNames == null)
				{
					this._importedContractNames = (from import in this.Part.ImportDefinitions
					select import.ContractName ?? ImportDefinition.EmptyContractName).Distinct<string>().ToArray<string>();
				}
				return this._importedContractNames;
			}

			public CompositionResult TrySetImport(ImportDefinition import, IEnumerable<Export> exports)
			{
				CompositionResult result;
				try
				{
					this.Part.SetImport(import, exports);
					this.UpdateDisposableDependencies(import, exports);
					result = CompositionResult.SucceededResult;
				}
				catch (CompositionException innerException)
				{
					result = new CompositionResult(new CompositionError[]
					{
						ErrorBuilder.CreatePartCannotSetImport(this.Part, import, innerException)
					});
				}
				catch (ComposablePartException innerException2)
				{
					result = new CompositionResult(new CompositionError[]
					{
						ErrorBuilder.CreatePartCannotSetImport(this.Part, import, innerException2)
					});
				}
				return result;
			}

			public void SetSavedImport(ImportDefinition import, Export[] exports, AtomicComposition atomicComposition)
			{
				if (atomicComposition != null)
				{
					Export[] savedExports = this.GetSavedImport(import);
					atomicComposition.AddRevertAction(delegate
					{
						this.SetSavedImport(import, savedExports, null);
					});
				}
				if (this._importCache == null)
				{
					this._importCache = new Dictionary<ImportDefinition, Export[]>();
				}
				this._importCache[import] = exports;
			}

			public Export[] GetSavedImport(ImportDefinition import)
			{
				Export[] result = null;
				if (this._importCache != null)
				{
					this._importCache.TryGetValue(import, out result);
				}
				return result;
			}

			public void ClearSavedImports()
			{
				this._importCache = null;
			}

			public CompositionResult TryOnComposed()
			{
				CompositionResult result;
				try
				{
					this.Part.Activate();
					result = CompositionResult.SucceededResult;
				}
				catch (ComposablePartException innerException)
				{
					result = new CompositionResult(new CompositionError[]
					{
						ErrorBuilder.CreatePartCannotActivate(this.Part, innerException)
					});
				}
				return result;
			}

			public void UpdateDisposableDependencies(ImportDefinition import, IEnumerable<Export> exports)
			{
				List<IDisposable> list = null;
				foreach (IDisposable item in exports.OfType<IDisposable>())
				{
					if (list == null)
					{
						list = new List<IDisposable>();
					}
					list.Add(item);
				}
				List<IDisposable> list2 = null;
				if (this._importedDisposableExports != null && this._importedDisposableExports.TryGetValue(import, out list2))
				{
					list2.ForEach(delegate(IDisposable disposable)
					{
						disposable.Dispose();
					});
					if (list == null)
					{
						this._importedDisposableExports.Remove(import);
						if (!this._importedDisposableExports.FastAny<KeyValuePair<ImportDefinition, List<IDisposable>>>())
						{
							this._importedDisposableExports = null;
						}
						return;
					}
				}
				if (list != null)
				{
					if (this._importedDisposableExports == null)
					{
						this._importedDisposableExports = new Dictionary<ImportDefinition, List<IDisposable>>();
					}
					this._importedDisposableExports[import] = list;
				}
			}

			public void DisposeAllDependencies()
			{
				if (this._importedDisposableExports != null)
				{
					IEnumerable<IDisposable> source = this._importedDisposableExports.Values.SelectMany((List<IDisposable> exports) => exports);
					this._importedDisposableExports = null;
					source.ForEach(delegate(IDisposable disposableExport)
					{
						disposableExport.Dispose();
					});
				}
			}

			private Dictionary<ImportDefinition, List<IDisposable>> _importedDisposableExports;

			private Dictionary<ImportDefinition, Export[]> _importCache;

			private string[] _importedContractNames;

			private ComposablePart _part;

			private ImportEngine.ImportState _state;

			private readonly ImportEngine _importEngine;
		}

		private class RecompositionManager
		{
			public void AddPartToIndex(ImportEngine.PartManager partManager)
			{
				this._partsToIndex.Add(partManager);
			}

			public void AddPartToUnindex(ImportEngine.PartManager partManager)
			{
				this._partsToUnindex.Add(partManager);
			}

			public IEnumerable<ImportEngine.PartManager> GetAffectedParts(IEnumerable<string> changedContractNames)
			{
				this.UpdateImportIndex();
				List<ImportEngine.PartManager> list = new List<ImportEngine.PartManager>();
				list.AddRange(this.GetPartsImporting(ImportDefinition.EmptyContractName));
				foreach (string contractName in changedContractNames)
				{
					list.AddRange(this.GetPartsImporting(contractName));
				}
				return list;
			}

			public static IEnumerable<ImportDefinition> GetAffectedImports(ComposablePart part, IEnumerable<ExportDefinition> changedExports)
			{
				return from import in part.ImportDefinitions
				where ImportEngine.RecompositionManager.IsAffectedImport(import, changedExports)
				select import;
			}

			private static bool IsAffectedImport(ImportDefinition import, IEnumerable<ExportDefinition> changedExports)
			{
				foreach (ExportDefinition exportDefinition in changedExports)
				{
					if (import.IsConstraintSatisfiedBy(exportDefinition))
					{
						return true;
					}
				}
				return false;
			}

			public IEnumerable<ImportEngine.PartManager> GetPartsImporting(string contractName)
			{
				WeakReferenceCollection<ImportEngine.PartManager> weakReferenceCollection;
				if (!this._partManagerIndex.TryGetValue(contractName, out weakReferenceCollection))
				{
					return Enumerable.Empty<ImportEngine.PartManager>();
				}
				return weakReferenceCollection.AliveItemsToList();
			}

			private void AddIndexEntries(ImportEngine.PartManager partManager)
			{
				foreach (string key in partManager.GetImportedContractNames())
				{
					WeakReferenceCollection<ImportEngine.PartManager> weakReferenceCollection;
					if (!this._partManagerIndex.TryGetValue(key, out weakReferenceCollection))
					{
						weakReferenceCollection = new WeakReferenceCollection<ImportEngine.PartManager>();
						this._partManagerIndex.Add(key, weakReferenceCollection);
					}
					if (!weakReferenceCollection.Contains(partManager))
					{
						weakReferenceCollection.Add(partManager);
					}
				}
			}

			private void RemoveIndexEntries(ImportEngine.PartManager partManager)
			{
				foreach (string key in partManager.GetImportedContractNames())
				{
					WeakReferenceCollection<ImportEngine.PartManager> weakReferenceCollection;
					if (this._partManagerIndex.TryGetValue(key, out weakReferenceCollection))
					{
						weakReferenceCollection.Remove(partManager);
						if (weakReferenceCollection.AliveItemsToList().Count == 0)
						{
							this._partManagerIndex.Remove(key);
						}
					}
				}
			}

			private void UpdateImportIndex()
			{
				List<ImportEngine.PartManager> list = this._partsToIndex.AliveItemsToList();
				this._partsToIndex.Clear();
				List<ImportEngine.PartManager> list2 = this._partsToUnindex.AliveItemsToList();
				this._partsToUnindex.Clear();
				if (list.Count == 0 && list2.Count == 0)
				{
					return;
				}
				foreach (ImportEngine.PartManager partManager in list)
				{
					int num = list2.IndexOf(partManager);
					if (num >= 0)
					{
						list2[num] = null;
					}
					else
					{
						this.AddIndexEntries(partManager);
					}
				}
				foreach (ImportEngine.PartManager partManager2 in list2)
				{
					if (partManager2 != null)
					{
						this.RemoveIndexEntries(partManager2);
					}
				}
			}

			private WeakReferenceCollection<ImportEngine.PartManager> _partsToIndex = new WeakReferenceCollection<ImportEngine.PartManager>();

			private WeakReferenceCollection<ImportEngine.PartManager> _partsToUnindex = new WeakReferenceCollection<ImportEngine.PartManager>();

			private Dictionary<string, WeakReferenceCollection<ImportEngine.PartManager>> _partManagerIndex = new Dictionary<string, WeakReferenceCollection<ImportEngine.PartManager>>();
		}

		private enum ImportState
		{
			NoImportsSatisfied,
			ImportsPreviewing,
			ImportsPreviewed,
			PreExportImportsSatisfying,
			PreExportImportsSatisfied,
			PostExportImportsSatisfying,
			PostExportImportsSatisfied,
			ComposedNotifying,
			Composed
		}
	}
}
