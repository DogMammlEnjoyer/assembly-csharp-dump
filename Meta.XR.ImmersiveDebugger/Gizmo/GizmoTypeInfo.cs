using System;

namespace Meta.XR.ImmersiveDebugger.Gizmo
{
	internal struct GizmoTypeInfo
	{
		public GizmoTypeInfo(Action<object> renderDelegate)
		{
			this.RenderDelegate = renderDelegate;
		}

		public readonly Action<object> RenderDelegate;
	}
}
