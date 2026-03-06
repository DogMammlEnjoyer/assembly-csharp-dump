using System;
using Fusion.LagCompensation;
using UnityEngine;

namespace Fusion
{
	[ScriptHelp(BackColor = ScriptHeaderBackColor.Sand)]
	[DisallowMultipleComponent]
	public class RunnerLagCompensationGizmos : Behaviour
	{
		private void Awake()
		{
			this._runner = base.GetComponentInParent<NetworkRunner>();
			if (this._runner == null)
			{
				Debug.LogWarning(string.Format("{0} was not able to find the NetworkRunner reference. Destroying the component.", this));
				Object.Destroy(this);
			}
		}

		private void OnDrawGizmos()
		{
			if (!(this._runner == null) && this._runner.IsRunning && this._runner.GetVisible())
			{
				HitboxManager lagCompensation = this._runner.LagCompensation;
				if (((lagCompensation != null) ? lagCompensation.DrawInfo : null) != null)
				{
					if (this.DrawBroadphaseNodes)
					{
						this.RenderBHVBroadphase();
					}
					if (this.DrawSnapshotHistory)
					{
						this.RenderHitboxHistory();
					}
					return;
				}
			}
		}

		private void RenderHitboxHistory()
		{
			Gizmos.color = (this._runner.IsServer ? this.StateAuthHitboxCollor : this.NonStateAuthHitboxCollor);
			foreach (HitboxColliderContainerDraw hitboxColliderContainerDraw in this._runner.LagCompensation.DrawInfo.SnapshotHistoryDraw)
			{
				foreach (ColliderDrawInfo colliderDrawInfo in hitboxColliderContainerDraw)
				{
					Gizmos.matrix = colliderDrawInfo.LocalToWorldMatrix;
					switch (colliderDrawInfo.Type)
					{
					case HitboxTypes.Box:
						Gizmos.DrawWireCube(colliderDrawInfo.Offset, colliderDrawInfo.BoxExtents * 2f);
						break;
					case HitboxTypes.Sphere:
						Gizmos.DrawWireSphere(colliderDrawInfo.Offset, colliderDrawInfo.Radius);
						break;
					case HitboxTypes.Capsule:
						LagCompensationDraw.GizmosDrawWireCapsule(colliderDrawInfo.CapsuleTopCenter, colliderDrawInfo.CapsuleBottomCenter, colliderDrawInfo.Radius);
						break;
					default:
						Debug.LogWarning(string.Format("HitboxType {0} not supported to draw.", colliderDrawInfo.Type));
						break;
					}
				}
			}
			Gizmos.matrix = Matrix4x4.identity;
		}

		private void RenderBHVBroadphase()
		{
			Color green = Color.green;
			foreach (BVHNodeDrawInfo bvhnodeDrawInfo in this._runner.LagCompensation.DrawInfo.BVHDraw)
			{
				Gizmos.color = green + Color.red * (float)bvhnodeDrawInfo.Depth / (float)bvhnodeDrawInfo.MaxDepth;
				Gizmos.DrawWireCube(bvhnodeDrawInfo.Bounds.center, bvhnodeDrawInfo.Bounds.size);
			}
		}

		public bool DrawSnapshotHistory;

		public bool DrawBroadphaseNodes;

		public Color StateAuthHitboxCollor = Color.green;

		public Color NonStateAuthHitboxCollor = Color.cyan;

		private NetworkRunner _runner;
	}
}
