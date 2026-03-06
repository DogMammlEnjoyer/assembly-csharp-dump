using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct EmbeddableModHubConfigurationObject
	{
		[JsonConstructor]
		public EmbeddableModHubConfigurationObject(long id, string name, string[] urls, string style, string css, bool allow_subscribing, bool allow_rating, bool allow_reporting, bool allow_downloading, bool allow_commenting, bool allow_filtering, bool allow_searching, bool allow_infinite_scroll, bool allow_email_auth, bool allow_sso_auth, bool allow_steam_auth, bool allow_PSN_auth, bool allow_xbox_auth, bool allow_egs_auth, bool allow_discord_auth, bool allow_google_auth, bool show_collection, bool show_comments, bool show_guides, bool show_user_avatars, bool show_sort_tabs, bool allow_links, bool filter_right_side, bool name_right_side, long results_per_page, long min_age, long date_added, long date_updated, string company_name, object[] agreement_urls)
		{
			this.Id = id;
			this.Name = name;
			this.Urls = urls;
			this.Style = style;
			this.Css = css;
			this.AllowSubscribing = allow_subscribing;
			this.AllowRating = allow_rating;
			this.AllowReporting = allow_reporting;
			this.AllowDownloading = allow_downloading;
			this.AllowCommenting = allow_commenting;
			this.AllowFiltering = allow_filtering;
			this.AllowSearching = allow_searching;
			this.AllowInfiniteScroll = allow_infinite_scroll;
			this.AllowEmailAuth = allow_email_auth;
			this.AllowSsoAuth = allow_sso_auth;
			this.AllowSteamAuth = allow_steam_auth;
			this.AllowPsnAuth = allow_PSN_auth;
			this.AllowXboxAuth = allow_xbox_auth;
			this.AllowEgsAuth = allow_egs_auth;
			this.AllowDiscordAuth = allow_discord_auth;
			this.AllowGoogleAuth = allow_google_auth;
			this.ShowCollection = show_collection;
			this.ShowComments = show_comments;
			this.ShowGuides = show_guides;
			this.ShowUserAvatars = show_user_avatars;
			this.ShowSortTabs = show_sort_tabs;
			this.AllowLinks = allow_links;
			this.FilterRightSide = filter_right_side;
			this.NameRightSide = name_right_side;
			this.ResultsPerPage = results_per_page;
			this.MinAge = min_age;
			this.DateAdded = date_added;
			this.DateUpdated = date_updated;
			this.CompanyName = company_name;
			this.AgreementUrls = agreement_urls;
		}

		internal readonly long Id;

		internal readonly string Name;

		internal readonly string[] Urls;

		internal readonly string Style;

		internal readonly string Css;

		internal readonly bool AllowSubscribing;

		internal readonly bool AllowRating;

		internal readonly bool AllowReporting;

		internal readonly bool AllowDownloading;

		internal readonly bool AllowCommenting;

		internal readonly bool AllowFiltering;

		internal readonly bool AllowSearching;

		internal readonly bool AllowInfiniteScroll;

		internal readonly bool AllowEmailAuth;

		internal readonly bool AllowSsoAuth;

		internal readonly bool AllowSteamAuth;

		internal readonly bool AllowPsnAuth;

		internal readonly bool AllowXboxAuth;

		internal readonly bool AllowEgsAuth;

		internal readonly bool AllowDiscordAuth;

		internal readonly bool AllowGoogleAuth;

		internal readonly bool ShowCollection;

		internal readonly bool ShowComments;

		internal readonly bool ShowGuides;

		internal readonly bool ShowUserAvatars;

		internal readonly bool ShowSortTabs;

		internal readonly bool AllowLinks;

		internal readonly bool FilterRightSide;

		internal readonly bool NameRightSide;

		internal readonly long ResultsPerPage;

		internal readonly long MinAge;

		internal readonly long DateAdded;

		internal readonly long DateUpdated;

		internal readonly string CompanyName;

		internal readonly object[] AgreementUrls;
	}
}
