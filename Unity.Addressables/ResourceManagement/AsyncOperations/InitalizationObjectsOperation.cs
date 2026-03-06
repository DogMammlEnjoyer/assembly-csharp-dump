using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
	internal class InitalizationObjectsOperation : AsyncOperationBase<bool>
	{
		public void Init(AsyncOperationHandle<ResourceManagerRuntimeData> rtdOp, AddressablesImpl addressables)
		{
			this.m_RtdOp = rtdOp;
			this.m_Addressables = addressables;
			this.m_Addressables.ResourceManager.RegisterForCallbacks();
		}

		protected override string DebugName
		{
			get
			{
				return "InitializationObjectsOperation";
			}
		}

		internal bool LogRuntimeWarnings(string pathToBuildLogs)
		{
			if (!File.Exists(pathToBuildLogs))
			{
				return false;
			}
			PackedPlayModeBuildLogs packedPlayModeBuildLogs = JsonUtility.FromJson<PackedPlayModeBuildLogs>(File.ReadAllText(pathToBuildLogs));
			bool result = false;
			foreach (PackedPlayModeBuildLogs.RuntimeBuildLog runtimeBuildLog in packedPlayModeBuildLogs.RuntimeBuildLogs)
			{
				result = true;
				switch (runtimeBuildLog.Type)
				{
				case LogType.Error:
					Addressables.LogError(runtimeBuildLog.Message);
					break;
				case LogType.Warning:
					Addressables.LogWarning(runtimeBuildLog.Message);
					break;
				}
			}
			return result;
		}

		protected override bool InvokeWaitForCompletion()
		{
			if (base.IsDone)
			{
				return true;
			}
			if (this.m_RtdOp.IsValid() && !this.m_RtdOp.IsDone)
			{
				this.m_RtdOp.WaitForCompletion();
			}
			ResourceManager rm = this.m_RM;
			if (rm != null)
			{
				rm.Update(Time.unscaledDeltaTime);
			}
			if (!this.HasExecuted)
			{
				base.InvokeExecute();
			}
			if (this.m_DepOp.IsValid() && !this.m_DepOp.IsDone)
			{
				this.m_DepOp.WaitForCompletion();
			}
			ResourceManager rm2 = this.m_RM;
			if (rm2 != null)
			{
				rm2.Update(Time.unscaledDeltaTime);
			}
			return base.IsDone;
		}

		protected override void Execute()
		{
			ResourceManagerRuntimeData result = this.m_RtdOp.Result;
			if (result == null)
			{
				Addressables.LogError("RuntimeData is null.  Please ensure you have built the correct Player Content.");
				base.Complete(true, true, "");
				return;
			}
			List<AsyncOperationHandle> list = new List<AsyncOperationHandle>();
			foreach (ObjectInitializationData objectInitializationData in result.InitializationObjects)
			{
				if (!(objectInitializationData.ObjectType.Value == null))
				{
					try
					{
						AsyncOperationHandle asyncInitHandle = objectInitializationData.GetAsyncInitHandle(this.m_Addressables.ResourceManager, null);
						list.Add(asyncInitHandle);
					}
					catch (Exception ex)
					{
						Addressables.LogErrorFormat("Exception thrown during initialization of object {0}: {1}", new object[]
						{
							objectInitializationData,
							ex.ToString()
						});
					}
				}
			}
			if (list.Count > 0)
			{
				this.m_DepOp = this.m_Addressables.ResourceManager.CreateGenericGroupOperation(list, true);
				this.m_DepOp.Completed += delegate(AsyncOperationHandle<IList<AsyncOperationHandle>> obj)
				{
					bool flag = obj.Status == AsyncOperationStatus.Succeeded;
					base.Complete(true, flag, flag ? "" : string.Format("{0}, status={1}, result={2} failed initialization.", obj.DebugName, obj.Status, obj.Result));
					this.m_DepOp.Release();
				};
				return;
			}
			base.Complete(true, true, "");
		}

		private AsyncOperationHandle<ResourceManagerRuntimeData> m_RtdOp;

		private AddressablesImpl m_Addressables;

		private AsyncOperationHandle<IList<AsyncOperationHandle>> m_DepOp;
	}
}
