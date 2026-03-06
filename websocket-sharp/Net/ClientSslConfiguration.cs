using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace WebSocketSharp.Net
{
	public class ClientSslConfiguration
	{
		public ClientSslConfiguration(string targetHost)
		{
			bool flag = targetHost == null;
			if (flag)
			{
				throw new ArgumentNullException("targetHost");
			}
			bool flag2 = targetHost.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("An empty string.", "targetHost");
			}
			this._targetHost = targetHost;
			this._enabledSslProtocols = SslProtocols.None;
		}

		public ClientSslConfiguration(ClientSslConfiguration configuration)
		{
			bool flag = configuration == null;
			if (flag)
			{
				throw new ArgumentNullException("configuration");
			}
			this._checkCertRevocation = configuration._checkCertRevocation;
			this._clientCertSelectionCallback = configuration._clientCertSelectionCallback;
			this._clientCerts = configuration._clientCerts;
			this._enabledSslProtocols = configuration._enabledSslProtocols;
			this._serverCertValidationCallback = configuration._serverCertValidationCallback;
			this._targetHost = configuration._targetHost;
		}

		public bool CheckCertificateRevocation
		{
			get
			{
				return this._checkCertRevocation;
			}
			set
			{
				this._checkCertRevocation = value;
			}
		}

		public X509CertificateCollection ClientCertificates
		{
			get
			{
				return this._clientCerts;
			}
			set
			{
				this._clientCerts = value;
			}
		}

		public LocalCertificateSelectionCallback ClientCertificateSelectionCallback
		{
			get
			{
				bool flag = this._clientCertSelectionCallback == null;
				if (flag)
				{
					this._clientCertSelectionCallback = new LocalCertificateSelectionCallback(ClientSslConfiguration.defaultSelectClientCertificate);
				}
				return this._clientCertSelectionCallback;
			}
			set
			{
				this._clientCertSelectionCallback = value;
			}
		}

		public SslProtocols EnabledSslProtocols
		{
			get
			{
				return this._enabledSslProtocols;
			}
			set
			{
				this._enabledSslProtocols = value;
			}
		}

		public RemoteCertificateValidationCallback ServerCertificateValidationCallback
		{
			get
			{
				bool flag = this._serverCertValidationCallback == null;
				if (flag)
				{
					this._serverCertValidationCallback = new RemoteCertificateValidationCallback(ClientSslConfiguration.defaultValidateServerCertificate);
				}
				return this._serverCertValidationCallback;
			}
			set
			{
				this._serverCertValidationCallback = value;
			}
		}

		public string TargetHost
		{
			get
			{
				return this._targetHost;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				bool flag2 = value.Length == 0;
				if (flag2)
				{
					throw new ArgumentException("An empty string.", "value");
				}
				this._targetHost = value;
			}
		}

		private static X509Certificate defaultSelectClientCertificate(object sender, string targetHost, X509CertificateCollection clientCertificates, X509Certificate serverCertificate, string[] acceptableIssuers)
		{
			return null;
		}

		private static bool defaultValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		private bool _checkCertRevocation;

		private LocalCertificateSelectionCallback _clientCertSelectionCallback;

		private X509CertificateCollection _clientCerts;

		private SslProtocols _enabledSslProtocols;

		private RemoteCertificateValidationCallback _serverCertValidationCallback;

		private string _targetHost;
	}
}
