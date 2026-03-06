using System;

namespace UnityEngine.Animations.Rigging
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Animation Rigging/Chain IK Constraint")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/ChainIKConstraint.html")]
	public class ChainIKConstraint : RigConstraint<ChainIKConstraintJob, ChainIKConstraintData, ChainIKConstraintJobBinder<ChainIKConstraintData>>
	{
		protected override void OnValidate()
		{
			base.OnValidate();
			this.m_Data.chainRotationWeight = Mathf.Clamp01(this.m_Data.chainRotationWeight);
			this.m_Data.tipRotationWeight = Mathf.Clamp01(this.m_Data.tipRotationWeight);
			this.m_Data.maxIterations = Mathf.Clamp(this.m_Data.maxIterations, 1, 50);
			this.m_Data.tolerance = Mathf.Clamp(this.m_Data.tolerance, 0f, 0.01f);
		}
	}
}
