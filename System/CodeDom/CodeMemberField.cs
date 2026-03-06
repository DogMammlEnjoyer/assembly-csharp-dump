using System;

namespace System.CodeDom
{
	/// <summary>Represents a declaration for a field of a type.</summary>
	[Serializable]
	public class CodeMemberField : CodeTypeMember
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeMemberField" /> class.</summary>
		public CodeMemberField()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeMemberField" /> class using the specified field type and field name.</summary>
		/// <param name="type">An object that indicates the type of the field.</param>
		/// <param name="name">The name of the field.</param>
		public CodeMemberField(CodeTypeReference type, string name)
		{
			this.Type = type;
			base.Name = name;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeMemberField" /> class using the specified field type and field name.</summary>
		/// <param name="type">The type of the field.</param>
		/// <param name="name">The name of the field.</param>
		public CodeMemberField(string type, string name)
		{
			this.Type = new CodeTypeReference(type);
			base.Name = name;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeMemberField" /> class using the specified field type and field name.</summary>
		/// <param name="type">The type of the field.</param>
		/// <param name="name">The name of the field.</param>
		public CodeMemberField(Type type, string name)
		{
			this.Type = new CodeTypeReference(type);
			base.Name = name;
		}

		/// <summary>Gets or sets the type of the field.</summary>
		/// <returns>The type of the field.</returns>
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

		/// <summary>Gets or sets the initialization expression for the field.</summary>
		/// <returns>The initialization expression for the field.</returns>
		public CodeExpression InitExpression { get; set; }

		private CodeTypeReference _type;
	}
}
