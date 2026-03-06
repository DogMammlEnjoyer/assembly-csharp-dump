using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	/// <summary>Discovers attributed parts in the dynamic link library (DLL) and EXE files in an application's directory and path.</summary>
	public class ApplicationCatalog : ComposablePartCatalog, ICompositionElement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ApplicationCatalog" /> class.</summary>
		public ApplicationCatalog()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ApplicationCatalog" /> class by using the specified source for parts.</summary>
		/// <param name="definitionOrigin">The element used by diagnostics to identify the source for parts.</param>
		public ApplicationCatalog(ICompositionElement definitionOrigin)
		{
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this._definitionOrigin = definitionOrigin;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ApplicationCatalog" /> class by using the specified reflection context.</summary>
		/// <param name="reflectionContext">The reflection context.</param>
		public ApplicationCatalog(ReflectionContext reflectionContext)
		{
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			this._reflectionContext = reflectionContext;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.ApplicationCatalog" /> class by using the specified reflection context and source for parts.</summary>
		/// <param name="reflectionContext">The reflection context.</param>
		/// <param name="definitionOrigin">The element used by diagnostics to identify the source for parts.</param>
		public ApplicationCatalog(ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
		{
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this._reflectionContext = reflectionContext;
			this._definitionOrigin = definitionOrigin;
		}

		internal ComposablePartCatalog CreateCatalog(string location, string pattern)
		{
			if (this._reflectionContext != null)
			{
				if (this._definitionOrigin == null)
				{
					return new DirectoryCatalog(location, pattern, this._reflectionContext);
				}
				return new DirectoryCatalog(location, pattern, this._reflectionContext, this._definitionOrigin);
			}
			else
			{
				if (this._definitionOrigin == null)
				{
					return new DirectoryCatalog(location, pattern);
				}
				return new DirectoryCatalog(location, pattern, this._definitionOrigin);
			}
		}

		private AggregateCatalog InnerCatalog
		{
			get
			{
				if (this._innerCatalog == null)
				{
					object thisLock = this._thisLock;
					lock (thisLock)
					{
						if (this._innerCatalog == null)
						{
							string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
							Assumes.NotNull<string>(baseDirectory);
							List<ComposablePartCatalog> list = new List<ComposablePartCatalog>();
							list.Add(this.CreateCatalog(baseDirectory, "*.exe"));
							list.Add(this.CreateCatalog(baseDirectory, "*.dll"));
							string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
							if (!string.IsNullOrEmpty(relativeSearchPath))
							{
								foreach (string path in relativeSearchPath.Split(new char[]
								{
									';'
								}, StringSplitOptions.RemoveEmptyEntries))
								{
									string text = Path.Combine(baseDirectory, path);
									if (Directory.Exists(text))
									{
										list.Add(this.CreateCatalog(text, "*.dll"));
									}
								}
							}
							AggregateCatalog innerCatalog = new AggregateCatalog(list);
							this._innerCatalog = innerCatalog;
						}
					}
				}
				return this._innerCatalog;
			}
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this._isDisposed)
				{
					IDisposable disposable = null;
					object thisLock = this._thisLock;
					lock (thisLock)
					{
						disposable = this._innerCatalog;
						this._innerCatalog = null;
						this._isDisposed = true;
					}
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public override IEnumerator<ComposablePartDefinition> GetEnumerator()
		{
			this.ThrowIfDisposed();
			return this.InnerCatalog.GetEnumerator();
		}

		/// <summary>Gets the export definitions that match the constraint expressed by the specified import definition.</summary>
		/// <param name="definition">The conditions of the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition" /> objects to be returned.</param>
		/// <returns>A collection of objects that contain the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition" /> objects and their associated <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects that match the specified constraint.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> object has been disposed of.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="definition" /> is <see langword="null" />.</exception>
		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ImportDefinition>(definition, "definition");
			return this.InnerCatalog.GetExports(definition);
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private string GetDisplayName()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} (Path=\"{1}\") (PrivateProbingPath=\"{2}\")", base.GetType().Name, AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath);
		}

		/// <summary>Retrieves a string representation of the application catalog.</summary>
		/// <returns>A string representation of the catalog.</returns>
		public override string ToString()
		{
			return this.GetDisplayName();
		}

		/// <summary>Gets the display name of the application catalog.</summary>
		/// <returns>A string that contains a human-readable display name of the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> object.</returns>
		string ICompositionElement.DisplayName
		{
			get
			{
				return this.GetDisplayName();
			}
		}

		/// <summary>Gets the composition element from which the application catalog originated.</summary>
		/// <returns>Always <see langword="null" />.</returns>
		ICompositionElement ICompositionElement.Origin
		{
			get
			{
				return null;
			}
		}

		private bool _isDisposed;

		private volatile AggregateCatalog _innerCatalog;

		private readonly object _thisLock = new object();

		private ICompositionElement _definitionOrigin;

		private ReflectionContext _reflectionContext;
	}
}
