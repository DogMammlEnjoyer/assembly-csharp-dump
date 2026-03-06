using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandPointerPose : MonoBehaviour, IActiveState
	{
		public IHand Hand { get; private set; }

		public bool Active
		{
			get
			{
				return this.Hand.IsPointerPoseValid;
			}
		}

		protected virtual void Awake()
		{
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
				this.Hand.WhenHandUpdated += this.HandleHandUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.HandleHandUpdated;
			}
		}

		private void HandleHandUpdated()
		{
			Pose pose;
			if (this.Hand.GetPointerPose(out pose))
			{
				pose.position += pose.rotation * this._offset;
				base.transform.SetPose(pose, Space.World);
			}
		}

		public void InjectAllHandPointerPose(IHand hand, Vector3 offset)
		{
			this.InjectHand(hand);
			this.InjectOffset(offset);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		public void InjectOffset(Vector3 offset)
		{
			this._offset = offset;
		}

		[Tooltip("The hand used for ray interaction")]
		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[Tooltip("How much the ray origin is offset relative to the hand.")]
		[SerializeField]
		private Vector3 _offset;

		protected bool _started;
	}
}
