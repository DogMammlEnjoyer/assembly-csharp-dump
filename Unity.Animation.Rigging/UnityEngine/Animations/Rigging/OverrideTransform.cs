using System;

namespace UnityEngine.Animations.Rigging
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Animation Rigging/Override Transform")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/OverrideTransform.html")]
	public class OverrideTransform : RigConstraint<OverrideTransformJob, OverrideTransformData, OverrideTransformJobBinder<OverrideTransformData>>
	{
		protected override void OnValidate()
		{
			base.OnValidate();
			this.m_Data.positionWeight = Mathf.Clamp01(this.m_Data.positionWeight);
			this.m_Data.rotationWeight = Mathf.Clamp01(this.m_Data.rotationWeight);
		}
	}
}
