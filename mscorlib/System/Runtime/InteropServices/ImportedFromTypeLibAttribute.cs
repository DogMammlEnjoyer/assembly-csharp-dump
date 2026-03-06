using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Indicates that the types defined within an assembly were originally defined in a type library.</summary>
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class ImportedFromTypeLibAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.ImportedFromTypeLibAttribute" /> class with the name of the original type library file.</summary>
		/// <param name="tlbFile">The location of the original type library file.</param>
		public ImportedFromTypeLibAttribute(string tlbFile)
		{
			this._val = tlbFile;
		}

		/// <summary>Gets the name of the original type library file.</summary>
		/// <returns>The name of the original type library file.</returns>
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
