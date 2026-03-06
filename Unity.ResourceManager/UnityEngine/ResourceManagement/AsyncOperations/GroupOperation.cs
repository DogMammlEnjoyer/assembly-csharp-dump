using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.Exceptions;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
	internal class GroupOperation : AsyncOperationBase<IList<AsyncOperationHandle>>, ICachable
	{
		public GroupOperation()
		{
			this.m_InternalOnComplete = new Action<AsyncOperationHandle>(this.OnOperationCompleted);
			base.Result = new List<AsyncOperationHandle>();
		}

		protected override bool InvokeWaitForCompletion()
		{
			if (base.IsDone || base.Result == null)
			{
				return true;
			}
			foreach (AsyncOperationHandle asyncOperationHandle in base.Result)
			{
				asyncOperationHandle.WaitForCompletion();
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

		IOperationCacheKey ICachable.Key { get; set; }

		internal IList<AsyncOperationHandle> GetDependentOps()
		{
			return base.Result;
		}

		public override void GetDependencies(List<AsyncOperationHandle> deps)
		{
			deps.AddRange(base.Result);
		}

		internal override void ReleaseDependencies()
		{
			for (int i = 0; i < base.Result.Count; i++)
			{
				if (base.Result[i].IsValid())
				{
					base.Result[i].Release();
				}
			}
			base.Result.Clear();
		}

		internal override DownloadStatus GetDownloadStatus(HashSet<object> visited)
		{
			DownloadStatus result = new DownloadStatus
			{
				IsDone = base.IsDone
			};
			for (int i = 0; i < base.Result.Count; i++)
			{
				if (base.Result[i].IsValid())
				{
					DownloadStatus downloadStatus = base.Result[i].InternalGetDownloadStatus(visited);
					result.DownloadedBytes += downloadStatus.DownloadedBytes;
					result.TotalBytes += downloadStatus.TotalBytes;
				}
			}
			return result;
		}

		private bool DependenciesAreUnchanged(List<AsyncOperationHandle> deps)
		{
			if (this.m_CachedDependencyLocations.Count != deps.Count)
			{
				return false;
			}
			foreach (AsyncOperationHandle asyncOperationHandle in deps)
			{
				if (!this.m_CachedDependencyLocations.Contains(asyncOperationHandle.LocationName))
				{
					return false;
				}
			}
			return true;
		}

		protected override string DebugName
		{
			get
			{
				List<AsyncOperationHandle> list = new List<AsyncOperationHandle>();
				this.GetDependencies(list);
				if (list.Count == 0)
				{
					return "Dependencies";
				}
				if (this.debugName != null && this.DependenciesAreUnchanged(list))
				{
					return this.debugName;
				}
				this.m_CachedDependencyLocations.Clear();
				string text = "Dependencies [";
				for (int i = 0; i < list.Count; i++)
				{
					string text2 = list[i].LocationName;
					this.m_CachedDependencyLocations.Add(text2);
					if (text2 != null)
					{
						if (text2.Length > 45)
						{
							text2 = AsyncOperationBase<object>.ShortenPath(text2, true);
							text2 = text2.Substring(0, Math.Min(45, text2.Length)) + "...";
						}
						if (i == list.Count - 1)
						{
							text += text2;
						}
						else
						{
							text = text + text2 + ", ";
						}
					}
				}
				text += "]";
				if (text.Length > 2000)
				{
					text = text.Substring(0, 2000) + "...";
				}
				this.debugName = text;
				return this.debugName;
			}
		}

		protected override void Execute()
		{
			this.m_LoadedCount = 0;
			for (int i = 0; i < base.Result.Count; i++)
			{
				if (base.Result[i].IsDone)
				{
					this.m_LoadedCount++;
				}
				else
				{
					base.Result[i].Completed += this.m_InternalOnComplete;
				}
			}
			this.CompleteIfDependenciesComplete();
		}

		private void CompleteIfDependenciesComplete()
		{
			if (this.m_LoadedCount == base.Result.Count)
			{
				bool success = true;
				OperationException exception = null;
				if (!this.m_Settings.HasFlag(GroupOperation.GroupOperationSettings.AllowFailedDependencies))
				{
					for (int i = 0; i < base.Result.Count; i++)
					{
						if (base.Result[i].Status != AsyncOperationStatus.Succeeded)
						{
							success = false;
							exception = new OperationException("GroupOperation failed because one of its dependencies failed", base.Result[i].OperationException);
							break;
						}
					}
				}
				base.Complete(base.Result, success, exception, this.m_Settings.HasFlag(GroupOperation.GroupOperationSettings.ReleaseDependenciesOnFailure));
			}
		}

		protected override void Destroy()
		{
			this.ReleaseDependencies();
			this.debugName = null;
		}

		protected override float Progress
		{
			get
			{
				float num = 0f;
				for (int i = 0; i < base.Result.Count; i++)
				{
					AsyncOperationHandle asyncOperationHandle = base.Result[i];
					if (!asyncOperationHandle.IsDone)
					{
						num += asyncOperationHandle.PercentComplete;
					}
					else
					{
						num += 1f;
					}
				}
				return num / (float)base.Result.Count;
			}
		}

		public void Init(List<AsyncOperationHandle> operations, bool releaseDependenciesOnFailure = true, bool allowFailedDependencies = false)
		{
			base.Result = new List<AsyncOperationHandle>(operations);
			this.m_Settings = (releaseDependenciesOnFailure ? GroupOperation.GroupOperationSettings.ReleaseDependenciesOnFailure : GroupOperation.GroupOperationSettings.None);
			if (allowFailedDependencies)
			{
				this.m_Settings |= GroupOperation.GroupOperationSettings.AllowFailedDependencies;
			}
		}

		public void Init(List<AsyncOperationHandle> operations, GroupOperation.GroupOperationSettings settings)
		{
			base.Result = new List<AsyncOperationHandle>(operations);
			this.m_Settings = settings;
		}

		private void OnOperationCompleted(AsyncOperationHandle op)
		{
			this.m_LoadedCount++;
			this.CompleteIfDependenciesComplete();
		}

		private Action<AsyncOperationHandle> m_InternalOnComplete;

		private int m_LoadedCount;

		private GroupOperation.GroupOperationSettings m_Settings;

		private string debugName;

		private const int k_MaxDisplayedLocationLength = 45;

		private const int k_MaxDebugNameLength = 2000;

		private HashSet<string> m_CachedDependencyLocations = new HashSet<string>();

		[Flags]
		public enum GroupOperationSettings
		{
			None = 0,
			ReleaseDependenciesOnFailure = 1,
			AllowFailedDependencies = 2
		}
	}
}
