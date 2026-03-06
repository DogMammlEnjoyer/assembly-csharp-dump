using System;
using System.Collections.Generic;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	internal abstract class ProxyController<ControllerType> where ControllerType : Controller
	{
		public ControllerType Target { get; private set; }

		public void Fill(ControllerType target, Dictionary<ControllerType, ProxyController<ControllerType>> targets)
		{
			ProxyController<ControllerType> proxyController;
			if (targets.TryGetValue(target, out proxyController) && proxyController == this)
			{
				return;
			}
			targets[target] = this;
			this.Target = target;
			this.Fill();
			this.Target.RefreshLayout();
		}

		protected abstract void Fill();
	}
}
