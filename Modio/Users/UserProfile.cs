using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API.SchemaDefinitions;
using Modio.Images;
using Modio.Reports;

namespace Modio.Users
{
	[Serializable]
	public class UserProfile : IEquatable<UserProfile>
	{
		public event Action OnProfileUpdated;

		public override int GetHashCode()
		{
			return this.UserId.GetHashCode();
		}

		public string Username { get; internal set; }

		public long UserId { get; internal set; }

		public string PortalUsername { get; private set; }

		public Wallet GetWallet()
		{
			if (this.UserId != User.Current.Profile.UserId)
			{
				return null;
			}
			return User.Current.Wallet;
		}

		public ModioImageSource<UserProfile.AvatarResolution> Avatar { get; private set; }

		public string Timezone { get; private set; }

		public string Language { get; private set; }

		internal UserProfile(UserObject userObject)
		{
			this.ApplyDetailsFromUserObject(userObject);
		}

		internal UserProfile()
		{
		}

		internal void ApplyDetailsFromUserObject(UserObject userObject)
		{
			this.Username = userObject.Username;
			this.UserId = userObject.Id;
			this.PortalUsername = userObject.DisplayNamePortal;
			this.Timezone = userObject.Timezone;
			this.Language = userObject.Language;
			this.Avatar = new ModioImageSource<UserProfile.AvatarResolution>(userObject.Avatar.Filename, new string[]
			{
				userObject.Avatar.Thumb50X50,
				userObject.Avatar.Thumb100X100,
				userObject.Avatar.Original
			});
			UserProfile._cache[this.UserId] = this;
			Action onProfileUpdated = this.OnProfileUpdated;
			if (onProfileUpdated == null)
			{
				return;
			}
			onProfileUpdated();
		}

		internal static UserProfile Get(UserObject user)
		{
			UserProfile userProfile;
			if (!UserProfile._cache.TryGetValue(user.Id, out userProfile))
			{
				return new UserProfile(user);
			}
			userProfile.ApplyDetailsFromUserObject(user);
			return userProfile;
		}

		public static bool operator ==(UserProfile left, UserProfile right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(UserProfile left, UserProfile right)
		{
			return !object.Equals(left, right);
		}

		public bool Equals(UserProfile other)
		{
			return other != null && (this == other || this.UserId == other.UserId);
		}

		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((UserProfile)obj)));
		}

		public Task<Error> Mute()
		{
			UserProfile.<Mute>d__39 <Mute>d__;
			<Mute>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Mute>d__.<>4__this = this;
			<Mute>d__.<>1__state = -1;
			<Mute>d__.<>t__builder.Start<UserProfile.<Mute>d__39>(ref <Mute>d__);
			return <Mute>d__.<>t__builder.Task;
		}

		public Task<Error> UnMute()
		{
			UserProfile.<UnMute>d__40 <UnMute>d__;
			<UnMute>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<UnMute>d__.<>4__this = this;
			<UnMute>d__.<>1__state = -1;
			<UnMute>d__.<>t__builder.Start<UserProfile.<UnMute>d__40>(ref <UnMute>d__);
			return <UnMute>d__.<>t__builder.Task;
		}

		public Task<Error> Report(ReportType reportType, string contact, string summary)
		{
			UserProfile.<Report>d__41 <Report>d__;
			<Report>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Report>d__.<>4__this = this;
			<Report>d__.reportType = reportType;
			<Report>d__.contact = contact;
			<Report>d__.summary = summary;
			<Report>d__.<>1__state = -1;
			<Report>d__.<>t__builder.Start<UserProfile.<Report>d__41>(ref <Report>d__);
			return <Report>d__.<>t__builder.Task;
		}

		private static Dictionary<long, UserProfile> _cache = new Dictionary<long, UserProfile>();

		public enum AvatarResolution
		{
			X50_Y50,
			X100_Y100,
			Original
		}
	}
}
