using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class Controller : DataModifier<ControllerDataAsset>, IController
	{
		public virtual Handedness Handedness
		{
			get
			{
				return base.GetData().Config.Handedness;
			}
		}

		public virtual bool IsConnected
		{
			get
			{
				ControllerDataAsset data = base.GetData();
				return data.IsDataValid && data.IsConnected;
			}
		}

		public virtual bool IsPoseValid
		{
			get
			{
				ControllerDataAsset data = base.GetData();
				return data.IsDataValid && data.RootPoseOrigin > PoseOrigin.None;
			}
		}

		public virtual bool IsPointerPoseValid
		{
			get
			{
				ControllerDataAsset data = base.GetData();
				return data.IsDataValid && data.PointerPoseOrigin > PoseOrigin.None;
			}
		}

		public virtual ControllerInput ControllerInput
		{
			get
			{
				return base.GetData().Input;
			}
		}

		public virtual event Action WhenUpdated = delegate()
		{
		};

		private ITrackingToWorldTransformer TrackingToWorldTransformer
		{
			get
			{
				return base.GetData().Config.TrackingToWorldTransformer;
			}
		}

		public virtual float Scale
		{
			get
			{
				if (this.TrackingToWorldTransformer == null)
				{
					return 1f;
				}
				return this.TrackingToWorldTransformer.Transform.lossyScale.x;
			}
		}

		public virtual bool IsButtonUsageAnyActive(ControllerButtonUsage buttonUsage)
		{
			ControllerDataAsset data = base.GetData();
			return data.IsDataValid && (buttonUsage & data.Input.ButtonUsageMask) > ControllerButtonUsage.None;
		}

		public virtual bool IsButtonUsageAllActive(ControllerButtonUsage buttonUsage)
		{
			ControllerDataAsset data = base.GetData();
			return data.IsDataValid && (buttonUsage & data.Input.ButtonUsageMask) == buttonUsage;
		}

		public virtual bool TryGetPose(out Pose pose)
		{
			if (!this.IsPoseValid)
			{
				pose = Pose.identity;
				return false;
			}
			pose = base.GetData().Config.TrackingToWorldTransformer.ToWorldPose(base.GetData().RootPose);
			return true;
		}

		public virtual bool TryGetPointerPose(out Pose pose)
		{
			if (!this.IsPointerPoseValid)
			{
				pose = Pose.identity;
				return false;
			}
			pose = base.GetData().Config.TrackingToWorldTransformer.ToWorldPose(base.GetData().PointerPose);
			return true;
		}

		public override void MarkInputDataRequiresUpdate()
		{
			base.MarkInputDataRequiresUpdate();
			if (base.Started)
			{
				this.WhenUpdated();
			}
		}

		protected override void Apply(ControllerDataAsset data)
		{
		}

		public void InjectAllController(DataSource<ControllerDataAsset>.UpdateModeFlags updateMode, IDataSource updateAfter, IDataSource<ControllerDataAsset> modifyDataFromSource, bool applyModifier)
		{
			base.InjectAllDataModifier(updateMode, updateAfter, modifyDataFromSource, applyModifier);
		}
	}
}
