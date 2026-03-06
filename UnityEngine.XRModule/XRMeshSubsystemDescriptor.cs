using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[UsedByNativeCode]
	[NativeType(Header = "Modules/XR/Subsystems/Planes/XRMeshSubsystemDescriptor.h")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	public class XRMeshSubsystemDescriptor : IntegratedSubsystemDescriptor<XRMeshSubsystem>
	{
		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(XRMeshSubsystemDescriptor descriptor)
			{
				return descriptor.m_Ptr;
			}
		}
	}
}
