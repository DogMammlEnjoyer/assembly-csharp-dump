using System;

namespace Fusion.Photon.Realtime
{
	internal class WebFlags
	{
		public bool HttpForward
		{
			get
			{
				return (this.WebhookFlags & 1) > 0;
			}
			set
			{
				if (value)
				{
					this.WebhookFlags |= 1;
				}
				else
				{
					this.WebhookFlags = (byte)((int)this.WebhookFlags & -2);
				}
			}
		}

		public bool SendAuthCookie
		{
			get
			{
				return (this.WebhookFlags & 2) > 0;
			}
			set
			{
				if (value)
				{
					this.WebhookFlags |= 2;
				}
				else
				{
					this.WebhookFlags = (byte)((int)this.WebhookFlags & -3);
				}
			}
		}

		public bool SendSync
		{
			get
			{
				return (this.WebhookFlags & 4) > 0;
			}
			set
			{
				if (value)
				{
					this.WebhookFlags |= 4;
				}
				else
				{
					this.WebhookFlags = (byte)((int)this.WebhookFlags & -5);
				}
			}
		}

		public bool SendState
		{
			get
			{
				return (this.WebhookFlags & 8) > 0;
			}
			set
			{
				if (value)
				{
					this.WebhookFlags |= 8;
				}
				else
				{
					this.WebhookFlags = (byte)((int)this.WebhookFlags & -9);
				}
			}
		}

		public WebFlags(byte webhookFlags)
		{
			this.WebhookFlags = webhookFlags;
		}

		public static readonly WebFlags Default = new WebFlags(0);

		public byte WebhookFlags;

		public const byte HttpForwardConst = 1;

		public const byte SendAuthCookieConst = 2;

		public const byte SendSyncConst = 4;

		public const byte SendStateConst = 8;
	}
}
