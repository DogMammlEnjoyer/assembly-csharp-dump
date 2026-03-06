using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(MemberSerialization.Fields)]
	internal readonly struct ModTagObject
	{
		[JsonConstructor]
		public ModTagObject(string name, string name_localized, long date_added)
		{
			this.Name = name;
			this.NameLocalized = name_localized;
			this.DateAdded = date_added;
		}

		internal readonly string Name;

		internal readonly string NameLocalized;

		internal readonly long DateAdded;
	}
}
