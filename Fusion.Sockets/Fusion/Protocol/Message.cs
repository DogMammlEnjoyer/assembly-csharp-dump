using System;

namespace Fusion.Protocol
{
	internal abstract class Message : IMessage
	{
		public virtual bool IsValid
		{
			get
			{
				return this.HasValidVersion;
			}
		}

		internal bool HasValidVersion
		{
			get
			{
				return this.ProtocolVersion != ProtocolMessageVersion.Invalid && this.FusionSerializationVersion != Versioning.InvalidVersion;
			}
		}

		public string CustomData
		{
			get
			{
				return this._customData;
			}
			set
			{
				Assert.Always(value.Length <= 1024, string.Format("Protocol Message Custom Data size is greater than {0}", 1024));
				this._customData = value.Substring(0, Math.Min(value.Length, 1024));
			}
		}

		public virtual Message Clone()
		{
			return (Message)base.MemberwiseClone();
		}

		public Message(ProtocolMessageVersion protocolMessage = ProtocolMessageVersion.V1_6_0, Version serializationVersion = null)
		{
			this.ProtocolVersion = protocolMessage;
			this.FusionSerializationVersion = (serializationVersion ?? Versioning.GetCurrentVersion);
		}

		public void Serialize(BitStream stream)
		{
			byte b = (byte)this.ProtocolVersion;
			int major = this.FusionSerializationVersion.Major;
			int minor = this.FusionSerializationVersion.Minor;
			int build = this.FusionSerializationVersion.Build;
			stream.Serialize(ref b);
			b = Math.Min(10, b);
			bool flag = b >= 2;
			if (flag)
			{
				stream.Serialize(ref major);
				stream.Serialize(ref minor);
				stream.Serialize(ref build);
			}
			else
			{
				major = 0;
				minor = 0;
				build = 0;
			}
			this.ProtocolVersion = (ProtocolMessageVersion)b;
			this.FusionSerializationVersion = new Version(major, minor, build);
			bool flag2 = this.FusionSerializationVersion.ShortVersion() == Versioning.GetCurrentVersion.ShortVersion();
			if (flag2)
			{
				this.SerializeProtected(stream);
				bool flag3 = b >= 3;
				if (flag3)
				{
					stream.Serialize(ref this._customData);
				}
				else
				{
					this._customData = string.Empty;
				}
			}
		}

		protected virtual void SerializeProtected(BitStream stream)
		{
		}

		public bool CheckCompatibility(ProtocolMessageVersion pluginProtocolVersion, Version pluginVersion, Version sessionSerializationVersion)
		{
			bool flag = this.ProtocolVersion == ProtocolMessageVersion.Invalid || pluginProtocolVersion == ProtocolMessageVersion.Invalid || this.ProtocolVersion != pluginProtocolVersion;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = pluginProtocolVersion < ProtocolMessageVersion.V1_1_0;
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = this.FusionSerializationVersion == Versioning.InvalidVersion || sessionSerializationVersion == Versioning.InvalidVersion || this.FusionSerializationVersion.ShortVersion() > pluginVersion.ShortVersion() || this.FusionSerializationVersion.ShortVersion() < new Version(1, 0);
					result = (!flag3 && this.FusionSerializationVersion == sessionSerializationVersion);
				}
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("{0}={1}, {2}={3}, {4}={5}", new object[]
			{
				"ProtocolVersion",
				this.ProtocolVersion,
				"FusionSerializationVersion",
				this.FusionSerializationVersion,
				"CustomData",
				this.CustomData
			});
		}

		private const int CustomDataLength = 1024;

		public ProtocolMessageVersion ProtocolVersion;

		public Version FusionSerializationVersion;

		private string _customData = string.Empty;
	}
}
