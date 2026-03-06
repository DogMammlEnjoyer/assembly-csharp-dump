using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	/// <summary>Represents a node in a tree of scoped catalogs, reflecting an underlying catalog and its child scopes.</summary>
	[DebuggerTypeProxy(typeof(CompositionScopeDefinitionDebuggerProxy))]
	public class CompositionScopeDefinition : ComposablePartCatalog, INotifyComposablePartCatalogChanged
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionScopeDefinition" /> class.</summary>
		protected CompositionScopeDefinition()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionScopeDefinition" /> class with the specified underlying catalog and children.</summary>
		/// <param name="catalog">The underlying catalog for this catalog.</param>
		/// <param name="children">A collection of the child scopes of this catalog.</param>
		public CompositionScopeDefinition(ComposablePartCatalog catalog, IEnumerable<CompositionScopeDefinition> children)
		{
			Requires.NotNull<ComposablePartCatalog>(catalog, "catalog");
			Requires.NullOrNotNullElements<CompositionScopeDefinition>(children, "children");
			this.InitializeCompositionScopeDefinition(catalog, children, null);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionScopeDefinition" /> class with the specified underlying catalog, children, and public surface.</summary>
		/// <param name="catalog">The underlying catalog for this catalog.</param>
		/// <param name="children">A collection of the child scopes of this catalog.</param>
		/// <param name="publicSurface">The public surface for this catalog.</param>
		public CompositionScopeDefinition(ComposablePartCatalog catalog, IEnumerable<CompositionScopeDefinition> children, IEnumerable<ExportDefinition> publicSurface)
		{
			Requires.NotNull<ComposablePartCatalog>(catalog, "catalog");
			Requires.NullOrNotNullElements<CompositionScopeDefinition>(children, "children");
			Requires.NullOrNotNullElements<ExportDefinition>(publicSurface, "publicSurface");
			this.InitializeCompositionScopeDefinition(catalog, children, publicSurface);
		}

		private void InitializeCompositionScopeDefinition(ComposablePartCatalog catalog, IEnumerable<CompositionScopeDefinition> children, IEnumerable<ExportDefinition> publicSurface)
		{
			this._catalog = catalog;
			if (children != null)
			{
				this._children = children.ToArray<CompositionScopeDefinition>();
			}
			if (publicSurface != null)
			{
				this._publicSurface = publicSurface;
			}
			INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = this._catalog as INotifyComposablePartCatalogChanged;
			if (notifyComposablePartCatalogChanged != null)
			{
				notifyComposablePartCatalogChanged.Changed += this.OnChangedInternal;
				notifyComposablePartCatalogChanged.Changing += this.OnChangingInternal;
			}
		}

		/// <summary>Called by the <see langword="Dispose()" /> and <see langword="Finalize()" /> methods to release the managed and unmanaged resources used by the current instance of the <see cref="T:System.ComponentModel.Composition.Hosting.CompositionScopeDefinition" /> class.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
				{
					INotifyComposablePartCatalogChanged notifyComposablePartCatalogChanged = this._catalog as INotifyComposablePartCatalogChanged;
					if (notifyComposablePartCatalogChanged != null)
					{
						notifyComposablePartCatalogChanged.Changed -= this.OnChangedInternal;
						notifyComposablePartCatalogChanged.Changing -= this.OnChangingInternal;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>Gets the child scopes of this catalog.</summary>
		/// <returns>A collection of the child scopes of this catalog.</returns>
		public virtual IEnumerable<CompositionScopeDefinition> Children
		{
			get
			{
				this.ThrowIfDisposed();
				return this._children;
			}
		}

		/// <summary>Gets a collection of parts visible to the parent scope of this catalog.</summary>
		/// <returns>A collection of parts visible to the parent scope of this catalog.</returns>
		public virtual IEnumerable<ExportDefinition> PublicSurface
		{
			get
			{
				this.ThrowIfDisposed();
				if (this._publicSurface == null)
				{
					return this.SelectMany((ComposablePartDefinition p) => p.ExportDefinitions);
				}
				return this._publicSurface;
			}
		}

		/// <summary>Returns an enumerator that iterates through the catalog.</summary>
		/// <returns>An enumerator that can be used to iterate through the catalog.</returns>
		public override IEnumerator<ComposablePartDefinition> GetEnumerator()
		{
			return this._catalog.GetEnumerator();
		}

		/// <summary>Gets a collection of exports that match the conditions specified by the import definition.</summary>
		/// <param name="definition">Conditions that specify which exports to match.</param>
		/// <returns>A collection of exports that match the specified conditions.</returns>
		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			this.ThrowIfDisposed();
			return this._catalog.GetExports(definition);
		}

		internal IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExportsFromPublicSurface(ImportDefinition definition)
		{
			Assumes.NotNull<ImportDefinition, string>(definition, "definition");
			List<Tuple<ComposablePartDefinition, ExportDefinition>> list = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
			foreach (ExportDefinition exportDefinition in this.PublicSurface)
			{
				if (definition.IsConstraintSatisfiedBy(exportDefinition))
				{
					foreach (Tuple<ComposablePartDefinition, ExportDefinition> tuple in this.GetExports(definition))
					{
						if (tuple.Item2 == exportDefinition)
						{
							list.Add(tuple);
							break;
						}
					}
				}
			}
			return list;
		}

		/// <summary>Occurs when the underlying catalog has changed, if that catalog supports notifications.</summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

		/// <summary>Occurs when the underlying catalog is changing, if that catalog supports notifications.</summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

		/// <summary>Raises the <see cref="E:System.ComponentModel.Composition.Hosting.CompositionScopeDefinition.Changed" /> event.</summary>
		/// <param name="e">Contains data for the <see cref="E:System.ComponentModel.Composition.Hosting.CompositionScopeDefinition.Changed" /> event.</param>
		protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changed = this.Changed;
			if (changed != null)
			{
				changed(this, e);
			}
		}

		/// <summary>Raises the <see cref="E:System.ComponentModel.Composition.Hosting.CompositionScopeDefinition.Changing" /> event.</summary>
		/// <param name="e">Contains data for the <see cref="E:System.ComponentModel.Composition.Hosting.CompositionScopeDefinition.Changing" /> event.</param>
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
			this.OnChanged(e);
		}

		private void OnChangingInternal(object sender, ComposablePartCatalogChangeEventArgs e)
		{
			this.OnChanging(e);
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed == 1)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private ComposablePartCatalog _catalog;

		private IEnumerable<ExportDefinition> _publicSurface;

		private IEnumerable<CompositionScopeDefinition> _children = Enumerable.Empty<CompositionScopeDefinition>();

		private volatile int _isDisposed;
	}
}
