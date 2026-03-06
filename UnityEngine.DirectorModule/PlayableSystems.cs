using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[StaticAccessor("PlayableSystemsBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/Director/ScriptBindings/PlayableSystems.bindings.h")]
	internal static class PlayableSystems
	{
		public static void RegisterSystemPhaseDelegate<TDataStream>(PlayableSystems.PlayableSystemStage stage, PlayableSystems.PlayableSystemDelegate systemDelegate) where TDataStream : new()
		{
			PlayableSystems.RegisterSystemPhaseDelegate(typeof(TDataStream), stage, systemDelegate);
		}

		private static void RegisterSystemPhaseDelegate(Type streamType, PlayableSystems.PlayableSystemStage stage, PlayableSystems.PlayableSystemDelegate systemDelegate)
		{
			int num = PlayableSystems.RegisterStreamStage(streamType, (int)stage);
			try
			{
				PlayableSystems.s_RWLock.EnterWriteLock();
				PlayableSystems.s_SystemTypes.TryAdd(num, streamType);
				int key = PlayableSystems.CombineTypeAndIndex(num, stage);
				bool flag = !PlayableSystems.s_Delegates.TryAdd(key, systemDelegate);
				if (flag)
				{
					PlayableSystems.s_Delegates[key] = systemDelegate;
				}
			}
			finally
			{
				PlayableSystems.s_RWLock.ExitWriteLock();
			}
		}

		private static int CombineTypeAndIndex(int typeIndex, PlayableSystems.PlayableSystemStage stage)
		{
			return typeIndex << 16 | (int)stage;
		}

		[RequiredByNativeCode]
		private unsafe static bool Internal_CallSystemDelegate(int systemIndex, PlayableSystems.PlayableSystemStage stage, IntPtr outputsPtr, int numOutputs)
		{
			PlayableOutputHandle* outputs = (PlayableOutputHandle*)((void*)outputsPtr);
			int key = PlayableSystems.CombineTypeAndIndex(systemIndex, stage);
			bool flag = false;
			PlayableSystems.PlayableSystemDelegate playableSystemDelegate = null;
			PlayableSystems.s_RWLock.EnterReadLock();
			Type type;
			bool flag2 = PlayableSystems.s_SystemTypes.TryGetValue(systemIndex, out type);
			bool flag3 = flag2;
			if (flag3)
			{
				flag = (PlayableSystems.s_Delegates.TryGetValue(key, out playableSystemDelegate) && playableSystemDelegate != null);
			}
			PlayableSystems.s_RWLock.ExitReadLock();
			bool flag4 = !flag2 || !flag;
			bool result;
			if (flag4)
			{
				result = false;
			}
			else
			{
				PlayableSystems.DataPlayableOutputList outputs2 = new PlayableSystems.DataPlayableOutputList(outputs, numOutputs);
				playableSystemDelegate(outputs2);
				result = true;
			}
			return result;
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RegisterStreamStage(Type streamType, int stage);

		static PlayableSystems()
		{
			PlayableSystems.s_SystemTypes = new Dictionary<int, Type>();
			PlayableSystems.s_RWLock = new ReaderWriterLockSlim();
		}

		private static Dictionary<int, Type> s_SystemTypes;

		private static Dictionary<int, PlayableSystems.PlayableSystemDelegate> s_Delegates = new Dictionary<int, PlayableSystems.PlayableSystemDelegate>();

		private static ReaderWriterLockSlim s_RWLock;

		public delegate void PlayableSystemDelegate(IReadOnlyList<DataPlayableOutput> outputs);

		public enum PlayableSystemStage : ushort
		{
			FixedUpdate,
			FixedUpdatePostPhysics,
			Update,
			AnimationBegin,
			AnimationEnd,
			LateUpdate,
			Render
		}

		private class DataPlayableOutputList : IReadOnlyList<DataPlayableOutput>, IEnumerable<DataPlayableOutput>, IEnumerable, IReadOnlyCollection<DataPlayableOutput>
		{
			public unsafe DataPlayableOutputList(PlayableOutputHandle* outputs, int count)
			{
				this.m_Outputs = outputs;
				this.m_Count = count;
			}

			public unsafe DataPlayableOutput this[int index]
			{
				get
				{
					bool flag = index >= this.m_Count;
					if (flag)
					{
						throw new IndexOutOfRangeException(string.Format("index {0} is greater than the number of items: {1}", index, this.m_Count));
					}
					bool flag2 = index < 0;
					if (flag2)
					{
						throw new IndexOutOfRangeException("index cannot be negative");
					}
					return new DataPlayableOutput(this.m_Outputs[index]);
				}
			}

			public int Count
			{
				get
				{
					return this.m_Count;
				}
			}

			public IEnumerator<DataPlayableOutput> GetEnumerator()
			{
				return new PlayableSystems.DataPlayableOutputList.DataPlayableOutputEnumerator(this);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private unsafe PlayableOutputHandle* m_Outputs;

			private int m_Count;

			private class DataPlayableOutputEnumerator : IEnumerator<DataPlayableOutput>, IEnumerator, IDisposable
			{
				public DataPlayableOutputEnumerator(PlayableSystems.DataPlayableOutputList list)
				{
					this.m_List = list;
					this.m_Index = -1;
				}

				public DataPlayableOutput Current
				{
					get
					{
						DataPlayableOutput result;
						try
						{
							result = this.m_List[this.m_Index];
						}
						catch (IndexOutOfRangeException)
						{
							throw new InvalidOperationException("Enumeration has either not started or has already finished.");
						}
						return result;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return this.Current;
					}
				}

				public void Dispose()
				{
					this.m_List = null;
				}

				public bool MoveNext()
				{
					this.m_Index++;
					return this.m_Index < this.m_List.Count;
				}

				public void Reset()
				{
					this.m_Index = -1;
				}

				private PlayableSystems.DataPlayableOutputList m_List;

				private int m_Index;
			}
		}
	}
}
