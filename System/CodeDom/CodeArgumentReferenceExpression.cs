using System;

namespace System.CodeDom
{
	/// <summary>Represents a reference to the value of an argument passed to a method.</summary>
	[Serializable]
	public class CodeArgumentReferenceExpression : CodeExpression
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeArgumentReferenceExpression" /> class.</summary>
		public CodeArgumentReferenceExpression()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.CodeArgumentReferenceExpression" /> class using the specified parameter name.</summary>
		/// <param name="parameterName">The name of the parameter to reference.</param>
		public CodeArgumentReferenceExpression(string parameterName)
		{
			this._parameterName = parameterName;
		}

		/// <summary>Gets or sets the name of the parameter this expression references.</summary>
		/// <returns>The name of the parameter to reference.</returns>
		public string ParameterName
		{
			get
			{
				return this._parameterName ?? string.Empty;
			}
			set
			{
				this._parameterName = value;
			}
		}

		private string _parameterName;
	}
}
