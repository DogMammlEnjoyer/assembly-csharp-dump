using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
	/// <summary>Discovers attributed parts from a collection of types.</summary>
	[DebuggerTypeProxy(typeof(ComposablePartCatalogDebuggerProxy))]
	public class TypeCatalog : ComposablePartCatalog, ICompositionElement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" /> class with the specified types.</summary>
		/// <param name="types">An array of attributed <see cref="T:System.Type" /> objects to add to the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> contains an element that is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> contains an element that was loaded in the reflection-only context.</exception>
		public TypeCatalog(params Type[] types) : this(types)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" /> class with the specified types.</summary>
		/// <param name="types">A collection of attributed <see cref="T:System.Type" /> objects to add to the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> contains an element that is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> contains an element that was loaded in the reflection-only context.</exception>
		public TypeCatalog(IEnumerable<Type> types)
		{
			this._thisLock = new object();
			base..ctor();
			Requires.NotNull<IEnumerable<Type>>(types, "types");
			this.InitializeTypeCatalog(types);
			this._definitionOrigin = this;
			this._contractPartIndex = new Lazy<IDictionary<string, List<ComposablePartDefinition>>>(new Func<IDictionary<string, List<ComposablePartDefinition>>>(this.CreateIndex), true);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" /> class with the specified types and source for parts.</summary>
		/// <param name="types">A collection of attributed <see cref="T:System.Type" /> objects to add to the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" /> object.</param>
		/// <param name="definitionOrigin">An element used by diagnostics to identify the source for parts.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> contains an element that is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> contains an element that was loaded in the reflection-only context.</exception>
		public TypeCatalog(IEnumerable<Type> types, ICompositionElement definitionOrigin)
		{
			this._thisLock = new object();
			base..ctor();
			Requires.NotNull<IEnumerable<Type>>(types, "types");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this.InitializeTypeCatalog(types);
			this._definitionOrigin = definitionOrigin;
			this._contractPartIndex = new Lazy<IDictionary<string, List<ComposablePartDefinition>>>(new Func<IDictionary<string, List<ComposablePartDefinition>>>(this.CreateIndex), true);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" /> class with the specified types in the specified reflection context.</summary>
		/// <param name="types">A collection of attributed <see cref="T:System.Type" /> objects to add to the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" /> object.</param>
		/// <param name="reflectionContext">The context used to interpret the types.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> contains an element that is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> contains an element that was loaded in the reflection-only context.</exception>
		public TypeCatalog(IEnumerable<Type> types, ReflectionContext reflectionContext)
		{
			this._thisLock = new object();
			base..ctor();
			Requires.NotNull<IEnumerable<Type>>(types, "types");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			this.InitializeTypeCatalog(types, reflectionContext);
			this._definitionOrigin = this;
			this._contractPartIndex = new Lazy<IDictionary<string, List<ComposablePartDefinition>>>(new Func<IDictionary<string, List<ComposablePartDefinition>>>(this.CreateIndex), true);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" /> class with the specified types in the specified reflection context and source for parts.</summary>
		/// <param name="types">A collection of attributed <see cref="T:System.Type" /> objects to add to the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" /> object.</param>
		/// <param name="reflectionContext">The context used to interpret the types.</param>
		/// <param name="definitionOrigin">An element used by diagnostics to identify the source for parts.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> contains an element that is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> contains an element that was loaded in the reflection-only context.</exception>
		public TypeCatalog(IEnumerable<Type> types, ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
		{
			this._thisLock = new object();
			base..ctor();
			Requires.NotNull<IEnumerable<Type>>(types, "types");
			Requires.NotNull<ReflectionContext>(reflectionContext, "reflectionContext");
			Requires.NotNull<ICompositionElement>(definitionOrigin, "definitionOrigin");
			this.InitializeTypeCatalog(types, reflectionContext);
			this._definitionOrigin = definitionOrigin;
			this._contractPartIndex = new Lazy<IDictionary<string, List<ComposablePartDefinition>>>(new Func<IDictionary<string, List<ComposablePartDefinition>>>(this.CreateIndex), true);
		}

		private void InitializeTypeCatalog(IEnumerable<Type> types, ReflectionContext reflectionContext)
		{
			List<Type> list = new List<Type>();
			foreach (Type type in types)
			{
				if (type == null)
				{
					throw ExceptionBuilder.CreateContainsNullElement("types");
				}
				if (type.Assembly.ReflectionOnly)
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Argument_ElementReflectionOnlyType, "types"), "types");
				}
				TypeInfo typeInfo = type.GetTypeInfo();
				TypeInfo typeInfo2 = (reflectionContext != null) ? reflectionContext.MapType(typeInfo) : typeInfo;
				if (typeInfo2 != null)
				{
					if (typeInfo2.Assembly.ReflectionOnly)
					{
						throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Argument_ReflectionContextReturnsReflectionOnlyType, "reflectionContext"), "reflectionContext");
					}
					list.Add(typeInfo2);
				}
			}
			this._types = list.ToArray();
		}

		private void InitializeTypeCatalog(IEnumerable<Type> types)
		{
			using (IEnumerator<Type> enumerator = types.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == null)
					{
						throw ExceptionBuilder.CreateContainsNullElement("types");
					}
				}
			}
			this._types = types.ToArray<Type>();
		}

		/// <summary>Returns an enumerator that iterates through the catalog.</summary>
		/// <returns>An enumerator that can be used to iterate through the catalog.</returns>
		public override IEnumerator<ComposablePartDefinition> GetEnumerator()
		{
			this.ThrowIfDisposed();
			return this.PartsInternal.GetEnumerator();
		}

		/// <summary>Gets the display name of the type catalog.</summary>
		/// <returns>A string containing a human-readable display name of the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" />.</returns>
		string ICompositionElement.DisplayName
		{
			get
			{
				return this.GetDisplayName();
			}
		}

		/// <summary>Gets the composition element from which the type catalog originated.</summary>
		/// <returns>Always <see langword="null" />.</returns>
		ICompositionElement ICompositionElement.Origin
		{
			get
			{
				return null;
			}
		}

		private IEnumerable<ComposablePartDefinition> PartsInternal
		{
			get
			{
				if (this._parts == null)
				{
					object thisLock = this._thisLock;
					lock (thisLock)
					{
						if (this._parts == null)
						{
							Assumes.NotNull<Type[]>(this._types);
							List<ComposablePartDefinition> list = new List<ComposablePartDefinition>();
							Type[] types = this._types;
							for (int i = 0; i < types.Length; i++)
							{
								ComposablePartDefinition composablePartDefinition = AttributedModelDiscovery.CreatePartDefinitionIfDiscoverable(types[i], this._definitionOrigin);
								if (composablePartDefinition != null)
								{
									list.Add(composablePartDefinition);
								}
							}
							Thread.MemoryBarrier();
							this._types = null;
							this._parts = list;
						}
					}
				}
				return this._parts;
			}
		}

		internal override IEnumerable<ComposablePartDefinition> GetCandidateParts(ImportDefinition definition)
		{
			Assumes.NotNull<ImportDefinition>(definition);
			string contractName = definition.ContractName;
			if (string.IsNullOrEmpty(contractName))
			{
				return this.PartsInternal;
			}
			string value = definition.Metadata.GetValue("System.ComponentModel.Composition.GenericContractName");
			ICollection<ComposablePartDefinition> candidateParts = this.GetCandidateParts(contractName);
			List<ComposablePartDefinition> candidateParts2 = this.GetCandidateParts(value);
			return candidateParts.ConcatAllowingNull(candidateParts2);
		}

		private List<ComposablePartDefinition> GetCandidateParts(string contractName)
		{
			if (contractName == null)
			{
				return null;
			}
			List<ComposablePartDefinition> result = null;
			this._contractPartIndex.Value.TryGetValue(contractName, out result);
			return result;
		}

		private IDictionary<string, List<ComposablePartDefinition>> CreateIndex()
		{
			Dictionary<string, List<ComposablePartDefinition>> dictionary = new Dictionary<string, List<ComposablePartDefinition>>(StringComparers.ContractName);
			foreach (ComposablePartDefinition composablePartDefinition in this.PartsInternal)
			{
				foreach (string key in (from export in composablePartDefinition.ExportDefinitions
				select export.ContractName).Distinct<string>())
				{
					List<ComposablePartDefinition> list = null;
					if (!dictionary.TryGetValue(key, out list))
					{
						list = new List<ComposablePartDefinition>();
						dictionary.Add(key, list);
					}
					list.Add(composablePartDefinition);
				}
			}
			return dictionary;
		}

		/// <summary>Returns a string representation of the type catalog.</summary>
		/// <returns>A string representation of the type catalog.</returns>
		public override string ToString()
		{
			return this.GetDisplayName();
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Composition.Hosting.TypeCatalog" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._isDisposed = true;
			}
			base.Dispose(disposing);
		}

		private string GetDisplayName()
		{
			return string.Format(CultureInfo.CurrentCulture, Strings.TypeCatalog_DisplayNameFormat, base.GetType().Name, this.GetTypesDisplay());
		}

		private string GetTypesDisplay()
		{
			int num = this.PartsInternal.Count<ComposablePartDefinition>();
			if (num == 0)
			{
				return Strings.TypeCatalog_Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ComposablePartDefinition composablePartDefinition in this.PartsInternal.Take(2))
			{
				ReflectionComposablePartDefinition reflectionComposablePartDefinition = (ReflectionComposablePartDefinition)composablePartDefinition;
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(reflectionComposablePartDefinition.GetPartType().GetDisplayName());
			}
			if (num > 2)
			{
				stringBuilder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
				stringBuilder.Append(" ...");
			}
			return stringBuilder.ToString();
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private readonly object _thisLock;

		private Type[] _types;

		private volatile List<ComposablePartDefinition> _parts;

		private volatile bool _isDisposed;

		private readonly ICompositionElement _definitionOrigin;

		private readonly Lazy<IDictionary<string, List<ComposablePartDefinition>>> _contractPartIndex;
	}
}
