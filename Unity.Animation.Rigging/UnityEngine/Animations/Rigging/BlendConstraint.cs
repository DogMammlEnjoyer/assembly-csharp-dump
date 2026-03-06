using System;

namespace UnityEngine.Animations.Rigging
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Animation Rigging/Blend Constraint")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/BlendConstraint.html")]
	public class BlendConstraint : RigConstraint<BlendConstraintJob, BlendConstraintData, BlendConstraintJobBinder<BlendConstraintData>>
	{
		protected override void OnValidate()
		{
			base.OnValidate();
			this.m_Data.positionWeight = Mathf.Clamp01(this.m_Data.positionWeight);
			this.m_Data.rotationWeight = Mathf.Clamp01(this.m_Data.rotationWeight);
		}
	}
}
