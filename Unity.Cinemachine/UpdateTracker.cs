using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine
{
	internal class UpdateTracker
	{
		[RuntimeInitializeOnLoadMethod]
		private static void InitializeModule()
		{
			UpdateTracker.s_UpdateStatus.Clear();
		}

		private static void UpdateTargets(UpdateTracker.UpdateClock currentClock)
		{
			int frameCount = Time.frameCount;
			Dictionary<Transform, UpdateTracker.UpdateStatus>.Enumerator enumerator = UpdateTracker.s_UpdateStatus.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<Transform, UpdateTracker.UpdateStatus> keyValuePair = enumerator.Current;
				if (keyValuePair.Key == null)
				{
					UpdateTracker.s_ToDelete.Add(keyValuePair.Key);
				}
				else
				{
					keyValuePair.Value.OnUpdate(frameCount, currentClock, keyValuePair.Key.localToWorldMatrix);
				}
			}
			for (int i = UpdateTracker.s_ToDelete.Count - 1; i >= 0; i--)
			{
				UpdateTracker.s_UpdateStatus.Remove(UpdateTracker.s_ToDelete[i]);
			}
			UpdateTracker.s_ToDelete.Clear();
			enumerator.Dispose();
		}

		public static UpdateTracker.UpdateClock GetPreferredUpdate(Transform target)
		{
			if (Application.isPlaying && target != null)
			{
				UpdateTracker.UpdateStatus updateStatus;
				if (UpdateTracker.s_UpdateStatus.TryGetValue(target, out updateStatus))
				{
					return updateStatus.PreferredUpdate;
				}
				updateStatus = new UpdateTracker.UpdateStatus(Time.frameCount, target.localToWorldMatrix);
				UpdateTracker.s_UpdateStatus.Add(target, updateStatus);
			}
			return UpdateTracker.UpdateClock.Late;
		}

		public static void OnUpdate(UpdateTracker.UpdateClock currentClock, object context)
		{
			if (UpdateTracker.s_LastUpdateContext == null || UpdateTracker.s_LastUpdateContext == context)
			{
				UpdateTracker.s_LastUpdateContext = context;
				UpdateTracker.UpdateTargets(currentClock);
			}
		}

		public static void ForgetContext(object context)
		{
			if (UpdateTracker.s_LastUpdateContext == context)
			{
				UpdateTracker.s_LastUpdateContext = null;
			}
		}

		private static Dictionary<Transform, UpdateTracker.UpdateStatus> s_UpdateStatus = new Dictionary<Transform, UpdateTracker.UpdateStatus>();

		private static List<Transform> s_ToDelete = new List<Transform>();

		private static object s_LastUpdateContext;

		public enum UpdateClock
		{
			Fixed = 1,
			Late
		}

		private class UpdateStatus
		{
			public UpdateTracker.UpdateClock PreferredUpdate { get; private set; }

			public UpdateStatus(int currentFrame, Matrix4x4 pos)
			{
				this.m_WindowStart = currentFrame;
				this.m_LastFrameUpdated = Time.frameCount;
				this.PreferredUpdate = UpdateTracker.UpdateClock.Late;
				this.m_LastPos = pos;
			}

			public void OnUpdate(int currentFrame, UpdateTracker.UpdateClock currentClock, Matrix4x4 pos)
			{
				if (this.m_LastPos == pos)
				{
					return;
				}
				if (currentClock == UpdateTracker.UpdateClock.Late)
				{
					this.m_NumWindowLateUpdateMoves++;
				}
				else if (this.m_LastFrameUpdated != currentFrame)
				{
					this.m_NumWindowFixedUpdateMoves++;
				}
				this.m_LastPos = pos;
				UpdateTracker.UpdateClock preferredUpdate = UpdateTracker.UpdateClock.Late;
				if (this.m_NumWindowFixedUpdateMoves > 3 && this.m_NumWindowLateUpdateMoves < this.m_NumWindowFixedUpdateMoves / 3)
				{
					preferredUpdate = UpdateTracker.UpdateClock.Fixed;
				}
				if (this.m_NumWindows == 0)
				{
					this.PreferredUpdate = preferredUpdate;
				}
				if (this.m_WindowStart + 30 <= currentFrame)
				{
					this.PreferredUpdate = preferredUpdate;
					this.m_NumWindows++;
					this.m_WindowStart = currentFrame;
					this.m_NumWindowLateUpdateMoves = ((this.PreferredUpdate == UpdateTracker.UpdateClock.Late) ? 1 : 0);
					this.m_NumWindowFixedUpdateMoves = ((this.PreferredUpdate == UpdateTracker.UpdateClock.Fixed) ? 1 : 0);
				}
			}

			private const int kWindowSize = 30;

			private int m_WindowStart;

			private int m_NumWindowLateUpdateMoves;

			private int m_NumWindowFixedUpdateMoves;

			private int m_NumWindows;

			private int m_LastFrameUpdated;

			private Matrix4x4 m_LastPos;
		}
	}
}
