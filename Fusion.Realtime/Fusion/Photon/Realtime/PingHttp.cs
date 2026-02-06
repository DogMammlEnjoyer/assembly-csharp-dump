using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Fusion.Photon.Realtime
{
	internal class PingHttp : PhotonPing
	{
		public override bool StartPing(string address)
		{
			base.Init();
			string arg = Application.isEditor ? "http://" : "https://";
			address = string.Format("{0}{1}/photon/m/?ping&r={2}", arg, address, Random.Range(0, 10000));
			this.webRequest = UnityWebRequest.Get(address);
			this.webRequest.SendWebRequest();
			return true;
		}

		public override bool Done()
		{
			bool isDone = this.webRequest.isDone;
			bool result;
			if (isDone)
			{
				this.Successful = true;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override void Dispose()
		{
			this.webRequest.Dispose();
		}

		private UnityWebRequest webRequest;
	}
}
