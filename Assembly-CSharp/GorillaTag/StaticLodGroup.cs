using System;
using UnityEngine;

namespace GorillaTag
{
	[DefaultExecutionOrder(2000)]
	public class StaticLodGroup : MonoBehaviour, IGorillaSimpleBackgroundWorker
	{
		protected void OnEnable()
		{
			if (this.initialized)
			{
				StaticLodManager.SetEnabled(this.index, true);
				return;
			}
			GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
		}

		protected void OnDisable()
		{
			if (this.initialized)
			{
				StaticLodManager.SetEnabled(this.index, false);
			}
		}

		private void OnDestroy()
		{
			if (this.initialized)
			{
				StaticLodManager.Unregister(this.index);
			}
		}

		public void SimpleWork()
		{
			if (this.initialized)
			{
				return;
			}
			this.index = StaticLodManager.Register(this);
			StaticLodManager.SetEnabled(this.index, true);
			this.initialized = true;
		}

		public const int k_monoDefaultExecutionOrder = 2000;

		private int index;

		public float collisionEnableDistance = 3f;

		public float uiFadeDistanceMax = 10f;

		private bool initialized;
	}
}
