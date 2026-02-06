using System;

namespace Photon.Voice.Windows
{
	public class MonoPInvokeCallbackAttribute : Attribute
	{
		public MonoPInvokeCallbackAttribute(Type t)
		{
			this.type = t;
		}

		private Type type;
	}
}
