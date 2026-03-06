using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.Layout
{
	internal class LayoutProcessorNative : ILayoutProcessor
	{
		unsafe void ILayoutProcessor.CalculateLayout(LayoutNode node, float parentWidth, float parentHeight, LayoutDirection parentDirection)
		{
			IntPtr node2 = (IntPtr)((void*)(&node));
			IntPtr zero = IntPtr.Zero;
			fixed (LayoutState* ptr = &this.m_State)
			{
				void* value = (void*)ptr;
				IntPtr state = (IntPtr)value;
				LayoutNative.CalculateLayout(node2, parentWidth, parentHeight, (int)parentDirection, state, (IntPtr)((void*)(&zero)));
				bool flag = zero != IntPtr.Zero;
				if (flag)
				{
					GCHandle gchandle = GCHandle.FromIntPtr(zero);
					Exception source = gchandle.Target as Exception;
					gchandle.Free();
					this.m_State.error = false;
					ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(source);
					exceptionDispatchInfo.Throw();
				}
			}
		}

		private LayoutState m_State = LayoutState.Default;
	}
}
