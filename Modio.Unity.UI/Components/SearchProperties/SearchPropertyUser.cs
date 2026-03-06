using System;
using System.Collections.Generic;
using Modio.Unity.UI.Search;
using Modio.Users;
using UnityEngine;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertyUser : ISearchProperty
	{
		public void OnSearchUpdate(ModioUISearch search)
		{
			IReadOnlyList<UserProfile> users = search.LastSearchFilter.GetUsers();
			this._user.SetUser((users.Count > 0) ? users[0] : null);
		}

		[SerializeField]
		private ModioUIUser _user;
	}
}
