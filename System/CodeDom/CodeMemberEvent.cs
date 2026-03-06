using System;

namespace System.CodeDom
{
	/// <summary>Represents a declaration for an event of a type.</summary>
	[Serializable]
	public class CodeMemberEvent : CodeTypeMember
	{
		/// <summary>Gets or sets the data type of the delegate type that handles the event.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the delegate type that handles the event.</returns>
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

		/// <summary>Gets or sets the privately implemented data type, if any.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReference" /> that indicates the data type that the event privately implements.</returns>
		public CodeTypeReference PrivateImplementationType { get; set; }

		/// <summary>Gets or sets the data type that the member event implements.</summary>
		/// <returns>A <see cref="T:System.CodeDom.CodeTypeReferenceCollection" /> that indicates the data type or types that the member event implements.</returns>
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

		private CodeTypeReference _type;

		private CodeTypeReferenceCollection _implementationTypes;
	}
}
