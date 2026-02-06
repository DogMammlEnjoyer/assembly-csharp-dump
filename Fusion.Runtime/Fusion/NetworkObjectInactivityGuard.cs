using System;
using UnityEngine;

namespace Fusion
{
	[AddComponentMenu("")]
	internal class NetworkObjectInactivityGuard : Behaviour
	{
		private void OnEnable()
		{
			bool flag = BehaviourUtils.IsNull(this.Object);
			if (!flag)
			{
				NetworkRunner runner = this.Object.Runner;
				this.Object = null;
				bool flag2 = runner;
				if (flag2)
				{
					TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
					if (logTraceObject != null)
					{
						logTraceObject.Log(this.Object, "NetworkObjectInactivityGuard: object has been activated, returning to a pool");
					}
					runner._inactivityGuardPool.Push(this);
					base.transform.SetParent(runner.transform);
				}
				else
				{
					TraceLogStream logTraceObject2 = InternalLogStreams.LogTraceObject;
					if (logTraceObject2 != null)
					{
						logTraceObject2.Log(this.Object, "NetworkObjectInactivityGuard: object has been activated but there's no runner, destroying");
					}
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		private void OnDestroy()
		{
			bool flag = BehaviourUtils.IsNull(this.Object);
			if (!flag)
			{
				bool flag2 = this.Object.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.HadAwake);
				if (!flag2)
				{
					TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
					if (logTraceObject != null)
					{
						logTraceObject.Log(this.Object, "NetworkObjectInactivityGuard: Invoking OnDestroyNeverActive");
					}
					this.Object.OnDestroyNeverActive();
				}
			}
		}

		[NonSerialized]
		public NetworkObject Object;
	}
}
