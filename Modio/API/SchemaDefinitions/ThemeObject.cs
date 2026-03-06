using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
	internal readonly struct ThemeObject
	{
		[JsonConstructor]
		public ThemeObject(string primary, string dark, string light, string success, string warning, string danger)
		{
			this.Primary = primary;
			this.Dark = dark;
			this.Light = light;
			this.Success = success;
			this.Warning = warning;
			this.Danger = danger;
		}

		internal readonly string Primary;

		internal readonly string Dark;

		internal readonly string Light;

		internal readonly string Success;

		internal readonly string Warning;

		internal readonly string Danger;
	}
}
