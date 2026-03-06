using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Primitives
{
	/// <summary>Represents the abstract base class for composable part catalogs, which collect and return <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects.</summary>
	[DebuggerTypeProxy(typeof(ComposablePartCatalogDebuggerProxy))]
	public abstract class ComposablePartCatalog : IEnumerable<ComposablePartDefinition>, IEnumerable, IDisposable
	{
		/// <summary>Gets the part definitions that are contained in the catalog.</summary>
		/// <returns>The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> contained in the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog" /> object has been disposed of.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual IQueryable<ComposablePartDefinition> Parts
		{
			get
			{
				this.ThrowIfDisposed();
				if (this._queryableParts == null)
				{
					IQueryable<ComposablePartDefinition> value = this.AsQueryable<ComposablePartDefinition>();
					Interlocked.CompareExchange<IQueryable<ComposablePartDefinition>>(ref this._queryableParts, value, null);
					Assumes.NotNull<IQueryable<ComposablePartDefinition>>(this._queryableParts);
				}
				return this._queryableParts;
			}
		}

		/// <summary>Gets a list of export definitions that match the constraint defined by the specified <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition" /> object.</summary>
		/// <param name="definition">The conditions of the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition" /> objects to be returned.</param>
		/// <returns>A collection of <see cref="T:System.Tuple`2" /> containing the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition" /> objects and their associated <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects for objects that match the constraint specified by <paramref name="definition" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog" /> object has been disposed of.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="definition" /> is <see langword="null" />.</exception>
		public virtual IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ImportDefinition>(definition, "definition");
			List<Tuple<ComposablePartDefinition, ExportDefinition>> list = null;
			IEnumerable<ComposablePartDefinition> candidateParts = this.GetCandidateParts(definition);
			if (candidateParts != null)
			{
				foreach (ComposablePartDefinition composablePartDefinition in candidateParts)
				{
					IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports = composablePartDefinition.GetExports(definition);
					if (exports != ComposablePartDefinition._EmptyExports)
					{
						list = list.FastAppendToListAllowNulls(exports);
					}
				}
			}
			return list ?? ComposablePartCatalog._EmptyExportsList;
		}

		internal virtual IEnumerable<ComposablePartDefinition> GetCandidateParts(ImportDefinition definition)
		{
			return this;
		}

		/// <summary>Releases all resources used by the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			this._isDisposed = true;
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		/// <summary>Returns an enumerator that iterates through the catalog.</summary>
		/// <returns>An enumerator that can be used to iterate through the catalog.</returns>
		public virtual IEnumerator<ComposablePartDefinition> GetEnumerator()
		{
			IQueryable<ComposablePartDefinition> parts = this.Parts;
			if (parts == this._queryableParts)
			{
				return Enumerable.Empty<ComposablePartDefinition>().GetEnumerator();
			}
			return parts.GetEnumerator();
		}

		/// <summary>Returns an enumerator that iterates through the catalog.</summary>
		/// <returns>An enumerator that can be used to iterate through the catalog.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private bool _isDisposed;

		private volatile IQueryable<ComposablePartDefinition> _queryableParts;

		private static readonly List<Tuple<ComposablePartDefinition, ExportDefinition>> _EmptyExportsList = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
	}
}
