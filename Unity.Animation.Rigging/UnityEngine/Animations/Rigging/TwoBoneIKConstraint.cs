using System;

namespace UnityEngine.Animations.Rigging
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Animation Rigging/Two Bone IK Constraint")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/TwoBoneIKConstraint.html")]
	public class TwoBoneIKConstraint : RigConstraint<TwoBoneIKConstraintJob, TwoBoneIKConstraintData, TwoBoneIKConstraintJobBinder<TwoBoneIKConstraintData>>
	{
		protected override void OnValidate()
		{
			base.OnValidate();
			this.m_Data.hintWeight = Mathf.Clamp01(this.m_Data.hintWeight);
			this.m_Data.targetPositionWeight = Mathf.Clamp01(this.m_Data.targetPositionWeight);
			this.m_Data.targetRotationWeight = Mathf.Clamp01(this.m_Data.targetRotationWeight);
		}
	}
}
