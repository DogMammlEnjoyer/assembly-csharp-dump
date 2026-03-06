using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Diagnostics;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	/// <summary>Discovers attributed parts in the assemblies in a specified directory.</summary>
	[DebuggerTypeProxy(typeof(DirectoryCatalog.DirectoryCatalogDebuggerProxy))]
	public class DirectoryCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged, ICompositionElement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> class by using <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects based on all the DLL files in the specified directory path.</summary>
		/// <param name="path">The path to the directory to scan for assemblies to add to the catalog.  
		///  The path must be absolute or relative to <see cref="P:System.AppDomain.BaseDirectory" />.</param>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified <paramref name="path" /> is invalid (for example, it is on an unmapped drive).</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more implementation-specific invalid characters.</exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified <paramref name="path" />, file name, or both exceed the system-defined maximum length.</exception>
		public DirectoryCatalog(string path) : this(path, "*.dll")
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> class by using <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects based on all the DLL files in the specified directory path, in the specified reflection context.</summary>
		/// <param name="path">The path to the directory to scan for assemblies to add to the catalog.  
		///  The path must be absolute or relative to <see cref="P:System.AppDomain.BaseDirectory" />.</param>
		/// <param name="reflectionContext">The context used to create parts.</param>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified <paramref name="path" /> is invalid (for example, it is on an unmapped drive).</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more implementation-specific invalid characters.</exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified <paramref name="path" />, file name, or both exceed the system-defined maximum length.</exception>
		public DirectoryCatalog(string path, ReflectionContext reflectionContext) : this(path, "*.dll", reflectionContext)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> class by using <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects based on all the DLL files in the specified directory path with the specified source for parts.</summary>
		/// <param name="path">The path to the directory to scan for assemblies to add to the catalog.  
		///  The path must be absolute or relative to <see cref="P:System.AppDomain.BaseDirectory" />.</param>
		/// <param name="definitionOrigin">The element used by diagnostics to identify the source for parts.</param>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified <paramref name="path" /> is invalid (for example, it is on an unmapped drive).</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more implementation-specific invalid characters.</exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified <paramref name="path" />, file name, or both exceed the system-defined maximum length.</exception>
		public DirectoryCatalog(string path, ICompositionElement definitionOrigin) : this(path, "*.dll", definitionOrigin)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> class by  using <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects based on all the DLL files in the specified directory path, in the specified reflection context.</summary>
		/// <param name="path">The path to the directory to scan for assemblies to add to the catalog.  
		///  The path must be absolute or relative to <see cref="P:System.AppDomain.BaseDirectory" />.</param>
		/// <param name="reflectionContext">The context used to create parts.</param>
		/// <param name="definitionOrigin">The element used by diagnostics to identify the source for parts.</param>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified <paramref name="path" /> is invalid (for example, it is on an unmapped drive).</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more implementation-specific invalid characters.</exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified <paramref name="path" />, file name, or both exceed the system-defined maximum length.</exception>
		public DirectoryCatalog(string path, ReflectionContext reflectionContext, ICompositionElement definitionOrigin) : this(path, "*.dll", reflectionContext, definitionOrigin)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> class by using <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects that match a specified search pattern in the specified directory path.</summary>
		/// <param name="path">The path to the directory to scan for assemblies to add to the catalog.  
		///  The path must be absolute or relative to <see cref="P:System.AppDomain.BaseDirectory" />.</param>
		/// <param name="searchPattern">The search string. The format of the string should be the same as specified for the <see cref="M:System.IO.Directory.GetFiles(System.String,System.String)" /> method.</param>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified <paramref name="path" /> is invalid (for example, it is on an unmapped drive).</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> or <paramref name="searchPattern" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more implementation-specific invalid characters.  
		/// -or-  
		/// <paramref name="searchPattern" /> does not contain a valid pattern.</exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified <paramref name="path" />, file name, or both exceed the system-defined maximum length.</exception>
		public DirectoryCatalog(string path, string searchPattern)
		{
			this._thisLock = new Lock();
			base..ctor();
			Requires.NotNullOrEmpty(path, "path");
			Requires.NotNullOrEmpty(searchPattern, "searchPattern");
			this._definitionOrigin = this;
			this.Initialize(path, searchPattern);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> class by using <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects based on the specified search pattern in the specified directory path with the specified source for parts.</summary>
		/// <param name="path">The path to the directory to scan for assemblies to add to the catalog.  
		///  The path must be absolute or relative to <see cref="P:System.AppDomain.BaseDirectory" />.</param>
		/// <param name="searchPattern">The search string. The format of the string should be the same as specified for the <see cref="M:System.IO.Directory.GetFiles(System.String,System.String)" /> method.</param>
		/// <param name="definitionOrigin">The element used by diagnostics to identify the source for parts.</param>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified <paramref name="path" /> is invalid (for example, it is on an unmapped drive).</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> or <paramref name="searchPattern" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more implementation-specific invalid characters.  
		/// -or-  
		/// <paramref name="searchPattern" /> does not contain a valid pattern.</exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified <paramref name="path" />, file name, or both exceed the system-defined maximum length.</exception>
		public DirectoryCatalog(string path, string searchPattern, ICompositionElement definitionOrigin)
		{
			this._thisLock = new Lock();
			base..ctor();
			Requires.NotNullOrEmpty(path, "path");
			Requires.NotNullOrEmpty(searchPattern, "searchPattern");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this._definitionOrigin = definitionOrigin;
			this.Initialize(path, searchPattern);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> class by using <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects based on the specified search pattern in the specified directory path, using the specified reflection context.</summary>
		/// <param name="path">The path to the directory to scan for assemblies to add to the catalog.  
		///  The path must be absolute or relative to <see cref="P:System.AppDomain.BaseDirectory" />.</param>
		/// <param name="searchPattern">The search string. The format of the string should be the same as specified for the <see cref="M:System.IO.Directory.GetFiles(System.String,System.String)" /> method.</param>
		/// <param name="reflectionContext">The context used to create parts.</param>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified <paramref name="path" /> is invalid (for example, it is on an unmapped drive).</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> or <paramref name="searchPattern" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more implementation-specific invalid characters.  
		/// -or-  
		/// <paramref name="searchPattern" /> does not contain a valid pattern.</exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified <paramref name="path" />, file name, or both exceed the system-defined maximum length.</exception>
		public DirectoryCatalog(string path, string searchPattern, ReflectionContext reflectionContext)
		{
			this._thisLock = new Lock();
			base..ctor();
			Requires.NotNullOrEmpty(path, "path");
			Requires.NotNullOrEmpty(searchPattern, "searchPattern");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			this._reflectionContext = reflectionContext;
			this._definitionOrigin = this;
			this.Initialize(path, searchPattern);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> class by using <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects based on the specified search pattern in the specified directory path, using the specified reflection context.</summary>
		/// <param name="path">The path to the directory to scan for assemblies to add to the catalog.  
		///  The path must be absolute or relative to <see cref="P:System.AppDomain.BaseDirectory" />.</param>
		/// <param name="searchPattern">The search string. The format of the string should be the same as specified for the <see cref="M:System.IO.Directory.GetFiles(System.String,System.String)" /> method.</param>
		/// <param name="reflectionContext">The context used to create parts.</param>
		/// <param name="definitionOrigin">The element used by diagnostics to identify the source for parts.</param>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified <paramref name="path" /> is invalid (for example, it is on an unmapped drive).</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> or <paramref name="searchPattern" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more implementation-specific invalid characters.  
		/// -or-  
		/// <paramref name="searchPattern" /> does not contain a valid pattern.</exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified <paramref name="path" />, file name, or both exceed the system-defined maximum length.</exception>
		public DirectoryCatalog(string path, string searchPattern, ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
		{
			this._thisLock = new Lock();
			base..ctor();
			Requires.NotNullOrEmpty(path, "path");
			Requires.NotNullOrEmpty(searchPattern, "searchPattern");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this._reflectionContext = reflectionContext;
			this._definitionOrigin = definitionOrigin;
			this.Initialize(path, searchPattern);
		}

		/// <summary>Gets the translated absolute path observed by the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> object.</summary>
		/// <returns>The translated absolute path observed by the catalog.</returns>
		public string FullPath
		{
			get
			{
				return this._fullPath;
			}
		}

		/// <summary>Gets the collection of files currently loaded in the catalog.</summary>
		/// <returns>A collection of files currently loaded in the catalog.</returns>
		public ReadOnlyCollection<string> LoadedFiles
		{
			get
			{
				ReadOnlyCollection<string> loadedFiles;
				using (new ReadLock(this._thisLock))
				{
					loadedFiles = this._loadedFiles;
				}
				return loadedFiles;
			}
		}

		/// <summary>Gets the path observed by the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> object.</summary>
		/// <returns>The path observed by the catalog.</returns>
		public string Path
		{
			get
			{
				return this._path;
			}
		}

		/// <summary>Gets the search pattern that is passed into the constructor of the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> object.</summary>
		/// <returns>The search pattern the catalog uses to find files. The default is *.dll, which returns all DLL files.</returns>
		public string SearchPattern
		{
			get
			{
				return this._searchPattern;
			}
		}

		/// <summary>Occurs when the contents of the catalog has changed.</summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

		/// <summary>Occurs when the catalog is changing.</summary>
		public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && !this._isDisposed)
				{
					bool flag = false;
					ComposablePartCatalogCollection composablePartCatalogCollection = null;
					try
					{
						using (new WriteLock(this._thisLock))
						{
							if (!this._isDisposed)
							{
								flag = true;
								composablePartCatalogCollection = this._catalogCollection;
								this._catalogCollection = null;
								this._assemblyCatalogs = null;
								this._isDisposed = true;
							}
						}
					}
					finally
					{
						if (composablePartCatalogCollection != null)
						{
							composablePartCatalogCollection.Dispose();
						}
						if (flag)
						{
							this._thisLock.Dispose();
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
			return this._catalogCollection.SelectMany((ComposablePartCatalog catalog) => catalog).GetEnumerator();
		}

		/// <summary>Gets the export definitions that match the constraint expressed by the specified import definition.</summary>
		/// <param name="definition">The conditions of the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition" /> objects to be returned.</param>
		/// <returns>A collection of objects that contain the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition" /> objects and their associated <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects that match the constraint specified by <paramref name="definition" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Hosting.DirectoryCatalog" /> object has been disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="definition" /> is <see langword="null" />.</exception>
		public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
		{
			this.ThrowIfDisposed();
			Requires.NotNull<ImportDefinition>(definition, "definition");
			return this._catalogCollection.SelectMany((ComposablePartCatalog catalog) => catalog.GetExports(definition));
		}

		/// <summary>Raises the <see cref="E:System.ComponentModel.Composition.Hosting.DirectoryCatalog.Changed" /> event.</summary>
		/// <param name="e">An object  that contains the event data.</param>
		protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changed = this.Changed;
			if (changed != null)
			{
				changed(this, e);
			}
		}

		/// <summary>Raises the <see cref="E:System.ComponentModel.Composition.Hosting.DirectoryCatalog.Changing" /> event.</summary>
		/// <param name="e">An object  that contains the event data.</param>
		protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
		{
			EventHandler<ComposablePartCatalogChangeEventArgs> changing = this.Changing;
			if (changing != null)
			{
				changing(this, e);
			}
		}

		/// <summary>Refreshes the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition" /> objects with the latest files in the directory that match the search pattern.</summary>
		public void Refresh()
		{
			this.ThrowIfDisposed();
			Assumes.NotNull<ReadOnlyCollection<string>>(this._loadedFiles);
			ComposablePartDefinition[] addedDefinitions;
			ComposablePartDefinition[] removedDefinitions;
			for (;;)
			{
				string[] files = this.GetFiles();
				object loadedFiles;
				string[] beforeFiles;
				using (new ReadLock(this._thisLock))
				{
					loadedFiles = this._loadedFiles;
					beforeFiles = this._loadedFiles.ToArray<string>();
				}
				List<Tuple<string, AssemblyCatalog>> list;
				List<Tuple<string, AssemblyCatalog>> list2;
				this.DiffChanges(beforeFiles, files, out list, out list2);
				if (list.Count == 0 && list2.Count == 0)
				{
					break;
				}
				addedDefinitions = list.SelectMany((Tuple<string, AssemblyCatalog> cat) => cat.Item2).ToArray<ComposablePartDefinition>();
				removedDefinitions = list2.SelectMany((Tuple<string, AssemblyCatalog> cat) => cat.Item2).ToArray<ComposablePartDefinition>();
				using (AtomicComposition atomicComposition = new AtomicComposition())
				{
					ComposablePartCatalogChangeEventArgs e = new ComposablePartCatalogChangeEventArgs(addedDefinitions, removedDefinitions, atomicComposition);
					this.OnChanging(e);
					using (new WriteLock(this._thisLock))
					{
						if (loadedFiles != this._loadedFiles)
						{
							continue;
						}
						foreach (Tuple<string, AssemblyCatalog> tuple in list)
						{
							this._assemblyCatalogs.Add(tuple.Item1, tuple.Item2);
							this._catalogCollection.Add(tuple.Item2);
						}
						foreach (Tuple<string, AssemblyCatalog> tuple2 in list2)
						{
							this._assemblyCatalogs.Remove(tuple2.Item1);
							this._catalogCollection.Remove(tuple2.Item2);
						}
						this._loadedFiles = files.ToReadOnlyCollection<string>();
						atomicComposition.Complete();
					}
				}
				goto IL_1CF;
			}
			return;
			IL_1CF:
			ComposablePartCatalogChangeEventArgs e2 = new ComposablePartCatalogChangeEventArgs(addedDefinitions, removedDefinitions, null);
			this.OnChanged(e2);
		}

		/// <summary>Gets a string representation of the directory catalog.</summary>
		/// <returns>A string representation of the catalog.</returns>
		public override string ToString()
		{
			return this.GetDisplayName();
		}

		private AssemblyCatalog CreateAssemblyCatalogGuarded(string assemblyFilePath)
		{
			Exception exception = null;
			try
			{
				return (this._reflectionContext != null) ? new AssemblyCatalog(assemblyFilePath, this._reflectionContext, this) : new AssemblyCatalog(assemblyFilePath, this);
			}
			catch (FileNotFoundException exception)
			{
			}
			catch (FileLoadException exception)
			{
			}
			catch (BadImageFormatException exception)
			{
			}
			catch (ReflectionTypeLoadException exception)
			{
			}
			CompositionTrace.AssemblyLoadFailed(this, assemblyFilePath, exception);
			return null;
		}

		private void DiffChanges(string[] beforeFiles, string[] afterFiles, out List<Tuple<string, AssemblyCatalog>> catalogsToAdd, out List<Tuple<string, AssemblyCatalog>> catalogsToRemove)
		{
			catalogsToAdd = new List<Tuple<string, AssemblyCatalog>>();
			catalogsToRemove = new List<Tuple<string, AssemblyCatalog>>();
			foreach (string text in afterFiles.Except(beforeFiles))
			{
				AssemblyCatalog assemblyCatalog = this.CreateAssemblyCatalogGuarded(text);
				if (assemblyCatalog != null)
				{
					catalogsToAdd.Add(new Tuple<string, AssemblyCatalog>(text, assemblyCatalog));
				}
			}
			IEnumerable<string> enumerable = beforeFiles.Except(afterFiles);
			using (new ReadLock(this._thisLock))
			{
				foreach (string text2 in enumerable)
				{
					AssemblyCatalog item;
					if (this._assemblyCatalogs.TryGetValue(text2, out item))
					{
						catalogsToRemove.Add(new Tuple<string, AssemblyCatalog>(text2, item));
					}
				}
			}
		}

		private string GetDisplayName()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} (Path=\"{1}\")", base.GetType().Name, this._path);
		}

		private string[] GetFiles()
		{
			return Directory.GetFiles(this._fullPath, this._searchPattern);
		}

		private static string GetFullPath(string path)
		{
			if (!System.IO.Path.IsPathRooted(path) && AppDomain.CurrentDomain.BaseDirectory != null)
			{
				path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
			}
			return System.IO.Path.GetFullPath(path);
		}

		private void Initialize(string path, string searchPattern)
		{
			this._path = path;
			this._fullPath = DirectoryCatalog.GetFullPath(path);
			this._searchPattern = searchPattern;
			this._assemblyCatalogs = new Dictionary<string, AssemblyCatalog>();
			this._catalogCollection = new ComposablePartCatalogCollection(null, null, null);
			this._loadedFiles = this.GetFiles().ToReadOnlyCollection<string>();
			foreach (string text in this._loadedFiles)
			{
				AssemblyCatalog assemblyCatalog = this.CreateAssemblyCatalogGuarded(text);
				if (assemblyCatalog != null)
				{
					this._assemblyCatalogs.Add(text, assemblyCatalog);
					this._catalogCollection.Add(assemblyCatalog);
				}
			}
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		/// <summary>Gets the display name of the directory catalog.</summary>
		/// <returns>A string that contains a human-readable display name of the directory catalog.</returns>
		string ICompositionElement.DisplayName
		{
			get
			{
				return this.GetDisplayName();
			}
		}

		/// <summary>Gets the composition element from which the directory catalog originated.</summary>
		/// <returns>Always <see langword="null" />.</returns>
		ICompositionElement ICompositionElement.Origin
		{
			get
			{
				return null;
			}
		}

		private readonly Lock _thisLock;

		private readonly ICompositionElement _definitionOrigin;

		private ComposablePartCatalogCollection _catalogCollection;

		private Dictionary<string, AssemblyCatalog> _assemblyCatalogs;

		private volatile bool _isDisposed;

		private string _path;

		private string _fullPath;

		private string _searchPattern;

		private ReadOnlyCollection<string> _loadedFiles;

		private readonly ReflectionContext _reflectionContext;

		internal class DirectoryCatalogDebuggerProxy
		{
			public DirectoryCatalogDebuggerProxy(DirectoryCatalog catalog)
			{
				Requires.NotNull<DirectoryCatalog>(catalog, "catalog");
				this._catalog = catalog;
			}

			public ReadOnlyCollection<Assembly> Assemblies
			{
				get
				{
					return (from catalog in this._catalog._assemblyCatalogs.Values
					select catalog.Assembly).ToReadOnlyCollection<Assembly>();
				}
			}

			public ReflectionContext ReflectionContext
			{
				get
				{
					return this._catalog._reflectionContext;
				}
			}

			public string SearchPattern
			{
				get
				{
					return this._catalog.SearchPattern;
				}
			}

			public string Path
			{
				get
				{
					return this._catalog._path;
				}
			}

			public string FullPath
			{
				get
				{
					return this._catalog._fullPath;
				}
			}

			public ReadOnlyCollection<string> LoadedFiles
			{
				get
				{
					return this._catalog._loadedFiles;
				}
			}

			public ReadOnlyCollection<ComposablePartDefinition> Parts
			{
				get
				{
					return this._catalog.Parts.ToReadOnlyCollection<ComposablePartDefinition>();
				}
			}

			private readonly DirectoryCatalog _catalog;
		}
	}
}
