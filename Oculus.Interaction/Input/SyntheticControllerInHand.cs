using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class SyntheticControllerInHand : Controller
	{
		private IHand RawHand { get; set; }

		private IHand SyntheticHand { get; set; }

		protected virtual void Awake()
		{
			if (this.RawHand == null)
			{
				this.RawHand = (this._rawHand as IHand);
			}
			if (this.SyntheticHand == null)
			{
				this.SyntheticHand = (this._syntheticHand as IHand);
			}
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this._rawHand != null;
			this._syntheticHand != null;
			this.EndStart(ref this._started);
		}

		protected override void LateUpdate()
		{
			if (this._applyModifier)
			{
				this.UpdateOffsets(this.ModifyDataFromSource.GetData());
			}
			base.LateUpdate();
		}

		protected override void Apply(ControllerDataAsset data)
		{
			this.ApplyOffsets(data);
		}

		private void UpdateOffsets(ControllerDataAsset data)
		{
			Pose pose;
			if (this.TryGetTrackingRoot(this.RawHand, data, out pose))
			{
				this._handToController = PoseUtils.Delta(pose, data.RootPose);
				this._rootToPointer = PoseUtils.Delta(data.RootPose, data.PointerPose);
			}
		}

		private void ApplyOffsets(ControllerDataAsset data)
		{
			Pose pose;
			if (this.TryGetTrackingRoot(this.SyntheticHand, data, out pose))
			{
				PoseUtils.Multiply(pose, this._handToController, ref data.RootPose);
				PoseUtils.Multiply(data.RootPose, this._rootToPointer, ref data.PointerPose);
			}
		}

		private bool TryGetTrackingRoot(IHand hand, ControllerDataAsset controller, out Pose root)
		{
			if (hand != null && hand.GetRootPose(out root))
			{
				ITrackingToWorldTransformer trackingToWorldTransformer = controller.Config.TrackingToWorldTransformer;
				if (trackingToWorldTransformer != null)
				{
					root = trackingToWorldTransformer.ToTrackingPose(root);
				}
				return true;
			}
			root = Pose.identity;
			return false;
		}

		public void InjectAllSyntheticControllerInHand(DataSource<ControllerDataAsset>.UpdateModeFlags updateMode, IDataSource updateAfter, IDataSource<ControllerDataAsset> modifyDataFromSource, bool applyModifier)
		{
			base.InjectAllController(updateMode, updateAfter, modifyDataFromSource, applyModifier);
		}

		public void InjectOptionalRawHand(IHand rawHand)
		{
			this._rawHand = (rawHand as Object);
			this.RawHand = rawHand;
		}

		public void InjectOptionalSyntheticHand(IHand syntheticHand)
		{
			this._syntheticHand = (syntheticHand as Object);
			this.SyntheticHand = syntheticHand;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		[Optional]
		private Object _rawHand;

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		[Optional]
		private Object _syntheticHand;

		private Pose _handToController = Pose.identity;

		private Pose _rootToPointer = Pose.identity;
	}
}
