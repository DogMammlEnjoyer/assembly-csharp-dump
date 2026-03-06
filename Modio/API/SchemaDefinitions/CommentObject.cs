using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct CommentObject
	{
		[JsonConstructor]
		public CommentObject(long id, long game_id, long mod_id, long resource_id, UserObject user, long date_added, long reply_id, string thread_position, long karma, long karma_guest, string content, long options)
		{
			this.Id = id;
			this.GameId = game_id;
			this.ModId = mod_id;
			this.ResourceId = resource_id;
			this.User = user;
			this.DateAdded = date_added;
			this.ReplyId = reply_id;
			this.ThreadPosition = thread_position;
			this.Karma = karma;
			this.KarmaGuest = karma_guest;
			this.Content = content;
			this.Options = options;
		}

		internal readonly long Id;

		internal readonly long GameId;

		internal readonly long ModId;

		internal readonly long ResourceId;

		internal readonly UserObject User;

		internal readonly long DateAdded;

		internal readonly long ReplyId;

		internal readonly string ThreadPosition;

		internal readonly long Karma;

		internal readonly long KarmaGuest;

		internal readonly string Content;

		internal readonly long Options;
	}
}
