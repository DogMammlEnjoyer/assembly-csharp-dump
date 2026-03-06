using System;

namespace Modio.Mods.Builder
{
	[Flags]
	public enum ChangeFlags
	{
		None = 0,
		Name = 1,
		Summary = 2,
		Description = 4,
		Logo = 8,
		Gallery = 16,
		Tags = 32,
		MetadataBlob = 64,
		MetadataKvps = 128,
		Visibility = 256,
		MaturityOptions = 512,
		CommunityOptions = 1024,
		Modfile = 2048,
		MonetizationConfig = 4096,
		Dependencies = 8192,
		AddFlags = 1903,
		EditFlags = 5999
	}
}
