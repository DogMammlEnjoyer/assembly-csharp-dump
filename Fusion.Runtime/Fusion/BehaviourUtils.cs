using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal static class BehaviourUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNull(Behaviour obj)
		{
			return obj == null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNotNull(Behaviour obj)
		{
			return obj != null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(NetworkRunner obj)
		{
			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNotAlive(NetworkRunner obj)
		{
			return !obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(SimulationBehaviour obj)
		{
			return obj != null && (obj.Flags & SimulationBehaviourRuntimeFlags.IsUnityDestroyed) == (SimulationBehaviourRuntimeFlags)0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNotAlive(SimulationBehaviour obj)
		{
			return obj == null || (obj.Flags & SimulationBehaviourRuntimeFlags.IsUnityDestroyed) == SimulationBehaviourRuntimeFlags.IsUnityDestroyed;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(NetworkObject obj)
		{
			return obj != null && (obj.RuntimeFlags & NetworkObjectRuntimeFlags.IsDestroyed) == NetworkObjectRuntimeFlags.None;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNotAlive(NetworkObject obj)
		{
			return obj == null || (obj.RuntimeFlags & NetworkObjectRuntimeFlags.IsDestroyed) == NetworkObjectRuntimeFlags.IsDestroyed;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSame(Behaviour a, Behaviour b)
		{
			return a == b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSameNotNull(Behaviour a, Behaviour b)
		{
			return a != null && a == b;
		}

		public static BehaviourUtils.NameDeferred GetName(Behaviour obj)
		{
			return new BehaviourUtils.NameDeferred(obj);
		}

		public static BehaviourUtils.DeferredJoin Join(IEnumerable objects)
		{
			return new BehaviourUtils.DeferredJoin
			{
				_enumerable = objects
			};
		}

		public struct DeferredJoin
		{
			public override string ToString()
			{
				return string.Join<object>(", ", this._enumerable.Cast<object>());
			}

			public IEnumerable _enumerable;
		}

		internal struct NameDeferred
		{
			public NameDeferred(Behaviour behaviour)
			{
				this._behaviour = behaviour;
			}

			public static explicit operator BehaviourUtils.NameDeferred(Behaviour behaviour)
			{
				return new BehaviourUtils.NameDeferred(behaviour);
			}

			public static implicit operator string(BehaviourUtils.NameDeferred wrapper)
			{
				return wrapper.ToString();
			}

			public override string ToString()
			{
				bool flag = BehaviourUtils.IsNull(this._behaviour);
				string result;
				if (flag)
				{
					result = "(null)";
				}
				else
				{
					result = this._behaviour.DebugNameThreadSafe;
				}
				return result;
			}

			private Behaviour _behaviour;
		}
	}
}
