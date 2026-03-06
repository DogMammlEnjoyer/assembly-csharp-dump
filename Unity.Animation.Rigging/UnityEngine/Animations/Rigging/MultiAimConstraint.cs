using System;

namespace UnityEngine.Animations.Rigging
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Animation Rigging/Multi-Aim Constraint")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/MultiAimConstraint.html")]
	public class MultiAimConstraint : RigConstraint<MultiAimConstraintJob, MultiAimConstraintData, MultiAimConstraintJobBinder<MultiAimConstraintData>>
	{
		protected override void OnValidate()
		{
			base.OnValidate();
			WeightedTransformArray sourceObjects = this.m_Data.sourceObjects;
			WeightedTransformArray.OnValidate(ref sourceObjects, 0f, 1f);
			this.m_Data.sourceObjects = sourceObjects;
			Vector2 limits = this.m_Data.limits;
			limits.x = Mathf.Clamp(limits.x, -180f, 180f);
			limits.y = Mathf.Clamp(limits.y, -180f, 180f);
			this.m_Data.limits = limits;
		}
	}
}
