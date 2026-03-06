using System;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Operations
{
	internal class LocalizationGroupOperation : GroupOperation
	{
		protected override bool InvokeWaitForCompletion()
		{
			if (base.IsDone || base.Result == null)
			{
				return true;
			}
			for (int i = 0; i < base.Result.Count; i++)
			{
				base.Result[i].WaitForCompletion();
				if (base.Result == null)
				{
					return true;
				}
			}
			ResourceManager rm = this.m_RM;
			if (rm != null)
			{
				rm.Update(Time.unscaledDeltaTime);
			}
			if (!base.IsDone && base.Result != null)
			{
				this.Execute();
			}
			ResourceManager rm2 = this.m_RM;
			if (rm2 != null)
			{
				rm2.Update(Time.unscaledDeltaTime);
			}
			return base.IsDone;
		}

		protected override void Destroy()
		{
			LocalizationGroupOperation.Pool.Release(this);
			base.Destroy();
		}

		public static readonly ObjectPool<LocalizationGroupOperation> Pool = new ObjectPool<LocalizationGroupOperation>(() => new LocalizationGroupOperation(), null, null, null, false, 10, 10000);
	}
}
