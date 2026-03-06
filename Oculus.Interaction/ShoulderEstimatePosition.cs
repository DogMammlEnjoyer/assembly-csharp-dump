using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ShoulderEstimatePosition : MonoBehaviour
	{
		private IHmd Hmd { get; set; }

		private IHand Hand { get; set; }

		protected virtual void Awake()
		{
			this.Hmd = (this._hmd as IHmd);
			this.Hand = (this._hand as IHand);
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
			this.Hmd.TryGetRootPose(out pose);
			Quaternion rotation = Quaternion.Euler(0f, pose.rotation.eulerAngles.y, 0f);
			Vector3 vector = ShoulderEstimatePosition.ShoulderOffset * this.Hand.Scale;
			if (this.Hand.Handedness == Handedness.Left)
			{
				vector.x = -vector.x;
			}
			Vector3 position = pose.position + rotation * vector;
			base.transform.SetPositionAndRotation(position, rotation);
		}

		public void InjectAllShoulderPosition(IHmd hmd, IHand hand)
		{
			this.InjectHmd(hmd);
			this.InjectHand(hand);
		}

		public void InjectHmd(IHmd hmd)
		{
			this._hmd = (hmd as Object);
			this.Hmd = hmd;
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		[SerializeField]
		[Interface(typeof(IHmd), new Type[]
		{

		})]
		private Object _hmd;

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		private static readonly Vector3 ShoulderOffset = new Vector3(0.13f, -0.25f, -0.13f);

		protected bool _started;
	}
}
