using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	/// <summary>Represents an export, which is a type that consists of a delay-created exported object and the metadata that describes that object.</summary>
	public class Export
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> class.</summary>
		protected Export()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> class with the specified contract name and exported value getter.</summary>
		/// <param name="contractName">The contract name of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> object.</param>
		/// <param name="exportedValueGetter">A method that is called to create the exported object of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" />. This delays the creation of the object until the <see cref="P:System.ComponentModel.Composition.Primitives.Export.Value" /> method is called.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="contractName" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="exportedObjectGetter" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="contractName" /> is an empty string ("").</exception>
		public Export(string contractName, Func<object> exportedValueGetter) : this(new ExportDefinition(contractName, null), exportedValueGetter)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> class with the specified contract name, metadata, and exported value getter.</summary>
		/// <param name="contractName">The contract name of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> object.</param>
		/// <param name="metadata">The metadata of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> object or <see langword="null" /> to set the <see cref="P:System.ComponentModel.Composition.Primitives.Export.Metadata" /> property to an empty, read-only <see cref="T:System.Collections.Generic.IDictionary`2" /> object.</param>
		/// <param name="exportedValueGetter">A method that is called to create the exported object of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" />. This delays the creation of the object until the <see cref="P:System.ComponentModel.Composition.Primitives.Export.Value" /> method is called.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="contractName" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="exportedObjectGetter" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="contractName" /> is an empty string ("").</exception>
		public Export(string contractName, IDictionary<string, object> metadata, Func<object> exportedValueGetter) : this(new ExportDefinition(contractName, metadata), exportedValueGetter)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> class with the specified export definition and exported object getter.</summary>
		/// <param name="definition">An object that describes the contract that the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> object satisfies.</param>
		/// <param name="exportedValueGetter">A method that is called to create the exported object of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" />. This delays the creation of the object until the <see cref="P:System.ComponentModel.Composition.Primitives.Export.Value" /> property is called.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="definition" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="exportedObjectGetter" /> is <see langword="null" />.</exception>
		public Export(ExportDefinition definition, Func<object> exportedValueGetter)
		{
			Requires.NotNull<ExportDefinition>(definition, "definition");
			Requires.NotNull<Func<object>>(exportedValueGetter, "exportedValueGetter");
			this._definition = definition;
			this._exportedValueGetter = exportedValueGetter;
		}

		/// <summary>Gets the definition that describes the contract that the export satisfies.</summary>
		/// <returns>A definition that describes the contract that the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> object satisfies.</returns>
		/// <exception cref="T:System.NotImplementedException">This property was not overridden by a derived class.</exception>
		public virtual ExportDefinition Definition
		{
			get
			{
				if (this._definition != null)
				{
					return this._definition;
				}
				throw ExceptionBuilder.CreateNotOverriddenByDerived("Definition");
			}
		}

		/// <summary>Gets the metadata for the export.</summary>
		/// <returns>The metadata of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" />.</returns>
		/// <exception cref="T:System.NotImplementedException">The <see cref="P:System.ComponentModel.Composition.Primitives.Export.Definition" /> property was not overridden by a derived class.</exception>
		public IDictionary<string, object> Metadata
		{
			get
			{
				return this.Definition.Metadata;
			}
		}

		/// <summary>Provides the object this export represents.</summary>
		/// <returns>The object this export represents.</returns>
		public object Value
		{
			get
			{
				if (this._exportedValue == Export._EmptyValue)
				{
					object exportedValueCore = this.GetExportedValueCore();
					Interlocked.CompareExchange(ref this._exportedValue, exportedValueCore, Export._EmptyValue);
				}
				return this._exportedValue;
			}
		}

		/// <summary>Returns the exported object the export provides.</summary>
		/// <returns>The exported object the export provides.</returns>
		/// <exception cref="T:System.NotImplementedException">The <see cref="M:System.ComponentModel.Composition.Primitives.Export.GetExportedValueCore" /> method was not overridden by a derived class.</exception>
		/// <exception cref="T:System.ComponentModel.Composition.CompositionException">An error occurred during composition. <see cref="P:System.ComponentModel.Composition.CompositionException.Errors" /> will contain a collection of errors that occurred.</exception>
		protected virtual object GetExportedValueCore()
		{
			if (this._exportedValueGetter != null)
			{
				return this._exportedValueGetter();
			}
			throw ExceptionBuilder.CreateNotOverriddenByDerived("GetExportedValueCore");
		}

		private readonly ExportDefinition _definition;

		private readonly Func<object> _exportedValueGetter;

		private static readonly object _EmptyValue = new object();

		private volatile object _exportedValue = Export._EmptyValue;
	}
}
