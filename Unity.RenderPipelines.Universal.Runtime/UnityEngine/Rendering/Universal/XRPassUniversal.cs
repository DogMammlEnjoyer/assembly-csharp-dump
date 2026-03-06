using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	internal class XRPassUniversal : XRPass
	{
		public static XRPass Create(XRPassCreateInfo createInfo)
		{
			XRPassUniversal xrpassUniversal = GenericPool<XRPassUniversal>.Get();
			xrpassUniversal.InitBase(createInfo);
			xrpassUniversal.isLateLatchEnabled = false;
			xrpassUniversal.canMarkLateLatch = false;
			xrpassUniversal.hasMarkedLateLatch = false;
			xrpassUniversal.canFoveateIntermediatePasses = true;
			return xrpassUniversal;
		}

		public override void Release()
		{
			GenericPool<XRPassUniversal>.Release(this);
		}

		internal bool isLateLatchEnabled { get; set; }

		internal bool canMarkLateLatch { get; set; }

		internal bool hasMarkedLateLatch { get; set; }

		internal bool canFoveateIntermediatePasses { get; set; }
	}
}
