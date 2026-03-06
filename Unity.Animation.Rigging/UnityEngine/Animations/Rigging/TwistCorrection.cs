using System;

namespace UnityEngine.Animations.Rigging
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Animation Rigging/Twist Correction")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/TwistCorrection.html")]
	public class TwistCorrection : RigConstraint<TwistCorrectionJob, TwistCorrectionData, TwistCorrectionJobBinder<TwistCorrectionData>>
	{
		protected override void OnValidate()
		{
			base.OnValidate();
			WeightedTransformArray twistNodes = this.m_Data.twistNodes;
			WeightedTransformArray.OnValidate(ref twistNodes, -1f, 1f);
			this.m_Data.twistNodes = twistNodes;
		}
	}
}
