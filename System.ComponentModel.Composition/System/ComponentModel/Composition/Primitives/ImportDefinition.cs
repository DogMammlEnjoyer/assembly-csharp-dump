using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	/// <summary>Represents an import that is required by a <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> object.</summary>
	public class ImportDefinition
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition" /> class.</summary>
		protected ImportDefinition()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition" /> class with the specified constraint, contract name, and cardinality, and indicates whether the import definition is recomposable or a prerequisite.</summary>
		/// <param name="constraint">An expression that contains a <see cref="T:System.Func`2" /> object that defines the conditions an <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> must match to satisfy the import definition.</param>
		/// <param name="contractName">The contract name.</param>
		/// <param name="cardinality">One of the enumeration values that indicates the cardinality of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects required by the import definition.</param>
		/// <param name="isRecomposable">
		///   <see langword="true" /> to specify that the import definition can be satisfied multiple times throughout the lifetime of a <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> object; otherwise, <see langword="false" />.</param>
		/// <param name="isPrerequisite">
		///   <see langword="true" /> to specify that the import definition must be satisfied before a <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> can start producing exported objects; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="constraint" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="cardinality" /> is not one of the values of <see cref="T:System.ComponentModel.Composition.Primitives.ImportCardinality" />.</exception>
		public ImportDefinition(Expression<Func<ExportDefinition, bool>> constraint, string contractName, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite) : this(contractName, cardinality, isRecomposable, isPrerequisite, MetadataServices.EmptyMetadata)
		{
			Requires.NotNull<Expression<Func<ExportDefinition, bool>>>(constraint, "constraint");
			this._constraint = constraint;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition" /> class with the specified constraint, contract name, cardinality, and metadata, and indicates whether the import definition is recomposable or a prerequisite.</summary>
		/// <param name="constraint">An expression that contains a <see cref="T:System.Func`2" /> object that defines the conditions an <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> must match to satisfy the import definition.</param>
		/// <param name="contractName">The contract name.</param>
		/// <param name="cardinality">One of the enumeration values that indicates the cardinality of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects required by the import definition.</param>
		/// <param name="isRecomposable">
		///   <see langword="true" /> to specify that the import definition can be satisfied multiple times throughout the lifetime of a <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> object; otherwise, <see langword="false" />.</param>
		/// <param name="isPrerequisite">
		///   <see langword="true" /> to specify that the import definition must be satisfied before a <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> can start producing exported objects; otherwise, <see langword="false" />.</param>
		/// <param name="metadata">The metadata associated with the import.</param>
		public ImportDefinition(Expression<Func<ExportDefinition, bool>> constraint, string contractName, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite, IDictionary<string, object> metadata) : this(contractName, cardinality, isRecomposable, isPrerequisite, metadata)
		{
			Requires.NotNull<Expression<Func<ExportDefinition, bool>>>(constraint, "constraint");
			this._constraint = constraint;
		}

		internal ImportDefinition(string contractName, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite, IDictionary<string, object> metadata)
		{
			if (cardinality != ImportCardinality.ExactlyOne && cardinality != ImportCardinality.ZeroOrMore && cardinality != ImportCardinality.ZeroOrOne)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ArgumentOutOfRange_InvalidEnum, "cardinality", cardinality, typeof(ImportCardinality).Name), "cardinality");
			}
			this._contractName = (contractName ?? ImportDefinition.EmptyContractName);
			this._cardinality = cardinality;
			this._isRecomposable = isRecomposable;
			this._isPrerequisite = isPrerequisite;
			if (metadata != null)
			{
				this._metadata = metadata;
			}
		}

		/// <summary>Gets the name of the contract.</summary>
		/// <returns>The contract name.</returns>
		public virtual string ContractName
		{
			get
			{
				return this._contractName;
			}
		}

		/// <summary>Gets the metadata associated with this import.</summary>
		/// <returns>A collection that contains the metadata associated with this import.</returns>
		public virtual IDictionary<string, object> Metadata
		{
			get
			{
				return this._metadata;
			}
		}

		/// <summary>Gets the cardinality of the exports required by the import definition.</summary>
		/// <returns>One of the enumeration values that indicates the cardinality of the <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> objects required by the <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition" />. The default is <see cref="F:System.ComponentModel.Composition.Primitives.ImportCardinality.ExactlyOne" />.</returns>
		public virtual ImportCardinality Cardinality
		{
			get
			{
				return this._cardinality;
			}
		}

		/// <summary>Gets an expression that defines conditions that the import must satisfy to match the import definition.</summary>
		/// <returns>An expression that contains a <see cref="T:System.Func`2" /> object that defines the conditions an <see cref="T:System.ComponentModel.Composition.Primitives.Export" /> must satisfy to match the <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition" />.</returns>
		/// <exception cref="T:System.NotImplementedException">The property was not overridden by a derived class.</exception>
		public virtual Expression<Func<ExportDefinition, bool>> Constraint
		{
			get
			{
				if (this._constraint != null)
				{
					return this._constraint;
				}
				throw ExceptionBuilder.CreateNotOverriddenByDerived("Constraint");
			}
		}

		/// <summary>Gets a value that indicates whether the import definition must be satisfied before a part can start producing exported objects.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition" /> must be satisfied before a <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> object can start producing exported objects; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public virtual bool IsPrerequisite
		{
			get
			{
				return this._isPrerequisite;
			}
		}

		/// <summary>Gets a value that indicates whether the import definition can be satisfied multiple times.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition" /> can be satisfied multiple times throughout the lifetime of a <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" /> object; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public virtual bool IsRecomposable
		{
			get
			{
				return this._isRecomposable;
			}
		}

		/// <summary>Gets a value that indicates whether the export represented by the specified definition satisfies the constraints of this import definition.</summary>
		/// <param name="exportDefinition">The export definition to test.</param>
		/// <returns>
		///   <see langword="true" /> if the constraints are satisfied; otherwise, <see langword="false" />.</returns>
		public virtual bool IsConstraintSatisfiedBy(ExportDefinition exportDefinition)
		{
			Requires.NotNull<ExportDefinition>(exportDefinition, "exportDefinition");
			if (this._compiledConstraint == null)
			{
				this._compiledConstraint = this.Constraint.Compile();
			}
			return this._compiledConstraint(exportDefinition);
		}

		/// <summary>Returns a string representation of the import definition.</summary>
		/// <returns>A string representation of the import definition.</returns>
		public override string ToString()
		{
			return this.Constraint.Body.ToString();
		}

		internal static readonly string EmptyContractName = string.Empty;

		private readonly Expression<Func<ExportDefinition, bool>> _constraint;

		private readonly ImportCardinality _cardinality = ImportCardinality.ExactlyOne;

		private readonly string _contractName = ImportDefinition.EmptyContractName;

		private readonly bool _isRecomposable;

		private readonly bool _isPrerequisite = true;

		private Func<ExportDefinition, bool> _compiledConstraint;

		private readonly IDictionary<string, object> _metadata = MetadataServices.EmptyMetadata;
	}
}
