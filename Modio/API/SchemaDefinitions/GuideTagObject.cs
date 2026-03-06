using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GuideTagObject
	{
		[JsonConstructor]
		public GuideTagObject(string name, long date_added, long count)
		{
			this.Name = name;
			this.DateAdded = date_added;
			this.Count = count;
		}

		internal readonly string Name;

		internal readonly long DateAdded;

		internal readonly long Count;
	}
}
