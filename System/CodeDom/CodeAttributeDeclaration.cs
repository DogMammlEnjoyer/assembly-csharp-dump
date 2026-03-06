using System;

namespace System.CodeDom
{
	/// <summary>Represents an attribute declaration.</summary>
	[Serializable]
	public class CodeAttributeDeclaration
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeAttributeDeclaration" /> class.</summary>
		public CodeAttributeDeclaration()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeAttributeDeclaration" /> class using the specified name.</summary>
		/// <param name="name">The name of the attribute.</param>
		public CodeAttributeDeclaration(string name)
		{
			this.Name = name;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeAttributeDeclaration" /> class using the specified name and arguments.</summary>
		/// <param name="name">The name of the attribute.</param>
		/// <param name="arguments">An array of type <see cref="T:System.CodeDom.CodeAttributeArgument" /> that contains the arguments for the attribute.</param>
		public CodeAttributeDeclaration(string name, params CodeAttributeArgument[] arguments)
		{
			this.Name = name;
			this.Arguments.AddRange(arguments);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeAttributeDeclaration" /> class using the specified code type reference.</summary>
		/// <param name="attributeType">The <see cref="T:System.CodeDom.CodeTypeReference" /> that identifies the attribute.</param>
		public CodeAttributeDeclaration(CodeTypeReference attributeType) : this(attributeType, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeAttributeDeclaration" /> class using the specified code type reference and arguments.</summary>
		/// <param name="attributeType">The <see cref="T:System.CodeDom.CodeTypeReference" /> that identifies the attribute.</param>
		/// <param name="arguments">An array of type <see cref="T:System.CodeDom.CodeAttributeArgument" /> that contains the arguments for the attribute.</param>
		public CodeAttributeDeclaration(CodeTypeReference attributeType, params CodeAttributeArgument[] arguments)
		{
			this._attributeType = attributeType;
			if (attributeType != null)
			{
				this._name = attributeType.BaseType;
			}
			if (arguments != null)
			{
				this.Arguments.AddRange(arguments);
			}
		}

		/// <summary>Gets or sets the name of the attribute being declared.</summary>
		/// <returns>The name of the attribute.</returns>
		public string Name
		{
			get
			{
				return this._name ?? string.Empty;
			}
			set
			{
				this._name = value;
				this._attributeType = new CodeTypeReference(this._name);
			}
		}

		/// <summary>Gets the arguments for the attribute.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeAttributeArgumentCollection" /> that contains the arguments for the attribute.</returns>
		public CodeAttributeArgumentCollection Arguments
		{
			get
			{
				return this._arguments;
			}
		}

		/// <summary>Gets the code type reference for the code attribute declaration.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> that identifies the <see cref="T:System.CodeDom.CodeAttributeDeclaration" />.</returns>
		public CodeTypeReference AttributeType
		{
			get
			{
				return this._attributeType;
			}
		}

		private string _name;

		private readonly CodeAttributeArgumentCollection _arguments = new CodeAttributeArgumentCollection();

		private CodeTypeReference _attributeType;
	}
}
