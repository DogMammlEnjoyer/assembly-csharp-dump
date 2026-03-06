using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(MemberSerialization.Fields)]
	internal readonly struct ModDependantsObject
	{
		[JsonConstructor]
		public ModDependantsObject(long mod_id, string name, string name_id, long status, long visible, long date_added, long date_updated, LogoObject logo)
		{
			this.ModId = mod_id;
			this.Name = name;
			this.NameId = name_id;
			this.Status = status;
			this.Visible = visible;
			this.DateAdded = date_added;
			this.DateUpdated = date_updated;
			this.Logo = logo;
		}

		internal readonly long ModId;

		internal readonly string Name;

		internal readonly string NameId;

		internal readonly long Status;

		internal readonly long Visible;

		internal readonly long DateAdded;

		internal readonly long DateUpdated;

		internal readonly LogoObject Logo;
	}
}
