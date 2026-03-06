using System;

namespace System.CodeDom
{
	/// <summary>Represents a declaration for a method of a type.</summary>
	[Serializable]
	public class CodeMemberMethod : CodeTypeMember
	{
		/// <summary>An event that will be raised the first time the <see cref="P:System.CodeDom.CodeMemberMethod.Parameters" /> collection is accessed.</summary>
		public event EventHandler PopulateParameters;

		/// <summary>An event that will be raised the first time the <see cref="P:System.CodeDom.CodeMemberMethod.Statements" /> collection is accessed.</summary>
		public event EventHandler PopulateStatements;

		/// <summary>An event that will be raised the first time the <see cref="P:System.CodeDom.CodeMemberMethod.ImplementationTypes" /> collection is accessed.</summary>
		public event EventHandler PopulateImplementationTypes;

		/// <summary>Gets or sets the data type of the return value of the method.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the data type of the value returned by the method.</returns>
		public CodeTypeReference ReturnType
		{
			get
			{
				CodeTypeReference result;
				if ((result = this._returnType) == null)
				{
					result = (this._returnType = new CodeTypeReference(typeof(void).FullName));
				}
				return result;
			}
			set
			{
				this._returnType = value;
			}
		}

		/// <summary>Gets the statements within the method.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeStatementCollection" /> that indicates the statements within the method.</returns>
		public CodeStatementCollection Statements
		{
			get
			{
				if ((this._populated & 2) == 0)
				{
					this._populated |= 2;
					EventHandler populateStatements = this.PopulateStatements;
					if (populateStatements != null)
					{
						populateStatements(this, EventArgs.Empty);
					}
				}
				return this._statements;
			}
		}

		/// <summary>Gets the parameter declarations for the method.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeParameterDeclarationExpressionCollection" /> that indicates the method parameters.</returns>
		public CodeParameterDeclarationExpressionCollection Parameters
		{
			get
			{
				if ((this._populated & 1) == 0)
				{
					this._populated |= 1;
					EventHandler populateParameters = this.PopulateParameters;
					if (populateParameters != null)
					{
						populateParameters(this, EventArgs.Empty);
					}
				}
				return this._parameters;
			}
		}

		/// <summary>Gets or sets the data type of the interface this method, if private, implements a method of, if any.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the data type of the interface with the method that the private method whose declaration is represented by this <see cref="T:System.CodeDom.CodeMemberMethod" /> implements.</returns>
		public CodeTypeReference PrivateImplementationType { get; set; }

		/// <summary>Gets the data types of the interfaces implemented by this method, unless it is a private method implementation, which is indicated by the <see cref="P:System.CodeDom.CodeMemberMethod.PrivateImplementationType" /> property.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReferenceCollection" /> that indicates the interfaces implemented by this method.</returns>
		public CodeTypeReferenceCollection ImplementationTypes
		{
			get
			{
				if (this._implementationTypes == null)
				{
					this._implementationTypes = new CodeTypeReferenceCollection();
				}
				if ((this._populated & 4) == 0)
				{
					this._populated |= 4;
					EventHandler populateImplementationTypes = this.PopulateImplementationTypes;
					if (populateImplementationTypes != null)
					{
						populateImplementationTypes(this, EventArgs.Empty);
					}
				}
				return this._implementationTypes;
			}
		}

		/// <summary>Gets the custom attributes of the return type of the method.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeAttributeDeclarationCollection" /> that indicates the custom attributes.</returns>
		public CodeAttributeDeclarationCollection ReturnTypeCustomAttributes
		{
			get
			{
				CodeAttributeDeclarationCollection result;
				if ((result = this._returnAttributes) == null)
				{
					result = (this._returnAttributes = new CodeAttributeDeclarationCollection());
				}
				return result;
			}
		}

		/// <summary>Gets the type parameters for the current generic method.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeParameterCollection" /> that contains the type parameters for the generic method.</returns>
		public CodeTypeParameterCollection TypeParameters
		{
			get
			{
				CodeTypeParameterCollection result;
				if ((result = this._typeParameters) == null)
				{
					result = (this._typeParameters = new CodeTypeParameterCollection());
				}
				return result;
			}
		}

		private readonly CodeParameterDeclarationExpressionCollection _parameters = new CodeParameterDeclarationExpressionCollection();

		private readonly CodeStatementCollection _statements = new CodeStatementCollection();

		private CodeTypeReference _returnType;

		private CodeTypeReferenceCollection _implementationTypes;

		private CodeAttributeDeclarationCollection _returnAttributes;

		private CodeTypeParameterCollection _typeParameters;

		private int _populated;

		private const int ParametersCollection = 1;

		private const int StatementsCollection = 2;

		private const int ImplTypesCollection = 4;
	}
}
