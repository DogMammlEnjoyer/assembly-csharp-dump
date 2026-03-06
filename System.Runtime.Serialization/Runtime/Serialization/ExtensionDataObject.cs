using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
	/// <summary>Stores data from a versioned data contract that has been extended by adding new members.</summary>
	public sealed class ExtensionDataObject
	{
		internal ExtensionDataObject()
		{
		}

		internal IList<ExtensionDataMember> Members
		{
			get
			{
				return this.members;
			}
			set
			{
				this.members = value;
			}
		}

		private IList<ExtensionDataMember> members;
	}
}
