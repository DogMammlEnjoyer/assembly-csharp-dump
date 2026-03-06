using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[NativeHeader("Runtime/Export/Director/PlayableHandle.bindings.h")]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
	[UsedByNativeCode]
	public struct PlayableHandle : IEquatable<PlayableHandle>
	{
		internal T GetObject<T>() where T : class, IPlayableBehaviour
		{
			bool flag = !this.IsValid();
			T result;
			if (flag)
			{
				result = default(T);
			}
			else
			{
				object scriptInstance = this.GetScriptInstance();
				bool flag2 = scriptInstance == null;
				if (flag2)
				{
					result = default(T);
				}
				else
				{
					result = (T)((object)scriptInstance);
				}
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.DirectorModule"
		})]
		internal T GetPayload<T>() where T : struct
		{
			bool flag = !this.IsValid();
			T result;
			if (flag)
			{
				result = default(T);
			}
			else
			{
				object scriptInstance = this.GetScriptInstance();
				bool flag2 = scriptInstance == null;
				if (flag2)
				{
					result = default(T);
				}
				else
				{
					result = (T)((object)scriptInstance);
				}
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.DirectorModule"
		})]
		internal void SetPayload<T>(T payload) where T : struct
		{
			bool flag = !this.IsValid();
			if (!flag)
			{
				this.SetScriptInstance(payload);
			}
		}

		[VisibleToOtherModules]
		internal bool IsPlayableOfType<T>()
		{
			return this.GetPlayableType() == typeof(T);
		}

		public static PlayableHandle Null
		{
			get
			{
				return PlayableHandle.m_Null;
			}
		}

		internal Playable GetInput(int inputPort)
		{
			return new Playable(this.GetInputHandle(inputPort));
		}

		internal Playable GetOutput(int outputPort)
		{
			return new Playable(this.GetOutputHandle(outputPort));
		}

		internal int GetOutputPortFromInputConnection(int inputPort)
		{
			return this.GetOutputPortFromInputIndex(inputPort);
		}

		internal int GetInputPortFromOutputConnection(int inputPort)
		{
			return this.GetInputPortFromOutputIndex(inputPort);
		}

		internal bool SetInputWeight(int inputIndex, float weight)
		{
			bool flag = this.CheckInputBounds(inputIndex);
			bool result;
			if (flag)
			{
				this.SetInputWeightFromIndex(inputIndex, weight);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal float GetInputWeight(int inputIndex)
		{
			bool flag = this.CheckInputBounds(inputIndex);
			float result;
			if (flag)
			{
				result = this.GetInputWeightFromIndex(inputIndex);
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		internal void Destroy()
		{
			this.GetGraph().DestroyPlayable<Playable>(new Playable(this));
		}

		public static bool operator ==(PlayableHandle x, PlayableHandle y)
		{
			return PlayableHandle.CompareVersion(x, y);
		}

		public static bool operator !=(PlayableHandle x, PlayableHandle y)
		{
			return !PlayableHandle.CompareVersion(x, y);
		}

		public override bool Equals(object p)
		{
			return p is PlayableHandle && this.Equals((PlayableHandle)p);
		}

		public bool Equals(PlayableHandle other)
		{
			return PlayableHandle.CompareVersion(this, other);
		}

		public override int GetHashCode()
		{
			return this.m_Handle.GetHashCode() ^ this.m_Version.GetHashCode();
		}

		internal static bool CompareVersion(PlayableHandle lhs, PlayableHandle rhs)
		{
			return lhs.m_Handle == rhs.m_Handle && lhs.m_Version == rhs.m_Version;
		}

		internal bool CheckInputBounds(int inputIndex)
		{
			return this.CheckInputBounds(inputIndex, false);
		}

		internal bool CheckInputBounds(int inputIndex, bool acceptAny)
		{
			bool flag = inputIndex == -1 && acceptAny;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = inputIndex < 0;
				if (flag2)
				{
					throw new IndexOutOfRangeException("Index must be greater than 0");
				}
				bool flag3 = this.GetInputCount() <= inputIndex;
				if (flag3)
				{
					throw new IndexOutOfRangeException(string.Concat(new string[]
					{
						"inputIndex ",
						inputIndex.ToString(),
						" is greater than the number of available inputs (",
						this.GetInputCount().ToString(),
						")."
					}));
				}
				result = true;
			}
			return result;
		}

		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsNull();

		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsValid();

		[FreeFunction("PlayableHandleBindings::GetPlayableType", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Type GetPlayableType();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::GetJobType", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Type GetJobType();

		[FreeFunction("PlayableHandleBindings::SetScriptInstance", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetScriptInstance(object scriptInstance);

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::CanChangeInputs", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool CanChangeInputs();

		[FreeFunction("PlayableHandleBindings::CanSetWeights", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool CanSetWeights();

		[FreeFunction("PlayableHandleBindings::CanDestroy", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool CanDestroy();

		[FreeFunction("PlayableHandleBindings::GetPlayState", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern PlayState GetPlayState();

		[FreeFunction("PlayableHandleBindings::Play", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Play();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::Pause", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Pause();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::GetSpeed", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern double GetSpeed();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::SetSpeed", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetSpeed(double value);

		[FreeFunction("PlayableHandleBindings::GetTime", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern double GetTime();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::SetTime", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetTime(double value);

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::IsDone", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsDone();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::SetDone", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetDone(bool value);

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::GetDuration", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern double GetDuration();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::SetDuration", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetDuration(double value);

		[FreeFunction("PlayableHandleBindings::GetPropagateSetTime", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetPropagateSetTime();

		[FreeFunction("PlayableHandleBindings::SetPropagateSetTime", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetPropagateSetTime(bool value);

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::GetGraph", HasExplicitThis = true, ThrowsException = true)]
		internal PlayableGraph GetGraph()
		{
			PlayableGraph result;
			PlayableHandle.GetGraph_Injected(ref this, out result);
			return result;
		}

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::GetInputCount", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetInputCount();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::GetOutputPortFromInputIndex", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetOutputPortFromInputIndex(int index);

		[FreeFunction("PlayableHandleBindings::GetInputPortFromOutputIndex", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetInputPortFromOutputIndex(int index);

		[FreeFunction("PlayableHandleBindings::SetInputCount", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetInputCount(int value);

		[FreeFunction("PlayableHandleBindings::GetOutputCount", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetOutputCount();

		[FreeFunction("PlayableHandleBindings::SetOutputCount", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetOutputCount(int value);

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::SetInputWeight", HasExplicitThis = true, ThrowsException = true)]
		internal void SetInputWeight(PlayableHandle input, float weight)
		{
			PlayableHandle.SetInputWeight_Injected(ref this, ref input, weight);
		}

		[FreeFunction("PlayableHandleBindings::SetDelay", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetDelay(double delay);

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::GetDelay", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern double GetDelay();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::IsDelayed", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsDelayed();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::GetPreviousTime", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern double GetPreviousTime();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::SetLeadTime", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetLeadTime(float value);

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::GetLeadTime", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetLeadTime();

		[FreeFunction("PlayableHandleBindings::GetTraversalMode", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern PlayableTraversalMode GetTraversalMode();

		[FreeFunction("PlayableHandleBindings::SetTraversalMode", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetTraversalMode(PlayableTraversalMode mode);

		[FreeFunction("PlayableHandleBindings::GetJobData", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetJobData();

		[FreeFunction("PlayableHandleBindings::GetTimeWrapMode", HasExplicitThis = true, ThrowsException = true)]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern DirectorWrapMode GetTimeWrapMode();

		[VisibleToOtherModules]
		[FreeFunction("PlayableHandleBindings::SetTimeWrapMode", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetTimeWrapMode(DirectorWrapMode mode);

		[FreeFunction("PlayableHandleBindings::GetScriptInstance", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object GetScriptInstance();

		[FreeFunction("PlayableHandleBindings::GetInputHandle", HasExplicitThis = true, ThrowsException = true)]
		private PlayableHandle GetInputHandle(int index)
		{
			PlayableHandle result;
			PlayableHandle.GetInputHandle_Injected(ref this, index, out result);
			return result;
		}

		[FreeFunction("PlayableHandleBindings::GetOutputHandle", HasExplicitThis = true, ThrowsException = true)]
		private PlayableHandle GetOutputHandle(int index)
		{
			PlayableHandle result;
			PlayableHandle.GetOutputHandle_Injected(ref this, index, out result);
			return result;
		}

		[FreeFunction("PlayableHandleBindings::SetInputWeightFromIndex", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetInputWeightFromIndex(int index, float weight);

		[FreeFunction("PlayableHandleBindings::GetInputWeightFromIndex", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetInputWeightFromIndex(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGraph_Injected(ref PlayableHandle _unity_self, out PlayableGraph ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetInputWeight_Injected(ref PlayableHandle _unity_self, [In] ref PlayableHandle input, float weight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetInputHandle_Injected(ref PlayableHandle _unity_self, int index, out PlayableHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetOutputHandle_Injected(ref PlayableHandle _unity_self, int index, out PlayableHandle ret);

		internal IntPtr m_Handle;

		internal uint m_Version;

		private static readonly PlayableHandle m_Null = default(PlayableHandle);
	}
}
