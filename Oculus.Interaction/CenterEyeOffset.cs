using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class CenterEyeOffset : MonoBehaviour
	{
		public IHmd Hmd { get; private set; }

		protected virtual void Awake()
		{
			this.Hmd = (this._hmd as IHmd);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Hmd.WhenUpdated += this.HandleUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hmd.WhenUpdated -= this.HandleUpdated;
			}
		}

		private void HandleUpdated()
		{
			Pose pose;
			if (this.Hmd.TryGetRootPose(out pose))
			{
				this.GetOffset(ref this._cachedPose);
				ref this._cachedPose.Postmultiply(pose);
				base.transform.SetPose(this._cachedPose, Space.World);
			}
		}

		public void GetOffset(ref Pose pose)
		{
			pose.position = this._offset;
			pose.rotation = this._rotation;
		}

		public void GetWorldPose(ref Pose pose)
		{
			pose.position = base.transform.position;
			pose.rotation = base.transform.rotation;
		}

		public void InjectOffset(Vector3 offset)
		{
			this._offset = offset;
		}

		public void InjectRotation(Quaternion rotation)
		{
			this._rotation = rotation;
		}

		public void InjectAllCenterEyeOffset(IHmd hmd, Vector3 offset, Quaternion rotation)
		{
			this.InjectHmd(hmd);
			this.InjectOffset(offset);
			this.InjectRotation(rotation);
		}

		public void InjectHmd(IHmd hmd)
		{
			this.Hmd = hmd;
			this._hmd = (hmd as Object);
		}

		[SerializeField]
		[Interface(typeof(IHmd), new Type[]
		{

		})]
		private Object _hmd;

		[SerializeField]
		private Vector3 _offset;

		[SerializeField]
		private Quaternion _rotation = Quaternion.identity;

		private Pose _cachedPose = Pose.identity;

		protected bool _started;
	}
}
