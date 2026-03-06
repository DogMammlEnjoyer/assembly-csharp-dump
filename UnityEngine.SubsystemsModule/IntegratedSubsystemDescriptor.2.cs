using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode("SubsystemDescriptor")]
	[NativeHeader("Modules/Subsystems/SubsystemDescriptor.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class IntegratedSubsystemDescriptor<TSubsystem> : IntegratedSubsystemDescriptor where TSubsystem : IntegratedSubsystem
	{
		internal override ISubsystem CreateImpl()
		{
			return this.Create();
		}

		public TSubsystem Create()
		{
			IntPtr ptr = SubsystemDescriptorBindings.Create(this.m_Ptr);
			TSubsystem tsubsystem = (TSubsystem)((object)SubsystemManager.GetIntegratedSubsystemByPtr(ptr));
			bool flag = tsubsystem != null;
			if (flag)
			{
				tsubsystem.m_SubsystemDescriptor = this;
			}
			return tsubsystem;
		}
	}
}
