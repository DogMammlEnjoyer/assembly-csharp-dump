using System;
using UnityEngine.Networking;

namespace Backtrace.Unity.Model
{
	public class BacktraceSelfSSLCertificateHandler : CertificateHandler
	{
		protected override bool ValidateCertificate(byte[] certificateData)
		{
			return true;
		}

		private static readonly string PUB_KEY = string.Empty;
	}
}
