using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Allows the user to specify the ProgID of a class.</summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	[ComVisible(true)]
	public sealed class ProgIdAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see langword="ProgIdAttribute" /> with the specified ProgID.</summary>
		/// <param name="progId">The ProgID to be assigned to the class.</param>
		public ProgIdAttribute(string progId)
		{
			this._val = progId;
		}

		/// <summary>Gets the ProgID of the class.</summary>
		/// <returns>The ProgID of the class.</returns>
		public string Value
		{
			get
			{
				return this._val;
			}
		}

		internal string _val;
	}
}
