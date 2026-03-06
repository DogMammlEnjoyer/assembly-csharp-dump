using System;

namespace System.CodeDom
{
	/// <summary>Represents a reference to a data type.</summary>
	[Serializable]
	public class CodeTypeReferenceExpression : CodeExpression
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReferenceExpression" /> class.</summary>
		public CodeTypeReferenceExpression()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReferenceExpression" /> class using the specified type.</summary>
		/// <param name="type">A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the data type to reference.</param>
		public CodeTypeReferenceExpression(CodeTypeReference type)
		{
			this.Type = type;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReferenceExpression" /> class using the specified data type name.</summary>
		/// <param name="type">The name of the data type to reference.</param>
		public CodeTypeReferenceExpression(string type)
		{
			this.Type = new CodeTypeReference(type);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeTypeReferenceExpression" /> class using the specified data type.</summary>
		/// <param name="type">An instance of the data type to reference.</param>
		public CodeTypeReferenceExpression(Type type)
		{
			this.Type = new CodeTypeReference(type);
		}

		/// <summary>Gets or sets the data type to reference.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the data type to reference.</returns>
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

		private CodeTypeReference _type;
	}
}
