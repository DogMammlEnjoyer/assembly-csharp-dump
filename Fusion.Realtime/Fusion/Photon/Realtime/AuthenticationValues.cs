using System;
using System.Collections.Generic;

namespace Fusion.Photon.Realtime
{
	public class AuthenticationValues
	{
		public CustomAuthenticationType AuthType
		{
			get
			{
				return this.authType;
			}
			set
			{
				this.authType = value;
			}
		}

		public string AuthGetParameters { get; set; }

		public object AuthPostData { get; private set; }

		public object Token { get; protected internal set; }

		public string UserId { get; set; }

		public AuthenticationValues()
		{
		}

		public AuthenticationValues(string userId)
		{
			this.UserId = userId;
		}

		public virtual void SetAuthPostData(string stringData)
		{
			this.AuthPostData = (string.IsNullOrEmpty(stringData) ? null : stringData);
		}

		public virtual void SetAuthPostData(byte[] byteData)
		{
			this.AuthPostData = byteData;
		}

		public virtual void SetAuthPostData(Dictionary<string, object> dictData)
		{
			this.AuthPostData = dictData;
		}

		public virtual void AddAuthParameter(string key, string value)
		{
			string text = string.IsNullOrEmpty(this.AuthGetParameters) ? "" : "&";
			this.AuthGetParameters = string.Format("{0}{1}{2}={3}", new object[]
			{
				this.AuthGetParameters,
				text,
				Uri.EscapeDataString(key),
				Uri.EscapeDataString(value)
			});
		}

		public override string ToString()
		{
			return string.Format("AuthenticationValues = AuthType: {0} UserId: {1}{2}{3}{4}", new object[]
			{
				this.AuthType,
				this.UserId,
				string.IsNullOrEmpty(this.AuthGetParameters) ? " GetParameters: yes" : "",
				(this.AuthPostData == null) ? "" : " PostData: yes",
				(this.Token == null) ? "" : " Token: yes"
			});
		}

		public AuthenticationValues CopyTo(AuthenticationValues copy)
		{
			copy.AuthType = this.AuthType;
			copy.AuthGetParameters = this.AuthGetParameters;
			copy.AuthPostData = this.AuthPostData;
			copy.UserId = this.UserId;
			return copy;
		}

		private CustomAuthenticationType authType = CustomAuthenticationType.None;
	}
}
