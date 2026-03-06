using System;

namespace Modio.Unity.UI.Search
{
	[Serializable]
	public enum SpecialSearchType
	{
		Nothing = 8,
		Installed = 5,
		Subscribed,
		InstalledOrSubscribed,
		UserCreations = 9,
		Purchased,
		SearchForTag,
		SearchForUser
	}
}
