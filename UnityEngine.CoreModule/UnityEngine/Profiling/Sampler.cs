using System;
using System.Collections.Generic;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	[NativeHeader("Runtime/Profiler/ScriptBindings/Sampler.bindings.h")]
	[UsedByNativeCode]
	public class Sampler
	{
		internal Sampler()
		{
		}

		internal Sampler(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		public bool isValid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		public Recorder GetRecorder()
		{
			ProfilerRecorderHandle handle = new ProfilerRecorderHandle((ulong)this.m_Ptr.ToInt64());
			return new Recorder(handle);
		}

		public static Sampler Get(string name)
		{
			IntPtr marker = ProfilerUnsafeUtility.GetMarker(name);
			bool flag = marker == IntPtr.Zero;
			Sampler result;
			if (flag)
			{
				result = Sampler.s_InvalidSampler;
			}
			else
			{
				result = new Sampler(marker);
			}
			return result;
		}

		public static int GetNames(List<string> names)
		{
			List<ProfilerRecorderHandle> list = new List<ProfilerRecorderHandle>();
			ProfilerRecorderHandle.GetAvailable(list);
			bool flag = names != null;
			if (flag)
			{
				bool flag2 = names.Count < list.Count;
				if (flag2)
				{
					names.Capacity = list.Count;
					for (int i = names.Count; i < list.Count; i++)
					{
						names.Add(null);
					}
				}
				int num = 0;
				foreach (ProfilerRecorderHandle handle in list)
				{
					names[num] = ProfilerRecorderHandle.GetDescription(handle).Name;
					num++;
				}
			}
			return list.Count;
		}

		public string name
		{
			get
			{
				return ProfilerUnsafeUtility.Internal_GetName(this.m_Ptr);
			}
		}

		internal IntPtr m_Ptr;

		internal static Sampler s_InvalidSampler = new Sampler();
	}
}
