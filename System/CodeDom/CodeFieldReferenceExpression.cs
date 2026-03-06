using System;

namespace System.CodeDom
{
	/// <summary>Represents a reference to a field.</summary>
	[Serializable]
	public class CodeFieldReferenceExpression : CodeExpression
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeFieldReferenceExpression" /> class.</summary>
		public CodeFieldReferenceExpression()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeFieldReferenceExpression" /> class using the specified target object and field name.</summary>
		/// <param name="targetObject">A <see cref="T:System.CodeDom.CodeExpression" /> that indicates the object that contains the field.</param>
		/// <param name="fieldName">The name of the field.</param>
		public CodeFieldReferenceExpression(CodeExpression targetObject, string fieldName)
		{
			this.TargetObject = targetObject;
			this.FieldName = fieldName;
		}

		/// <summary>Gets or sets the object that contains the field to reference.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeExpression" /> that indicates the object that contains the field to reference.</returns>
		public CodeExpression TargetObject { get; set; }

		/// <summary>Gets or sets the name of the field to reference.</summary>
		/// <returns>A string containing the field name.</returns>
		public string FieldName
		{
			get
			{
				return this._fieldName ?? string.Empty;
			}
			set
			{
				this._fieldName = value;
			}
		}

		private string _fieldName;
	}
}
