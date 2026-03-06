using System;

namespace System.CodeDom
{
	/// <summary>Represents a declaration for a property of a type.</summary>
	[Serializable]
	public class CodeMemberProperty : CodeTypeMember
	{
		/// <summary>Gets or sets the data type of the interface, if any, this property, if private, implements.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the data type of the interface, if any, the property, if private, implements.</returns>
		public CodeTypeReference PrivateImplementationType { get; set; }

		/// <summary>Gets the data types of any interfaces that the property implements.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReferenceCollection" /> that indicates the data types the property implements.</returns>
		public CodeTypeReferenceCollection ImplementationTypes
		{
			get
			{
				CodeTypeReferenceCollection result;
				if ((result = this._implementationTypes) == null)
				{
					result = (this._implementationTypes = new CodeTypeReferenceCollection());
				}
				return result;
			}
		}

		/// <summary>Gets or sets the data type of the property.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the data type of the property.</returns>
		public CodeTypeReference Type
		{
			get
			{
				CodeTypeReference result;
				if ((result = this._type) == null)
				{
					result = (this._type = new CodeTypeReference(""));
				}
				return result;
			}
			set
			{
				this._type = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether the property has a <see langword="get" /> method accessor.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see langword="Count" /> property of the <see cref="P:System.CodeDom.CodeMemberProperty.GetStatements" /> collection is non-zero, or if the value of this property has been set to <see langword="true" />; otherwise, <see langword="false" />.</returns>
		public bool HasGet
		{
			get
			{
				return this._hasGet || this.GetStatements.Count > 0;
			}
			set
			{
				this._hasGet = value;
				if (!value)
				{
					this.GetStatements.Clear();
				}
			}
		}

		/// <summary>Gets or sets a value indicating whether the property has a <see langword="set" /> method accessor.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Collections.CollectionBase.Count" /> property of the <see cref="P:System.CodeDom.CodeMemberProperty.SetStatements" /> collection is non-zero; otherwise, <see langword="false" />.</returns>
		public bool HasSet
		{
			get
			{
				return this._hasSet || this.SetStatements.Count > 0;
			}
			set
			{
				this._hasSet = value;
				if (!value)
				{
					this.SetStatements.Clear();
				}
			}
		}

		/// <summary>Gets the collection of <see langword="get" /> statements for the property.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeStatementCollection" /> that contains the <see langword="get" /> statements for the member property.</returns>
		public CodeStatementCollection GetStatements { get; } = new CodeStatementCollection();

		/// <summary>Gets the collection of <see langword="set" /> statements for the property.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeStatementCollection" /> that contains the <see langword="set" /> statements for the member property.</returns>
		public CodeStatementCollection SetStatements { get; } = new CodeStatementCollection();

		/// <summary>Gets the collection of declaration expressions for the property.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeParameterDeclarationExpressionCollection" /> that indicates the declaration expressions for the property.</returns>
		public CodeParameterDeclarationExpressionCollection Parameters { get; } = new CodeParameterDeclarationExpressionCollection();

		private CodeTypeReference _type;

		private bool _hasGet;

		private bool _hasSet;

		private CodeTypeReferenceCollection _implementationTypes;
	}
}
