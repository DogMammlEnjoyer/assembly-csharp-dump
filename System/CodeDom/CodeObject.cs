using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.CodeDom
{
	/// <summary>Provides a common base class for most Code Document Object Model (CodeDOM) objects.</summary>
	[Serializable]
	public class CodeObject
	{
		/// <summary>Gets the user-definable data for the current object.</summary>
		/// <returns>An <see cref="T:System.Collections.IDictionary" /> containing user data for the current object.</returns>
		public IDictionary UserData
		{
			get
			{
				IDictionary result;
				if ((result = this._userData) == null)
				{
					result = (this._userData = new ListDictionary());
				}
				return result;
			}
		}

		private IDictionary _userData;
	}
}
