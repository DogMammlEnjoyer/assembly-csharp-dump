using System;

namespace Oculus.Platform.Models
{
	public class Pid
	{
		public Pid(IntPtr o)
		{
			this.Id = CAPI.ovr_Pid_GetId(o);
		}

		public readonly string Id;
	}
}
