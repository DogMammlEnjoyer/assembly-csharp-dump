using System;
using Newtonsoft.Json.Linq;

namespace Modio.Customizations
{
	[Serializable]
	internal struct WssMessage
	{
		public bool TryGetValue<TOutput>(out TOutput output) where TOutput : struct
		{
			JToken jtoken = this.context;
			if (jtoken != null)
			{
				output = jtoken.ToObject<TOutput>();
				return true;
			}
			output = default(TOutput);
			return false;
		}

		public string operation;

		public JToken context;
	}
}
