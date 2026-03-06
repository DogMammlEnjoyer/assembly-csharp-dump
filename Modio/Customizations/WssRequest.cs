using System;
using Newtonsoft.Json.Linq;

namespace Modio.Customizations
{
	internal static class WssRequest
	{
		public static WssMessage DeviceLogin()
		{
			return new WssMessage
			{
				operation = "device_login",
				context = JToken.FromObject(default(WssDeviceLoginRequest))
			};
		}
	}
}
