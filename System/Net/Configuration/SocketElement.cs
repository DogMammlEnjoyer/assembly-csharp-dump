using System;
using System.Configuration;
using System.Net.Sockets;
using Unity;

namespace System.Net.Configuration
{
	/// <summary>Represents information used to configure <see cref="T:System.Net.Sockets.Socket" /> objects. This class cannot be inherited.</summary>
	public sealed class SocketElement : ConfigurationElement
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.SocketElement" /> class.</summary>
		public SocketElement()
		{
			SocketElement.alwaysUseCompletionPortsForAcceptProp = new ConfigurationProperty("alwaysUseCompletionPortsForAccept", typeof(bool), false);
			SocketElement.alwaysUseCompletionPortsForConnectProp = new ConfigurationProperty("alwaysUseCompletionPortsForConnect", typeof(bool), false);
			SocketElement.properties = new ConfigurationPropertyCollection();
			SocketElement.properties.Add(SocketElement.alwaysUseCompletionPortsForAcceptProp);
			SocketElement.properties.Add(SocketElement.alwaysUseCompletionPortsForConnectProp);
		}

		/// <summary>Gets or sets a Boolean value that specifies whether completion ports are used when accepting connections.</summary>
		/// <returns>
		///   <see langword="true" /> to use completion ports; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("alwaysUseCompletionPortsForAccept", DefaultValue = "False")]
		public bool AlwaysUseCompletionPortsForAccept
		{
			get
			{
				return (bool)base[SocketElement.alwaysUseCompletionPortsForAcceptProp];
			}
			set
			{
				base[SocketElement.alwaysUseCompletionPortsForAcceptProp] = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that specifies whether completion ports are used when making connections.</summary>
		/// <returns>
		///   <see langword="true" /> to use completion ports; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("alwaysUseCompletionPortsForConnect", DefaultValue = "False")]
		public bool AlwaysUseCompletionPortsForConnect
		{
			get
			{
				return (bool)base[SocketElement.alwaysUseCompletionPortsForConnectProp];
			}
			set
			{
				base[SocketElement.alwaysUseCompletionPortsForConnectProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return SocketElement.properties;
			}
		}

		[MonoTODO]
		protected override void PostDeserialize()
		{
		}

		/// <summary>Gets or sets a value that specifies the default <see cref="T:System.Net.Sockets.IPProtectionLevel" /> to use for a socket.</summary>
		/// <returns>The value of the <see cref="T:System.Net.Sockets.IPProtectionLevel" /> for the current instance.</returns>
		public IPProtectionLevel IPProtectionLevel
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return (IPProtectionLevel)0;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty alwaysUseCompletionPortsForAcceptProp;

		private static ConfigurationProperty alwaysUseCompletionPortsForConnectProp;
	}
}
