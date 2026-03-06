using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class HmdOffset : MonoBehaviour
	{
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
				this.Hmd.WhenUpdated += this.HandleHmdUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hmd.WhenUpdated -= this.HandleHmdUpdated;
			}
		}

		protected virtual void HandleHmdUpdated()
		{
			Pose pose;
			if (!this.Hmd.TryGetRootPose(out pose))
			{
				return;
			}
			Vector3 position = pose.position;
			Quaternion rotation = pose.rotation;
			Vector3 eulerAngles = rotation.eulerAngles;
			Quaternion rhs = Quaternion.Euler(new Vector3(eulerAngles.x, 0f, 0f));
			Quaternion rhs2 = Quaternion.Euler(new Vector3(0f, eulerAngles.y, 0f));
			Quaternion rhs3 = Quaternion.Euler(new Vector3(0f, 0f, eulerAngles.z));
			Quaternion lhs = Quaternion.identity;
			if (!this._disableYawFromSource)
			{
				lhs *= rhs2;
			}
			if (!this._disablePitchFromSource)
			{
				lhs *= rhs;
			}
			if (!this._disableRollFromSource)
			{
				lhs *= rhs3;
			}
			Quaternion rotation2 = lhs * Quaternion.Euler(this._offsetRotation);
			base.transform.position = position + rotation2 * this._offsetTranslation;
			base.transform.rotation = rotation2;
		}

		public void InjectAllHmdOffset(IHmd hmd)
		{
			this.InjectHmd(hmd);
		}

		public void InjectHmd(IHmd hmd)
		{
			this._hmd = (hmd as Object);
			this.Hmd = hmd;
		}

		public void InjectOptionalOffsetTranslation(Vector3 val)
		{
			this._offsetTranslation = val;
		}

		public void InjectOptionalOffsetRotation(Vector3 val)
		{
			this._offsetRotation = val;
		}

		public void InjectOptionalDisablePitchFromSource(bool val)
		{
			this._disablePitchFromSource = val;
		}

		public void InjectOptionalDisableYawFromSource(bool val)
		{
			this._disableYawFromSource = val;
		}

		public void InjectOptionalDisableRollFromSource(bool val)
		{
			this._disableRollFromSource = val;
		}

		[SerializeField]
		[Interface(typeof(IHmd), new Type[]
		{

		})]
		private Object _hmd;

		private IHmd Hmd;

		[SerializeField]
		private Vector3 _offsetTranslation = Vector3.zero;

		[SerializeField]
		private Vector3 _offsetRotation = Vector3.zero;

		[SerializeField]
		private bool _disablePitchFromSource;

		[SerializeField]
		private bool _disableYawFromSource;

		[SerializeField]
		private bool _disableRollFromSource;

		protected bool _started;
	}
}
